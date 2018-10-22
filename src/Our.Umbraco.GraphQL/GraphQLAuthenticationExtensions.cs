using GraphQL;
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
        public static readonly string PermissionsKey = "permissions";

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

        public static bool HasPermission(this IProvideMetadata type, string docTypeAlias, IEnumerable<string> claims)
        {
            var permissionsData = type.GetMetadata<IEnumerable<string>>(PermissionsKey, new List<string>());
            var permissions = new List<string>();

            foreach (var permissionKey in permissionsData)
            {
                var permission = permissionKey;

                // if it starts with : we don't know what doctype it belongs to on the point of creating the field metadat
                // So we work it out when required and pass it in here
                if (permissionKey.StartsWith(":"))
                {
                    permission = $"{docTypeAlias}{permissionKey}";
                }

                permissions.Add(permission);
            }
            
            return permissions.All(x => claims?.Contains(x) ?? false);
        }

        public static bool HasPermission(this IProvideMetadata type, IEnumerable<string> claims)
        {
            var permissions = type.GetMetadata<IEnumerable<string>>(PermissionsKey, new List<string>());
            return permissions.All(x => claims?.Contains(x) ?? false);
        }

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

        public static void SetPermissions(this FieldType type, string documentTypeAlias, bool isBuiltInProperty = false)
        {
            var propertyAlias = type.Name;

            // If its a built in Umbraco property, add an additional flag to the key so it doesn't clash with any custom properties we set
            var readPermissionKey = isBuiltInProperty ?
                $"{documentTypeAlias}:builtInProperty:{propertyAlias}:Read" : $"{documentTypeAlias}:{propertyAlias}:Read";

            type.RequirePermission(readPermissionKey);
        }

        public static void SetPermissions(this FieldType type, GraphType graphType, bool isBuiltInProperty = false)
        {
            // The graph type should have the doc type alias set in the meta data so we're accessing it from that
            var doctypeAlias = graphType.GetMetadata<string>(Constants.Metadata.ContentTypeAlias);
            type.SetPermissions(doctypeAlias, isBuiltInProperty);
        }

        public static void SetDoctypeMetadata(this FieldType type, string doctypeAlias)
        {
            var currentAlias = type.GetMetadata<List<string>>(Constants.Metadata.ContentTypeAlias);

            if (currentAlias == null)
            {
                type.Metadata[Constants.Metadata.ContentTypeAlias] = doctypeAlias;
            }
        }

        public static FieldBuilder<TSourceType, TReturnType> SetDoctypeMetadata<TSourceType, TReturnType>(
            this FieldBuilder<TSourceType, TReturnType> builder, string doctypeAlias)
        {
            SetDoctypeMetadata(builder.FieldType, doctypeAlias);
            return builder;
        }

        public static FieldBuilder<TSourceType, TReturnType> RequirePermission<TSourceType, TReturnType>(
            this FieldBuilder<TSourceType, TReturnType> builder, string permission)
        {
            builder.FieldType.RequirePermission(permission);
            return builder;
        }
    }
}
