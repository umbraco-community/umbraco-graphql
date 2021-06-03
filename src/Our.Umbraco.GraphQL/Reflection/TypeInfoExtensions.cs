using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL.Types;

namespace Our.Umbraco.GraphQL.Reflection
{
    internal static class TypeInfoExtensions
    {
        public static bool IsNullable(this TypeInfo type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        public static  TypeInfo GetReturnType(this MemberInfo memberInfo)
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

        public static TypeInfo Unwrap(this TypeInfo typeInfo)
        {
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Task<>))
                typeInfo = typeInfo.GenericTypeArguments[0].GetTypeInfo();

            var isNullable = typeInfo.IsNullable();
            if (isNullable)
                return typeInfo.GenericTypeArguments[0].GetTypeInfo();

            var enumerableArgument = GetEnumerableArgument(typeInfo);
            return enumerableArgument != null ? enumerableArgument.GetTypeInfo() : typeInfo;
        }

        public static TypeInfo Wrap(this TypeInfo graphType, TypeInfo typeInfo, bool isNonNull, bool isNonNullItem)
            => Wrap(graphType, typeInfo, isNonNull, isNonNullItem, true, out _);

        public static TypeInfo Wrap(this TypeInfo graphType, TypeInfo typeInfo, bool isNonNull, bool isNonNullItem, bool checkEnumerable, out bool isEnumerable)
        {
            isEnumerable = false;

            if (graphType == null)
                return null;

            var enumerableArgument = checkEnumerable ? GetEnumerableArgument(typeInfo) : null;
            isEnumerable = enumerableArgument != null;

            if (typeInfo.IsValueType && typeInfo.IsNullable() == false || enumerableArgument != null &&
                (enumerableArgument.IsValueType && enumerableArgument.IsNullable() == false || isNonNullItem))
                graphType = typeof(NonNullGraphType<>).MakeGenericType(graphType).GetTypeInfo();

            if (isEnumerable)
                graphType = typeof(ListGraphType<>).MakeGenericType(graphType).GetTypeInfo();

            if (isNonNull && typeof(NonNullGraphType).IsAssignableFrom(graphType) == false)
                graphType = typeof(NonNullGraphType<>).MakeGenericType(graphType).GetTypeInfo();

            return graphType;
        }

        public static TypeInfo GetEnumerableArgument(this TypeInfo typeInfo)
        {
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Task<>))
                typeInfo = typeInfo.GenericTypeArguments[0].GetTypeInfo();

            if (typeInfo == typeof(string))
                return null;

            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return typeInfo.GenericTypeArguments[0].GetTypeInfo();

            var enumerableInterface = typeInfo.ImplementedInterfaces.FirstOrDefault(x =>
                x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return enumerableInterface?.GenericTypeArguments[0].GetTypeInfo();
        }
    }
}
