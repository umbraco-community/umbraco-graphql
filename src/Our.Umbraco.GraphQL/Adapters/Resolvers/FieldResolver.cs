using System;
using System.Linq;
using System.Reflection;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Attributes;

namespace Our.Umbraco.GraphQL.Adapters.Resolvers
{
    public class FieldResolver : IFieldResolver
    {
        private readonly MemberInfo _memberInfo;
        private readonly IDependencyResolver _dependencyResolver;

        public FieldResolver(MemberInfo memberInfo, IDependencyResolver dependencyResolver)
        {
            _memberInfo = memberInfo ?? throw new ArgumentNullException(nameof(memberInfo));
            _dependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
        }

        public object Resolve(ResolveFieldContext context)
        {
            var source = context.Source ?? _dependencyResolver.Resolve(_memberInfo.DeclaringType);

            switch (_memberInfo)
            {
                case FieldInfo fieldInfo:
                    return fieldInfo.GetValue(source);
                case MethodInfo methodInfo:
                    return CallMethod(methodInfo, source, context);
                case PropertyInfo propertyInfo:
                    return propertyInfo.GetValue(source);
                default:
                    throw new ArgumentOutOfRangeException(nameof(context));
            }
        }

        private object CallMethod(MethodInfo methodInfo, object source,  ResolveFieldContext context)
        {
            var arguments = methodInfo.GetParameters().ToList();
            var parameters = new object[arguments.Count];
            for (var i = 0; i < arguments.Count; i++)
            {
                var argument = arguments[i];
                var parameterType = argument.ParameterType;

                if (argument.GetCustomAttribute<InjectAttribute>() != null)
                {
                    parameters[i] = _dependencyResolver.Resolve(parameterType);
                    continue;
                }

                parameters[i] = context.GetArgument(parameterType, argument.Name, argument.DefaultValue);
            }

            return methodInfo.Invoke(source, parameters);
        }
    }
}
