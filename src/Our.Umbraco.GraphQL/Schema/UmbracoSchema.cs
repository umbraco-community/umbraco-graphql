using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Conversion;
using GraphQL.Resolvers;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Types;
using Our.Umbraco.GraphQL.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Our.Umbraco.GraphQL.Schema
{
    public class UmbracoSchema : global::GraphQL.Types.Schema
    {
        // TODO: Move registration to another place
        public UmbracoSchema(
            IContentTypeService contentTypeService,
            IMemberTypeService memberTypeService,
            GraphQLServerOptions options)
        {
            if (contentTypeService == null)
            {
                throw new ArgumentNullException(nameof(contentTypeService));
            }

            FieldNameConverter = new DefaultFieldNameConverter();

            var resolveName = options.PublishedContentNameResolver;
            var documentTypes = CreateGraphTypes(contentTypeService.GetAllContentTypes(), PublishedItemType.Content, resolveName).ToList();
            var mediaTypes = CreateGraphTypes(contentTypeService.GetAllMediaTypes(), PublishedItemType.Media, resolveName);

            //foreach (var documentType in documentTypes.OfType<ComplexGraphType<IPublishedContent>>())
            //{
            //    var allowedChildren = documentType.GetMetadata<string[]>("allowedChildren");
            //    if (allowedChildren == null || allowedChildren.Length == 0) continue;

            //    var childTypes =
            //        documentTypes.FindAll(x =>
            //            allowedChildren.Contains(x.GetMetadata<string>("documentTypeAlias")));

            //    IGraphType childrenGraphType;
            //    if (childTypes.Count == 1)
            //    {
            //        childrenGraphType = childTypes[0];
            //    }
            //    else
            //    {
            //        var unionType = new UnionGraphType()
            //        {
            //            Name = $"{documentType.Name}Children",
            //        };

            //        foreach (var childType in childTypes.OfType<IObjectGraphType>())
            //        {
            //            unionType.AddPossibleType(childType);
            //        }

            //        childrenGraphType = unionType;

            //        RegisterTypes(unionType);
            //    }

            //    documentType.AddField(
            //        new FieldType
            //        {
            //            Name = "children",
            //            Description = "Children of the content.",
            //            Resolver = new FuncFieldResolver<IPublishedContent, object>(context => context.Source.Children),
            //            ResolvedType = new ListGraphType(childrenGraphType)
            //        }
            //    );
            //}

            RegisterTypes(documentTypes.ToArray());
            RegisterTypes(mediaTypes.ToArray());
            // RegisterTypes(memberTypeService.GetAll().CreateGraphTypes(PublishedItemType.Member, resolveName).ToArray());

            var query = new UmbracoQuery();



            foreach (var type in documentTypes.FindAll(x => x.GetMetadata<bool>("allowedAtRoot")))
            {
                string documentTypeAlias = type.GetMetadata<string>("documentTypeAlias");

                query.AddField(
                    new FieldType
                    {
                        Name = type.GetMetadata<string>("documentTypeAlias"),
                        ResolvedType = new ListGraphType(type),
                        Resolver = new FuncFieldResolver<object>(context =>
                        {
                            var userContext = (UmbracoGraphQLContext)context.UserContext;
                            return userContext.Umbraco.TypedContentAtXPath($"/root/{documentTypeAlias}");
                        })
                    }
                );
            }

            Query = query;
        }

        public static IEnumerable<IGraphType> CreateGraphTypes(
           IEnumerable<IContentTypeComposition> contentTypes,
           PublishedItemType publishedItemType,
           Func<IContentTypeBase, string> resolveName = null)
        {
            if (resolveName == null)
            {
                resolveName = Conventions.NameResolvers.PascalCase;
            }

            var interfaceGraphTypes = new Dictionary<string, IInterfaceGraphType>();

            //TODO: Whitelist/blacklist content types

            var contentTypeList = contentTypes.ToList();
            var compositions = contentTypeList.SelectMany(x => x.ContentTypeComposition).Distinct().ToList();

            foreach (var contentType in compositions)
            {
                var graphType = new InterfaceGraphType<IPublishedContent>
                {
                    Name = resolveName(contentType),
                    Description = contentType.Description,
                    Metadata =
                    {
                        ["documentTypeAlias"] = contentType.Alias,
                    }
                };

                // TODO: Permissions for mutations
                graphType.RequirePermission($"{contentType.Alias:can_read}");

                graphType.AddUmbracoContentPropeties(contentType, publishedItemType);

                yield return graphType;
                interfaceGraphTypes.Add(contentType.Alias, graphType);
            }

            foreach (var contentType in contentTypeList.Except(compositions))
            {
                var graphType = new ObjectGraphType<IPublishedContent>
                {
                    Name = resolveName(contentType),
                    Description = contentType.Description,
                    IsTypeOf = content => ((IPublishedContent) content).DocumentTypeAlias == contentType.Alias,
                    Metadata =
                    {
                        ["documentTypeAlias"] = contentType.Alias,
                        ["allowedAtRoot"] = contentType.AllowedAsRoot,
                        ["allowedChildren"] = contentType.AllowedContentTypes.Select(x => x.Alias).ToArray(),
                    }
                };

                // TODO: Permissions for mutations and maybe relocate where we generate the name to a single class
                graphType.RequirePermission($"{contentType.Alias:can_read}");

                graphType.Interface<PublishedContentGraphType>();
                foreach (var composition in contentType.ContentTypeComposition)
                {
                    if (interfaceGraphTypes.TryGetValue(composition.Alias, out IInterfaceGraphType interfaceType))
                    {
                        graphType.AddResolvedInterface(interfaceType);
                    }
                }

                graphType.AddUmbracoBuiltInProperties();
                graphType.AddUmbracoContentPropeties(contentType, publishedItemType);

                yield return graphType;
            }
        }
    }
}
