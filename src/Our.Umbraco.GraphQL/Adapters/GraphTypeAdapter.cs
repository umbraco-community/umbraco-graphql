using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQL;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Adapters.Resolvers;
using Our.Umbraco.GraphQL.Adapters.Types.Resolution;
using Our.Umbraco.GraphQL.Adapters.Visitors;
using Our.Umbraco.GraphQL.Attributes;

namespace Our.Umbraco.GraphQL.Adapters
{
    public class GraphTypeAdapter : IGraphTypeAdapter
    {
        private readonly Dictionary<TypeInfo, IGraphType> _cache;
        private readonly ITypeRegistry _typeRegistry;
        private readonly IDependencyResolver _dependencyResolver;
        private readonly IGraphVisitor _visitor;

        public GraphTypeAdapter(ITypeRegistry typeRegistry, IDependencyResolver dependencyResolver, IGraphVisitor visitor)
        {
            _typeRegistry = typeRegistry ?? throw new ArgumentNullException(nameof(typeRegistry));
            _dependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
            _visitor = visitor;
            _cache = new Dictionary<TypeInfo, IGraphType>();
        }

        public IGraphType Adapt<T>()
        {
            return Adapt(typeof(T).GetTypeInfo());
        }

        public IGraphType Adapt(TypeInfo typeInfo)
        {
            if (typeInfo == null) throw new ArgumentNullException(nameof(typeInfo));

            var unwrappedTypeInfo = UnwrapTypeInfo(typeInfo);
            var graphType = TryGetFromCache(unwrappedTypeInfo, typeInfo);
            if (graphType != null) return graphType;

            if (unwrappedTypeInfo.IsEnum)
            {
                graphType = CreateGraphType(unwrappedTypeInfo,
                    typeof(EnumerationGraphType<>).MakeGenericType(unwrappedTypeInfo));
            }
            else if (unwrappedTypeInfo.IsInterface || unwrappedTypeInfo.IsAbstract)
            {
                graphType = CreateGraphType(unwrappedTypeInfo,
                    typeof(InterfaceGraphType<>).MakeGenericType(unwrappedTypeInfo));
                _visitor?.Visit((IInterfaceGraphType) graphType);
            }
            else
            {
                graphType = CreateGraphType(unwrappedTypeInfo,
                    typeof(ObjectGraphType<>).MakeGenericType(unwrappedTypeInfo));
                _visitor?.Visit((IObjectGraphType) graphType);
            }

            return Wrap(typeInfo, graphType);
        }

        private IGraphType AdaptInput(TypeInfo typeInfo)
        {
            var initialType = typeInfo;
            typeInfo = UnwrapTypeInfo(typeInfo);
            var graphType = TryGetFromCache(typeInfo, initialType);
            if (graphType != null) return graphType;

            graphType = CreateGraphType(typeInfo, typeof(InputObjectGraphType<>).MakeGenericType(typeInfo));
            _visitor?.Visit((IInputObjectGraphType) graphType);
            return Wrap(initialType, graphType);
        }

        private void AddFields(TypeInfo typeInfo, IComplexGraphType graphType)
        {
            do
            {
                var fields = typeInfo.DeclaredFields.Where(x =>
                    x.IsPublic && x.IsStatic == false && x.FieldType != typeof(object));
                var methods = typeInfo.DeclaredMethods.Where(x =>
                    x.IsPublic && x.IsStatic == false && x.IsSpecialName == false && x.ReturnType != typeof(void) &&
                    x.ReturnType != typeof(object));
                var properties = typeInfo.DeclaredProperties.Where(x =>
                    x.CanRead && x.GetMethod.IsPublic && x.GetMethod.IsStatic == false &&
                    x.GetMethod.ReturnType != typeof(object));

                var members = fields.Cast<MemberInfo>().Concat(methods).Concat(properties)
                    .Where(x => x.GetCustomAttribute<IgnoreAttribute>() == null)
                    .OrderBy(x => x.Name);

                foreach (var memberInfo in members)
                {
                    graphType.AddField(CreateField(memberInfo));
                }
            } while (typeInfo.BaseType != null && (typeInfo = typeInfo.BaseType.GetTypeInfo()) != typeof(object));
        }

        private FieldType CreateField(MemberInfo memberInfo)
        {
            var returnType = GetReturnType(memberInfo);

            var resolvedType = Adapt(returnType);
            if (memberInfo.GetCustomAttribute<NonNullAttribute>() != null)
            {
                resolvedType = WrapNonNull(resolvedType);
            }

            if (memberInfo.GetCustomAttribute<NonNullItemAttribute>() != null &&
                resolvedType is ListGraphType listGraphType)
            {
                resolvedType = WrapList(WrapNonNull(listGraphType.ResolvedType));
            }

            var fieldType = new FieldType
            {
                Arguments = CreateArguments(memberInfo),
                DefaultValue = memberInfo.GetCustomAttribute<DefaultValueAttribute>()?.DefaultValue,
                Description = memberInfo.GetCustomAttribute<DescriptionAttribute>()?.Description,
                DeprecationReason = memberInfo.GetCustomAttribute<DeprecatedAttribute>()?.DeprecationReason,
                Metadata = {{nameof(MemberInfo), memberInfo}},
                Name = memberInfo.GetCustomAttribute<NameAttribute>()?.Name ?? memberInfo.Name,
                ResolvedType = resolvedType,
                Resolver = new FieldResolver(memberInfo, _dependencyResolver)
            };

            return fieldType;
        }

