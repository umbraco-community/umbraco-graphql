using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Execution;
using GraphQL.Language.AST;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Filters;
using Our.Umbraco.GraphQL.ValueResolvers;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Our.Umbraco.GraphQL.Types
{
    internal static class ComplexGraphTypeOfIPublishedContentExtensions
    {
        public static ComplexGraphType<IPublishedContent> AddUmbracoBuiltInProperties(this ComplexGraphType<IPublishedContent> graphType)
        {
            // TODO: set this field name as a reserved property alias
            graphType.Field<PublishedContentDataGraphType>("_contentData", "Built in published content data.", resolve: context => context.Source);

            return graphType;
        }

        public static ComplexGraphType<IPublishedContent> AddContentDataProperties(this ComplexGraphType<IPublishedContent> graphType)
        {
            //TODO: black/whitelist properties
            graphType.Field<NonNullGraphType<DateGraphType>>("createDate", "Create date of the content.");
            graphType.Field<NonNullGraphType<StringGraphType>>("creatorName", "Name of the content creator.");
            graphType.Field<NonNullGraphType<StringGraphType>>("documentTypeAlias", "Document type alias of the content.");
            graphType.Field<NonNullGraphType<IdGraphType>>("id", "Unique id of the content.");
            graphType.Field<NonNullGraphType<IntGraphType>>("index", "Index of the content.", resolve: context => context.Source.GetIndex());
            graphType.Field<NonNullGraphType<IntGraphType>>("level", "Level of the content.");
            graphType.Field<NonNullGraphType<BooleanGraphType>>("isFirst", "Is the content first in the list.", resolve: context => context.Source.IsFirst());
            graphType.Field<NonNullGraphType<BooleanGraphType>>("isLast", "Is the content last in the list.", resolve: context => context.Source.IsLast());
            graphType.Field<NonNullGraphType<BooleanGraphType>>("isVisible", "Is the content visible.", resolve: context => context.Source.IsVisible());
            graphType.Field<NonNullGraphType<StringGraphType>>("name", "Name of the content.");
            graphType.Field<PublishedContentGraphType>("parent", "Parent of the content.");
            graphType.Field<NonNullGraphType<IntGraphType>>("sortOrder", "SortOrder of the content.");
            graphType.Field<NonNullGraphType<DateGraphType>>("updateDate", "Update date of the content.");
            graphType.Field<NonNullGraphType<StringGraphType>>("url", "Url of the content.");
            graphType.Field<NonNullGraphType<StringGraphType>>("urlAbsolute", "Absolute url of the content.", resolve: context => context.Source.UrlAbsolute());
            graphType.Field<NonNullGraphType<StringGraphType>>("writerName", "Name of the content writer.");

            graphType.FilteredConnection<PublishedContentGraphType, IPublishedContent>()
                .Name("ancestors")
                .Description("Ancestors of the content.")
                .Argument<BooleanGraphType>("includeSelf", "include self in list")
                .Bidirectional()
                .Resolve(context =>
                    (context.GetArgument<bool?>("includeSelf") == true
                        ? context.Source.AncestorsOrSelf()
                        : context.Source.Ancestors()).Filter(context).ToConnection(context)
                );

            graphType.FilteredConnection<PublishedContentGraphType, IPublishedContent>()
                .Name("siblings")
                .Description("Siblings of the content.")
                .Bidirectional()
                .Resolve(context => context.Source.Siblings().Filter(context).ToConnection(context));

            graphType.FilteredConnection<PublishedContentGraphType, IPublishedContent>()
                .Name("children")
                .Description("Children of the content.")
                .Bidirectional()
                .Resolve(context => context.Source.Children.Filter(context).ToConnection(context));

            return graphType;
        }

        public static ComplexGraphType<IPublishedContent> AddUmbracoContentPropeties(
            this ComplexGraphType<IPublishedContent> graphType,
            IContentTypeComposition contentType,
            PublishedItemType publishedItemType)
        {

            var publishedContentType = PublishedContentType.Get(publishedItemType, contentType.Alias);
            foreach (var property in contentType.CompositionPropertyTypes)
            {
                //TODO: black/whitelist properties
                if (property.PropertyEditorAlias == Constants.PropertyEditors.ListViewAlias ||
                    property.PropertyEditorAlias == Constants.PropertyEditors.FolderBrowserAlias ||
                    property.Alias.StartsWith("umbracoMember"))
                {
                    continue;
                }

                var publishedPropertyType = publishedContentType.GetPropertyType(property.Alias);

                var resolver = GraphQLValueResolversResolver.Current.FindResolver(publishedPropertyType)
                               ?? new DefaultValueResolver();

                var propertyGraphType = resolver.GetGraphQLType(publishedPropertyType);

                if (property.Mandatory)
                {
                    propertyGraphType = typeof(NonNullGraphType<>).MakeGenericType(propertyGraphType);
                }

                graphType.Field(
                    propertyGraphType,
                    property.Alias.ToCamelCase(),
                    property.Description,
                    resolve: context =>
                    {
                        var publishedProperty = context.Source.GetProperty(property.Alias);
                        return publishedProperty == null
                            ? null
                            : resolver.Resolve(publishedPropertyType, publishedProperty.Value);
                    }
                );

                // TODO: Permissions for mutations
                graphType.RequirePermission($"{publishedContentType.Alias}:{property.Alias.ToCamelCase()}:{property}can_read");
            }
            return graphType;
        }
    }
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

    internal class OrderByGraphType<T> : EnumerationGraphType where T : GraphType
    {
        public OrderByGraphType(IComplexGraphType graphType)
        {
            Type = typeof(T);
            Name = $"{Type.GraphQLName()}Order";

            foreach (var field in graphType.Fields)
            {
                var namedType = field.Type.GetNamedType();
                if (namedType == typeof(EnumerationGraphType) ||
                    namedType.IsGenericType &&
                    namedType.GetGenericTypeDefinition() == typeof(EnumerationGraphType<>) ||
                    namedType == typeof(BooleanGraphType) ||
                    namedType == typeof(IntGraphType) ||
                    namedType == typeof(FloatGraphType) ||
                    namedType == typeof(DateGraphType) ||
                    namedType == typeof(StringGraphType) ||
                    namedType == typeof(IdGraphType))
                {
                    AddValue($"{field.Name}_ASC", "", new OrderBy(field.Name, SortOrder.Ascending));
                    AddValue($"{field.Name}_DESC", "", new OrderBy(field.Name, SortOrder.Descending));
                }
            }
        }

        public Type Type { get; }

        public override string CollectTypes(TypeCollectionContext context)
        {
            var innerType = context.ResolveType(Type);
            var name = innerType.CollectTypes(context);
            context.AddType(name, innerType, context);
            return Name;
        }
    }

    public class OrderBy
    {
        public string Field { get; }
        public SortOrder Order { get; }

        public OrderBy(string field, SortOrder order)
        {
            Field = field;
            Order = order;
        }
    }

    public enum SortOrder
    {
        Ascending,
        Descending
    }

    public class FilterGraphType<T> : InputObjectGraphType where T : GraphType
    {
        public FilterGraphType(IComplexGraphType graphType)
        {
            Type = typeof(T);
            Name = $"{Type.GraphQLName()}Filter";

            AddFilterField(
                new ListGraphType(new NonNullGraphType(this)),
                graphType,
                "AND",
                $"Combines all passed `{Name}` objects with logical AND",
                subFilters => new AndFilter(subFilters)
            );

            AddFilterField(
                new ListGraphType(new NonNullGraphType(this)),
                graphType,
                "OR",
                $"Combines all passed `{Name}` objects with logical OR",
                subFilters => new OrFilter(subFilters)
            );

            foreach (var field in graphType.Fields)
            {
                if (field.GetMetadata("disableFiltering", false))
                {
                    continue;
                }

                FilterField(graphType, field);
            }

            // relation
            // _every
            // _some
            // _none
        }

        public void FilterField(IGraphType sourceType, FieldType fieldType)
        {
            var namedType = fieldType.Type.GetNamedType();
            var graphType = new Lazy<IGraphType>(() => namedType.BuildNamedType());

            if (namedType == typeof(EnumerationGraphType) ||
                namedType.IsGenericType && namedType.GetGenericTypeDefinition() == typeof(EnumerationGraphType<>) ||
                namedType == typeof(BooleanGraphType) ||
                namedType == typeof(IntGraphType) ||
                namedType == typeof(FloatGraphType) ||
                namedType == typeof(DateGraphType) ||
                namedType == typeof(StringGraphType) ||
                namedType == typeof(IdGraphType))
            {
                AddFilterField(
                    graphType.Value,
                    fieldType,
                    sourceType,
                    fieldType.Name,
                    "matches all nodes with exact value",
                    (resolver, source) => new EqFilter(resolver, source)
                );

                AddFilterField(
                    graphType.Value,
                    fieldType,
                    sourceType,
                    $"{fieldType.Name}_not",
                    "matches all nodes with different value",
                    (resolver, source) => new NotFilter(new EqFilter(resolver, source))
                );
            }

            if (namedType == typeof(EnumerationGraphType) ||
                namedType.IsGenericType && namedType.GetGenericTypeDefinition() == typeof(EnumerationGraphType<>) ||
                namedType == typeof(IntGraphType) ||
                namedType == typeof(FloatGraphType) ||
                namedType == typeof(DateGraphType) ||
                namedType == typeof(StringGraphType) ||
                namedType == typeof(IdGraphType))
            {
                AddFilterField(
                    typeof(ListGraphType<>).MakeGenericType(typeof(NonNullGraphType<>).MakeGenericType(namedType)).BuildNamedType(),
                    fieldType,
                    sourceType,
                    $"{fieldType.Name}_in",
                    "matches all nodes with value in the passed list",
                    (resolver, source) => new InFilter(resolver, source)
                );

                AddFilterField(
                    typeof(ListGraphType<>).MakeGenericType(typeof(NonNullGraphType<>).MakeGenericType(namedType)).BuildNamedType(),
                    fieldType,
                    sourceType,
                    $"{fieldType.Name}_not_in",
                    "matches all nodes with value not in the passed list",
                    (resolver, source) => new EqFilter(resolver, source)
                );
            }
            if (namedType == typeof(IntGraphType) ||
                namedType == typeof(FloatGraphType) ||
                namedType == typeof(DateGraphType) ||
                namedType == typeof(StringGraphType) ||
                namedType == typeof(IdGraphType))
            {
                AddFilterField(
                    graphType.Value,
                    fieldType,
                    sourceType,
                    $"{fieldType.Name}_lt",
                    "matches all nodes with lesser value",
                    (resolver, source) => new LtFilter(resolver, source)
                );

                AddFilterField(
                    graphType.Value,
                    fieldType,
                    sourceType,
                    $"{fieldType.Name}_lte",
                    "matches all nodes with lesser or equal value",
                    (resolver, source) => new LteFilter(resolver, source)
                );

                AddFilterField(
                    graphType.Value,
                    fieldType,
                    sourceType,
                    $"{fieldType.Name}_gt",
                    "matches all nodes with greater value",
                    (resolver, source) => new GtFilter(resolver, source)
                );

                AddFilterField(
                    graphType.Value,
                    fieldType,
                    sourceType,
                    $"{fieldType.Name}_gte",
                    "matches all nodes with greater or equal value",
                    (resolver, source) => new GteFilter(resolver, source)
                );
            }

            if (namedType == typeof(StringGraphType) || namedType == typeof(IdGraphType))
            {
                AddFilterField(
                    graphType.Value,
                    fieldType,
                    sourceType,
                    $"{fieldType.Name}_contains",
                    "matches all nodes with a value that contains given substring",
                    (resolver, source) => new ContainsFilter(resolver, source)
                );

                AddFilterField(
                    graphType.Value,
                    fieldType,
                    sourceType,
                    $"{fieldType.Name}_not_contains",
                    "matches all nodes with a value that does not contain given substring",
                    (resolver, source) => new NotFilter(new ContainsFilter(resolver, source))
                );

                AddFilterField(
                    graphType.Value,
                    fieldType,
                    sourceType,
                    $"{fieldType.Name}_starts_with",
                    "matches all nodes with a value that starts with given substring",
                    (resolver, source) => new StartsWithFilter(resolver, source)
                );

                AddFilterField(
                    graphType.Value,
                    fieldType,
                    sourceType,
                    $"{fieldType.Name}_not_starts_with",
                    "matches all nodes with a value that does not start with given substring",
                    (resolver, source) => new NotFilter(new StartsWithFilter(resolver, source))
                );

                AddFilterField(
                    graphType.Value,
                    fieldType,
                    sourceType,
                    $"{fieldType.Name}_ends_with",
                    "matches all nodes with a value that ends with given substring",
                    (resolver, source) => new EndsWithFilter(resolver, source)
                );

                AddFilterField(
                    graphType.Value,
                    fieldType,
                    sourceType,
                    $"{fieldType.Name}_not_ends_with",
                    "matches all nodes with a value that does not end with given substring",
                    (resolver, source) => new NotFilter(new EndsWithFilter(resolver, source))
                );
            }
        }

        protected void AddFilterField(
            IGraphType field,
            FieldType fieldType,
            IGraphType parentType,
            string name,
            string description,
            Func<Func<object, object>, object, IFilter> resolveFilter)
        {
            Field(field.GetType(),
                name,
                description,
                deprecationReason: fieldType.DeprecationReason,
                resolve: context =>
                {
                    object ResolveValue(object source)
                    {
                        var path = (context.Path ?? new [] { "filter" }).Concat(new[] { fieldType.Name });
                        var value = fieldType.Resolver.Resolve(
                            new ResolveFieldContext
                            {
                                FieldName = fieldType.Name,
                                FieldAst = new Field(null, new NameNode(fieldType.Name)),
                                FieldDefinition = fieldType,
                                ParentType = (IObjectGraphType)parentType,
                                Source = source,
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
                            }
                        );

                        if (value is Task<object> task)
                        {
                            return task.Result;
                        }

                        return value;
                    }

                    return resolveFilter(ResolveValue, context.Source);
                }).ResolvedType = field;
        }

        protected void AddFilterField(
            IGraphType field,
            IGraphType parentType,
            string name,
            string description,
            Func<IEnumerable<IFilter>, IFilter> resolveFilter)
        {
            Field(field.GetType(),
                name,
                description,
                resolve: context =>
                {
                    var value = (IEnumerable<object>)context.Source;
                    var subFilters = new List<IFilter>();
                    foreach (var obj in value)
                    {
                        var dict = (IDictionary<string, object>)obj;
                        foreach (KeyValuePair<string, object> pair in dict)
                        {
                            subFilters.Add(this.ResolveFilter(pair, context));
                        }
                    }
                    return resolveFilter(subFilters);
                }).ResolvedType = field;
        }

        public Type Type { get; }

        public override string CollectTypes(TypeCollectionContext context)
        {
            var innerType = context.ResolveType(Type);
            var name = innerType.CollectTypes(context);
            context.AddType(name, innerType, context);
            return Name;
        }
    }
}
