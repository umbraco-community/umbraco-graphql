using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Attributes;

namespace Our.Umbraco.GraphQL.Adapters.Resolvers
{
    public class FieldResolver : IFieldResolver
    {
        private readonly FieldType _fieldType;
        private readonly IServiceProvider _serviceProvider;

        public FieldResolver(FieldType fieldType, IServiceProvider serviceProvider)
        {
            _fieldType = fieldType ?? throw new ArgumentNullException(nameof(fieldType));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public object Resolve(IResolveFieldContext context)
        {
            var memberInfo = _fieldType.GetMetadata<MemberInfo>(nameof(MemberInfo));
            var source = context.Source;
            if(source == null || memberInfo.DeclaringType.IsInstanceOfType(source) == false)
                source = _serviceProvider.GetService(memberInfo.DeclaringType);

            switch (memberInfo)
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

        private object CallMethod(MethodInfo methodInfo, object source, IResolveFieldContext context)
        {
            var parameters = methodInfo.GetParameters().ToList();
            var arguments = new object[parameters.Count];

            var argumentsIndex = 0;
            for (var i = 0; i < parameters.Count; i++)
            {
                var parameterInfo = parameters[i];
                var parameterType = parameterInfo.ParameterType;

                if (parameterInfo.GetCustomAttribute<InjectAttribute>() != null)
                {
                    arguments[i] = _serviceProvider.GetService(parameterType);
                    continue;
                }

                if (parameterInfo.ParameterType == typeof(CancellationToken))
                {
                    arguments[i] = context.CancellationToken;
                    continue;
                }

                var argument = _fieldType.Arguments[argumentsIndex];
                argumentsIndex++;

                arguments[i] = context.GetArgument(parameterType, parameterInfo.Name, argument.DefaultValue);
            }

            return methodInfo.Invoke(source, arguments);
        }
    }
}
