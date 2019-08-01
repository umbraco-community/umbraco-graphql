using GraphQL;
using GraphQL.Types;
using System.Collections.Generic;

namespace Our.Umbraco.GraphQL.Types
{
    public class PublishedContentByTypeQueryGraphType : ObjectGraphType
    {
        public PublishedContentByTypeQueryGraphType(IEnumerable<IGraphType> documentGraphTypes)
        {
            Name = "PublishedContentByTypeQuery";

            foreach (var type in documentGraphTypes)
            {
                string contentTypeAlias = type.GetMetadata<string>(Constants.Metadata.ContentTypeAlias);

                Field<PublishedContentInterfaceGraphType>()
                    .Name(contentTypeAlias.ToPascalCase())
                    .Argument<NonNullGraphType<IdGraphType>>("id", "The unique content id")
                    .Type(type)
                    .Resolve(context =>
                    {
                        var userContext = (UmbracoGraphQLContext)context.UserContext;
                        var id = context.GetArgument<int>("id");
                        var result = userContext.UmbracoContext.Content.GetById(id);
                        if(result != null && result.ContentType.Alias != contentTypeAlias)
                        {
                            var error = new ExecutionError($"Found content of type '{result.ContentType.Alias.ToPascalCase()}', but expected it to be '{contentTypeAlias.ToPascalCase()}'.");
                            error.AddLocation(context.FieldAst, context.Document);
                            context.Errors.Add(error);
                            return null;
                        }

                        return result;
                    });
            }
        }
    }
}
