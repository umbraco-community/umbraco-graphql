using GraphQL.Language.AST;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Filters;
using Our.Umbraco.GraphQL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Our.Umbraco.GraphQL
{
    internal static class FilterUtils
    {
        public static IEnumerable<TSource> Filter<TSource, TParent>(
            this IEnumerable<TSource> source,
            ResolveFieldContext<TParent> context)
        {
            var orderByArg = context.GetArgument<IEnumerable<object>>("orderBy")?.OfType<OrderBy>();

            if (context.Arguments.TryGetValue("filter", out object filterArg) && filterArg != null)
            {
                var rootFilter = ((IDictionary<string, object>)filterArg).First();
                var filterType = (IComplexGraphType)context.FieldDefinition.Arguments.Find("filter").ResolvedType;
                var filter = ResolveFilter(filterType, rootFilter, context);

                source = source.Where(x => filter.IsSatisfiedBy(x));
            }
            if (orderByArg != null)
            {
                foreach (var order in orderByArg)
                {
                    FieldType fieldType = context.ParentType.Fields.Single(x => x.Name == order.Field);

                    object KeySelector(TSource item)
                    {
                        var path = (context.Path ?? new[] { "order" }).Concat(new[] { fieldType.Name });
                        var value = fieldType.Resolver.Resolve(new ResolveFieldContext
                        {
                            FieldName = order.Field,
                            FieldAst = new Field(null, new NameNode(order.Field)),
                            FieldDefinition = fieldType,
                            ParentType = context.ParentType,
                            Source = item,
                            Schema = context.Schema,
                            Document = context.Document,
                            Fragments = context.Fragments,
                            RootValue = context.RootValue,
                            UserContext = context.UserContext,
                            Operation = context.Operation,
                            Variables = context.Variables,
                            CancellationToken = context.CancellationToken,
                            Metrics = context.Metrics,
                            Errors = context.Errors,
                            Path = path
                        });
                        if (value is Task<object> task)
                        {
                            return task.Result;
                        }
                        return value;
                    }

                    if (source is IOrderedEnumerable<TSource> ordered)
                    {
                        source = order.Order == SortOrder.Ascending
                            ? ordered.ThenBy(KeySelector)
                            : ordered.ThenByDescending(KeySelector);
                    }
                    else
                    {
                        source = order.Order == SortOrder.Ascending
                            ? source.OrderBy(KeySelector)
                            : source.OrderByDescending(KeySelector);
                    }
                }

            }
            return source;
        }

        internal static IFilter ResolveFilter<TParentType>(
            this IComplexGraphType filterType,
            KeyValuePair<string, object> value,
            ResolveFieldContext<TParentType> context)
        {
            var path = (context.Path ?? new[] { "filter" }).Concat(new[] { value.Key });
            var field = filterType.Fields.First(x => x.Name == value.Key);

            return (IFilter)field.Resolver.Resolve(
                new ResolveFieldContext
                {
                    FieldName = field.Name,
                    FieldAst = new Field(null, new NameNode(field.Name)),
                    FieldDefinition = field,
                    ParentType = context.ParentType,
                    Source = value.Value,
                    Schema = context.Schema,
                    Document = context.Document,
                    Fragments = context.Fragments,
                    RootValue = context.RootValue,
                    UserContext = context.UserContext,
                    Operation = context.Operation,
                    Variables = context.Variables,
                    CancellationToken = context.CancellationToken,
                    Metrics = context.Metrics,
                    Errors = context.Errors,
                    Path = path,
                }
            );
        }
    }
}
