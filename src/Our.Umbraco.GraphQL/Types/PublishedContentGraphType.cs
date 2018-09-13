using GraphQL.Types;
using Umbraco.Core.Models;

namespace Our.Umbraco.GraphQL.Types
{
    public class PublishedContentGraphType : InterfaceGraphType<IPublishedContent>
    {
        public PublishedContentGraphType()
        {
            Name = "PublishedContent";

            this.AddUmbracoBuiltInProperties();
        }
    }

    public class PublishedContentDataGraphType : ObjectGraphType<IPublishedContent>
    {
        public PublishedContentDataGraphType()
        {
            Name = "PublishedContentData";

            this.AddContentDataProperties();
        }
    }
}
