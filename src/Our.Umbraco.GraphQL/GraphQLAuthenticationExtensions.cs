using GraphQL.Builders;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.GraphQL
{
    // Based on the example used here https://graphql-dotnet.github.io/docs/getting-started/authorization
    public static class GraphQLAuthenticationExtensions
    {
        public static readonly string PermissionsKey = "Permissions";

        /// <summary>
        /// Checks if this field has any permissions set or not, if not then anyone can view it
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool RequiresPermissions(this IProvideMetadata type)
        {
            var permissions = type.GetMetadata<IEnumerable<string>>(PermissionsKey, new List<string>());
            return permissions.Any();
        }

        public static bool CanAccess(this IProvideMetadata type, IEnumerable<string> claims)
        {
            var permissions = type.GetMetadata<IEnumerable<string>>(PermissionsKey, new List<string>());
            return permissions.All(x => claims?.Contains(x) ?? false);
        }

        public static bool HasPermission(this IProvideMetadata type, string permission)
        {
            var permissions = type.GetMetadata<IEnumerable<string>>(PermissionsKey, new List<string>());
            return permissions.Any(x => string.Equals(x, permission));
        }

        /// <summary>
        /// Will check if the field or type has the passed in permission set on it or not. If not then this field won't be accessible
        /// </summary>
        /// <param name="type"></param>
        /// <param name="permission"></param>
        public static void RequirePermission(this IProvideMetadata type, string permission)
        {
            var permissions = type.GetMetadata<List<string>>(PermissionsKey);

            if (permissions == null)
            {
                permissions = new List<string>();
                type.Metadata[PermissionsKey] = permissions;
            }

            permissions.Add(permission);
        }

        public static FieldBuilder<TSourceType, TReturnType> RequirePermission<TSourceType, TReturnType>(
            this FieldBuilder<TSourceType, TReturnType> builder, string permission)
        {
            builder.FieldType.RequirePermission(permission);
            return builder;
        }
    }
}
