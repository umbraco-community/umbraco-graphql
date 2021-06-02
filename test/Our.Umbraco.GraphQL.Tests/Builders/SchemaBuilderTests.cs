using System;
using System.Reflection;
using FluentAssertions;
using GraphQL;
using GraphQL.Types;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Our.Umbraco.GraphQL.Adapters;
using Our.Umbraco.GraphQL.Adapters.Visitors;
using Our.Umbraco.GraphQL.Builders;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Builders
{
    public class SchemaBuilderTests
    {
        private SchemaBuilder CreateSUT(IGraphTypeAdapter graphTypeAdapter = null, GraphVisitor visitor = null)
        {
            if (graphTypeAdapter == null)
            {
                var queryObjectGraphType = new ObjectGraphType();
                graphTypeAdapter = Substitute.For<IGraphTypeAdapter>();
                graphTypeAdapter.Adapt(Arg.Is(typeof(Query).GetTypeInfo())).Returns(queryObjectGraphType);
            }

            return new SchemaBuilderWithQuery(graphTypeAdapter, new FuncServiceProvider(Activator.CreateInstance), visitor);
        }

        [Fact]
        public void Build_SchemaWithQueryProperty_ReturnsSchemaWithQuery()
        {
            var graphTypeAdapter = Substitute.For<IGraphTypeAdapter>();
            var queryObjectGraphType = new ObjectGraphType();
            graphTypeAdapter.Adapt(Arg.Is(typeof(Query).GetTypeInfo())).Returns(queryObjectGraphType);
            var schemaBuilder = CreateSUT(graphTypeAdapter);

            var schema = schemaBuilder.Build();

            schema.Query.Should().Be(queryObjectGraphType);
        }

        [Fact]
        public void Build_WithVisitor_CallsVisitWithSchema()
        {
            var visitor = Substitute.For<GraphVisitor>();
            var schemaBuilder = CreateSUT(visitor: visitor);

            var schema = schemaBuilder.Build();

            visitor.Received(1).Visit(Arg.Is(schema));
        }

        private class EmptySchema
        {
        }

        private class SchemaWithSetOnlyQuery
        {
            private Query _query;
            public Query Query
            {
                set => _query = value;
            }
        }

        private class SchemaBuilderWithQuery : SchemaBuilder
        {
            public SchemaBuilderWithQuery(IGraphTypeAdapter graphTypeAdapter, IServiceProvider serviceProvider, IGraphVisitor visitor) : base(graphTypeAdapter, serviceProvider, visitor)
            {
            }

            public override Type SchemaType => typeof(SchemaWithQuery);
        }

        private class SchemaWithQuery
        {
            public Query Query { get; set; }
        }

        private class Query
        {
        }

        private class MyType
        {
            public string Name { get; set; }
        }
    }
}
