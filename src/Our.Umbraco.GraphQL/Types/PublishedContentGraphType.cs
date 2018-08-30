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

    public class BuildInContentDataGraphType : ObjectGraphType<IPublishedContent>
    {
        public BuildInContentDataGraphType()
        {
            Name = "ContentData";

            this.AddContentDataProperties();
        }
    }
}
