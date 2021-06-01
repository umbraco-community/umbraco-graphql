using Examine;
using Examine.Lucene.Providers;
using Examine.Search;
using GraphQL;
using GraphQL.Types;
using System;
using System.Linq;
using System.Reflection;
using Umbraco.Cms.Core.PublishedCache;

namespace Our.Umbraco.GraphQL.Adapters.Examine.Types
{
    public class ExamineSearcherGraphType : ObjectGraphType<ExamineSearcherQuery>
    {
        private static readonly System.Reflection.FieldInfo _resultField = typeof(LuceneSearcher).GetField("_reader", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo _validateSearcherMethod = typeof(LuceneSearcher).GetMethod("ValidateSearcher", BindingFlags.NonPublic | BindingFlags.Instance);
        private readonly ISearcher _searcher;

        public ExamineSearcherGraphType(IPublishedSnapshotAccessor snapshotAccessor, ISearcher searcher, string searcherSafeName)
        {
            Name = $"{searcherSafeName}Searcher";
            var fields = searcher is BaseLuceneSearcher bls ? bls.GetSearchContext().SearchableFields : null;

            Field<SearchResultsInterfaceGraphType>()
                .Name("query")
                .Description("Queries the Examine searcher using a raw Lucene syntax query")
                .Argument<StringGraphType>("query", "The raw Lucene query to execute in this searcher")
                .Argument<StringGraphType, string>("category", "The category of data to include in the results.  For Umbraco content indexes, this is the content type alias", null)
                .Argument<BooleanOperationGraphType, BooleanOperation>("defaultOperation", "The default operation to use when searching", BooleanOperation.And)
                .Argument<IntGraphType, int>("skip", "The number of results to skip over", 0)
                .Argument<IntGraphType, int>("take", "The maximum number of results to return", 500)
                .Argument<StringGraphType>("sortFields", "A comma-separated list of field names to sort by.  If you need to specify a sort field type, do so with a pipe and then the type, i.e. fieldName|bool")
                .Argument<SortDirectionGraphType, SortDirection>("sortDir", "The direction for Examine to sort", SortDirection.ASC)
                .Resolve(GetQueryResults);
            GetField("query").ResolvedType = new SearchResultsGraphType(snapshotAccessor, $"{searcherSafeName}Query", fields);

            Field<SearchResultsInterfaceGraphType>()
                .Name("search")
                .Description("Queries the Examine searcher using the natural language Search method")
                .Argument<StringGraphType>("query", "The text to search for")
                .Argument<IntGraphType, int>("skip", "The number of results to skip over", 0)
                .Argument<IntGraphType, int>("maxResults", "The maximum number of results to return", 500)
                .Resolve(GetSearchResults);
            GetField("search").ResolvedType = new SearchResultsGraphType(snapshotAccessor, $"{searcherSafeName}Search", fields);

            _searcher = searcher;
        }

        private ISearchResults GetQueryResults(IResolveFieldContext<ExamineSearcherQuery> ctx)
        {
            var query = _searcher.CreateQuery(ctx.GetArgument<string>("category"), ctx.GetArgument<bool>("defaultAnd") ? BooleanOperation.And : BooleanOperation.Or)
                .NativeQuery(ctx.GetArgument<string>("query")) as IOrdering;

            var sortFields = (ctx.GetArgument<string>("sortFields") ?? "").Split(',').Select(f => f.Trim()).Where(f => f.Length > 0).ToList();
            if (sortFields.Count > 0)
            {
                var sortableFields = sortFields.Select(s =>
                {
                    var pieces = s.Split('|');
                    if (pieces.Length == 1 || !Enum.TryParse<SortType>(pieces[1], true, out var type)) return new SortableField(pieces[0]);
                    return new SortableField(pieces[0], type);
                }).ToArray();

                if (ctx.GetArgument<SortDirection>("sortDir") == SortDirection.ASC) query = query.OrderBy(sortableFields);
                else query = query.OrderByDescending(sortableFields);
            }

            var results = query.Execute(new QueryOptions(ctx.GetArgument<int>("skip"), ctx.GetArgument<int>("take")));
            return results;
        }

        private ISearchResults GetSearchResults(IResolveFieldContext<ExamineSearcherQuery> ctx)
        {
            var results = _searcher.Search(ctx.GetArgument<string>("query"), new QueryOptions(ctx.GetArgument<int>("skip"), ctx.GetArgument<int>("take")));
            return results;
        }
    }

    public class BooleanOperationGraphType : EnumerationGraphType<BooleanOperation> { }

    public class SortDirectionGraphType : EnumerationGraphType<SortDirection> { }
    public enum SortDirection
    {
        ASC,
        DESC
    }
}
