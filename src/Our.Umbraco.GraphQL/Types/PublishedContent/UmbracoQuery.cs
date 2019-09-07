using Our.Umbraco.GraphQL.Attributes;

namespace Our.Umbraco.GraphQL.Types.PublishedContent
{
    public class UmbracoQuery
    {
    }

    public class ExtendQueryWithUmbracoQuery
    {
        [NonNull]
        public UmbracoQuery Umbraco([Inject] UmbracoQuery query) => query;
    }
}
