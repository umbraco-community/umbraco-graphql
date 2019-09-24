using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Types;
using Our.Umbraco.GraphQL.Filters;

namespace Our.Umbraco.GraphQL.Types
{
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
