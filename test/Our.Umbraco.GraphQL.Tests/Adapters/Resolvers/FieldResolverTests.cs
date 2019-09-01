using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using GraphQL;
using GraphQL.Types;
using NSubstitute;
using Our.Umbraco.GraphQL.Adapters.Resolvers;
using Our.Umbraco.GraphQL.Attributes;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Adapters.Resolvers
{
    public class FieldResolverTests
    {
        private FieldResolver CreateSUT(MemberInfo memberInfo, IDependencyResolver dependencyResolver = null)
        {
            return new FieldResolver(memberInfo, dependencyResolver ?? new DefaultDependencyResolver());
        }

        [Fact]
        public void Resolve_FieldDefinitionWithFieldInfo_ReturnsFieldValue()
        {
            var fieldInfo = typeof(Query).GetField(nameof(Query.Answer));
            var resolver = CreateSUT(fieldInfo);

            var result = resolver.Resolve(new ResolveFieldContext
            {
                Source = new Query()
            });

            result.Should().Be(42);
        }

        [Fact]
        public void Resolve_FieldDefinitionWithMethodInfo_ReturnsMethodValue()
        {
            var methodInfo = typeof(Query).GetMethod(nameof(Query.Ping));
            var resolver = CreateSUT(methodInfo);

            var result = resolver.Resolve(new ResolveFieldContext
            {
                Source = new Query()
            });

            result.Should().Be("Pong");
        }

        [Fact]
        public void Resolve_FieldDefinitionWithPropertyInfo_ReturnsPropertyValue()
        {
            var propertyInfo = typeof(Query).GetProperty(nameof(Query.Hello));
            var resolver = CreateSUT(propertyInfo);

            var result = resolver.Resolve(new ResolveFieldContext
            {
                Source = new Query()
            });

            result.Should().Be("World!");
        }

        [Fact]
        public void Resolve_FieldDefinitionWithMethodInfoWithArguments_ReturnsMethodValue()
        {
            var methodInfo = typeof(Query).GetMethod(nameof(Query.SayHello));
            var resolver = CreateSUT(methodInfo);

            var result = resolver.Resolve(new ResolveFieldContext
            {
                Source = new Query(),
                Arguments = new Dictionary<string, object>
                {
                    {"name", "World!"}
                }
            });

            result.Should().Be("Hello World!");
        }

        [Fact]
        public void Resolve_FieldDefinitionWithMethodInfoWithInjectedArgument_ResolvesInjectedFromDependencyResolver()
        {
            var dependencyResolver = Substitute.For<IDependencyResolver>();
            dependencyResolver.Resolve(Arg.Is(typeof(Injected)))
                .Returns(new Injected());
            var methodInfo = typeof(Query).GetMethod(nameof(Query.GetInjected));
            var resolver = CreateSUT(methodInfo, dependencyResolver);

            var result = resolver.Resolve(new ResolveFieldContext
            {
                Source = new Query()
            });

            result.Should().Be("Injected Pong");
            dependencyResolver.Received()
                .Resolve(typeof(Injected));
        }

        [Fact]
        public void Resolve_SourceIsNull_ResolvesSourceFromDependencyResolver()
        {
            var dependencyResolver = Substitute.For<IDependencyResolver>();
            dependencyResolver.Resolve(Arg.Is(typeof(Query)))
                .Returns(new Query
                {
                    Answer = 17,
                });
            var methodInfo = typeof(Query).GetField(nameof(Query.Answer));
            var resolver = CreateSUT(methodInfo, dependencyResolver);

            var result = resolver.Resolve(new ResolveFieldContext());

            result.Should().Be(17);
            dependencyResolver.Received()
                .Resolve(typeof(Query));
        }

        private class Query
        {
            public int Answer = 42;
            public string Ping() => "Pong";
            public string Hello => "World!";
            public string SayHello(string name) => $"Hello {name}";
            public string GetInjected([Inject]Injected injected) => injected.Ping;
        }

        private class Injected
        {
            public string Ping => "Injected Pong";
        }
    }
}
