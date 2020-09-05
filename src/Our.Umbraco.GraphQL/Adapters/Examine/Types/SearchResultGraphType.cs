using Examine;
using GraphQL.Types;
using System.Collections.Generic;

namespace Our.Umbraco.GraphQL.Adapters.Examine.Types
{
    public class SearchResultGraphType : ObjectGraphType<ISearchResult>
    {
        public SearchResultGraphType(string searcherSafeName, IEnumerable<string> fields)
        {
            Name = $"{searcherSafeName}Result";

            Interface<SearchResultInterfaceGraphType>();

            this.AddBuiltInFields(fields == null);

            if (fields != null)
            {
                foreach (var field in fields)
                    Field<StringGraphType>().Name(field.SafeName()).Resolve(ctx => ctx.Source.AllValues.TryGetValue(field, out var list) ? string.Join(", ", list) : null);
            }
        }
    }
}
