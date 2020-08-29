using Examine;
using GraphQL.Types;
using System.Collections.Generic;

namespace Our.Umbraco.GraphQL.Adapters.Examine.Types
{
    public class SearchResultsGraphType : ObjectGraphType<ISearchResults>
    {
        public SearchResultsGraphType(string searcherSafeName, IEnumerable<string> fields)
        {
            Name = $"{searcherSafeName}SearchResults";

            Interface<SearchResultsInterfaceGraphType>();

            this.AddBuiltInFields(searcherSafeName, fields);
        }
    }
}