        private QueryArguments CreateArguments(MemberInfo memberInfo)
        {
            if (memberInfo is MethodInfo methodInfo)
            {
                return new QueryArguments(methodInfo.GetParameters()
                    .Where(x => x.GetCustomAttribute<InjectAttribute>() == null).Select(CreateArgument));
            }

            return null;
        }

        private QueryArgument CreateArgument(ParameterInfo parameterInfo)
        {
            return new QueryArgument(AdaptInput(parameterInfo.ParameterType.GetTypeInfo()))
            {
                DefaultValue = parameterInfo.HasDefaultValue
                    ? parameterInfo.DefaultValue
                    : parameterInfo.GetCustomAttribute<DefaultValueAttribute>()?.DefaultValue,
                Description = parameterInfo.GetCustomAttribute<DescriptionAttribute>()?.Description,
                Name = parameterInfo.GetCustomAttribute<NameAttribute>()?.Name ?? parameterInfo.Name,
            };
        }

        private TypeInfo GetReturnType(MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case FieldInfo fieldInfo:
                    return fieldInfo.FieldType.GetTypeInfo();
                case MethodInfo methodInfo:
                    return methodInfo.ReturnType.GetTypeInfo();
                case PropertyInfo propertyInfo:
                    return propertyInfo.GetMethod.ReturnType.GetTypeInfo();
                default:
                    throw new ArgumentOutOfRangeException(nameof(memberInfo));
            }
        }

        private IGraphType CreateGraphType(TypeInfo typeInfo, Type graphTypeType)
        {
            var graphType = (IGraphType) Activator.CreateInstance(graphTypeType);
            graphType.Description = typeInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
            graphType.DeprecationReason = typeInfo.GetCustomAttribute<DeprecatedAttribute>()?.DeprecationReason;
            graphType.Metadata[nameof(TypeInfo)] = typeInfo;
            graphType.Name = typeInfo.GetCustomAttribute<NameAttribute>()?.Name ?? typeInfo.Name;

            _cache.Add(typeInfo, graphType);

            switch (graphType)
            {
                case IComplexGraphType complexGraphType:
                    AddFields(typeInfo, complexGraphType);
                    break;
                case EnumerationGraphType enumerationGraphType:
                {
                    foreach (var value in enumerationGraphType.Values)
                    {
                        var enumMember = typeInfo.GetDeclaredField(value.Value.ToString());
                        value.Description = enumMember.GetCustomAttribute<DescriptionAttribute>()?.Description;
                        value.DeprecationReason =
                            enumMember.GetCustomAttribute<DeprecatedAttribute>()?.DeprecationReason;
                        value.Name = enumMember.GetCustomAttribute<NameAttribute>()?.Name ?? value.Name;
                    }

                    break;
                }
            }

            return graphType;
        }

        private IGraphType TryGetFromCache(TypeInfo typeInfo, TypeInfo unwrappedTypeInfo)
        {
            if (_cache.TryGetValue(typeInfo, out var foundGraphType))
            {
                return Wrap(unwrappedTypeInfo, foundGraphType);
            }

            var foundType = _typeRegistry.Get(typeInfo);
            if (foundType != null)
            {
                return Wrap(unwrappedTypeInfo, (IGraphType) Activator.CreateInstance(foundType));;
            }

            return null;
        }

        private static TypeInfo UnwrapTypeInfo(TypeInfo typeInfo)
        {
            var isNullable = typeInfo.IsNullable();
            if (isNullable)
                return typeInfo.GenericTypeArguments[0].GetTypeInfo();

            var enumerableArgument = GetEnumerableArgument(typeInfo);
            if (enumerableArgument != null)
                return enumerableArgument.GetTypeInfo();

            return typeInfo;
        }

        private IGraphType Wrap(TypeInfo typeInfo, IGraphType graphType)
        {
            var enumerableArgument = GetEnumerableArgument(typeInfo);

            if (typeInfo.IsValueType && typeInfo.IsNullable() == false || enumerableArgument != null &&
                enumerableArgument.IsValueType && enumerableArgument.IsNullable() == false)
                graphType = WrapNonNull(graphType);

            if (enumerableArgument != null) graphType = WrapList(graphType);

            return graphType;
        }

        private static ListGraphType WrapList(IGraphType graphType)
        {
            var listGraphType =
                (ListGraphType) Activator.CreateInstance(
                    typeof(ListGraphType<>).MakeGenericType(graphType.GetType()));
            listGraphType.ResolvedType = graphType;
            return listGraphType;
        }

        private static IGraphType WrapNonNull(IGraphType graphType)
        {
            var nonNullGraphType =
                (NonNullGraphType) Activator.CreateInstance(
                    typeof(NonNullGraphType<>).MakeGenericType(graphType.GetType()));
            nonNullGraphType.ResolvedType = graphType;
            return nonNullGraphType;
        }

        private static Type GetEnumerableArgument(TypeInfo typeInfo)
        {
            if (typeInfo == typeof(string))
                return null;

            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return typeInfo.GenericTypeArguments[0];

            var enumerableInterface = typeInfo.ImplementedInterfaces.FirstOrDefault(x =>
                x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return enumerableInterface?.GenericTypeArguments[0];
        }
    }
}
