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

            return new SchemaBuilder(graphTypeAdapter, new FuncServiceProvider(Activator.CreateInstance), visitor);
        }

        [Fact]
        public void Build_WithNull_ThrowsException()
        {
            var schemaBuilder = CreateSUT();

            Action action = () => schemaBuilder.Build(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Build_SchemaWithoutQueryProperty_ThrowsException()
        {
            var schemaBuilder = CreateSUT();

            Action action = () => schemaBuilder.Build(typeof(EmptySchema).GetTypeInfo());

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Build_SchemaWithSetOnlyQueryProperty_ThrowsException()
        {
            var schemaBuilder = CreateSUT();

            Action action = () => schemaBuilder.Build<SchemaWithSetOnlyQuery>();

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Build_SchemaWithQueryProperty_ReturnsSchemaWithQuery()
        {
            var graphTypeAdapter = Substitute.For<IGraphTypeAdapter>();
            var queryObjectGraphType = new ObjectGraphType();
            graphTypeAdapter.Adapt(Arg.Is(typeof(Query).GetTypeInfo())).Returns(queryObjectGraphType);
            var schemaBuilder = CreateSUT(graphTypeAdapter);

            var schema = schemaBuilder.Build<SchemaWithQuery>();

            schema.Query.Should().Be(queryObjectGraphType);
        }

        [Fact]
        public void Build_WithVisitor_CallsVisitWithSchema()
        {
            var visitor = Substitute.For<GraphVisitor>();
            var schemaBuilder = CreateSUT(visitor: visitor);

            var schema = schemaBuilder.Build<SchemaWithQuery>();

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
