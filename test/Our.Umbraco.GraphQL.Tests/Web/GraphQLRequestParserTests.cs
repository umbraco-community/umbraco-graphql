using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Owin;
using Newtonsoft.Json;
using Our.Umbraco.GraphQL.Web;
using Xunit;

namespace Our.Umbraco.GraphQL.Tests.Web
{
    public class GraphQLRequestParserTests
    {
        private GraphQLRequestParser createSUT() => new GraphQLRequestParser(new JsonSerializer());

        [Fact]
        public async Task Parse_WithJsonObject_ReturnsParsedRequest()
        {
            var parser = createSUT();

            using (var stream = new MemoryStream())
            {
                await WriteToStream(stream,
                    "{ \"query\": \"{ hero(name: heroName) { age } }\", \"variables\": { \"name\": \"myHero\" } }");

                var request = await parser.Parse(new OwinRequest()
                {
                    ContentType = "application/json",
                    Body = stream,
                });

                request.Should().NotBeNull()
                    .And.Contain(x => x.Query == "{ hero(name: heroName) { age } }")
                    .Which.Variables.Should().ContainKey("name")
                    .WhichValue.Should().Be("myHero");
            }
        }

        [Fact]
        public async Task Parse_WithJsonObject_SetsOperationName()
        {
            var parser = createSUT();

            using (var stream = new MemoryStream())
            {
                await WriteToStream(stream,
                    "{ \"query\": \"{ hero { age } }\", \"operationName\" : \"myOperation\" }");

                var request = await parser.Parse(new OwinRequest()
                {
                    ContentType = "application/json",
                    Body = stream,
                });

                request.Should().NotBeNull()
                    .And.Contain(x => x.Query == "{ hero { age } }")
                    .Which.OperationName.Should().Be("myOperation");
            }
        }

        [Fact]
        public async Task Parse_WithJsonObject_IsBatchedIsFalse()
        {
            var parser = createSUT();

            using (var stream = new MemoryStream())
            {
                await WriteToStream(stream, "{ \"query\": \"{ hero { name } }\" }");

                var request = await parser.Parse(new OwinRequest()
                {
                    ContentType = "application/json",
                    Body = stream,
                });

                request.IsBatched.Should().BeFalse();
            }
        }

        [Fact]
        public async Task Parse_WithJsonArray_ReturnsParsedRequest()
        {
            var parser = createSUT();

            using (var stream = new MemoryStream())
            {
                await WriteToStream(stream,
                    "[{ \"query\": \"{ hero(name: heroName) { age } }\", \"variables\": { \"name\": \"myHero\" } }]");

                var request = await parser.Parse(new OwinRequest()
                {
                    ContentType = "application/json",
                    Body = stream,
                });

                request.Should().NotBeNull()
                    .And.Contain(x => x.Query == "{ hero(name: heroName) { age } }")
                    .Which.Variables.Should().ContainKey("name")
                    .WhichValue.Should().Be("myHero");
            }
        }

        [Fact]
        public async Task Parse_WithJsonArray_IsBatchedIsTrue()
        {
            var parser = createSUT();

            using (var stream = new MemoryStream())
            {
                await WriteToStream(stream, "[{ \"query\": \"{ hero { name } }\" }]");

                var request = await parser.Parse(new OwinRequest()
                {
                    ContentType = "application/json",
                    Body = stream,
                });

                request.IsBatched.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Parse_WithApplicationJsonContentTypeButDataIsNotJson_ReturnsNull()
        {
            var parser = createSUT();

            using (var stream = new MemoryStream())
            {
                var request = await parser.Parse(new OwinRequest()
                {
                    ContentType = "application/json",
                    Body = stream,
                });

                request.Should().BeNull();
            }
        }

        [Fact]
        public async Task Parse_WithApplicationGraphQLContentType_ReturnsParsedRequest()
        {
            var parser = createSUT();

            using (var stream = new MemoryStream())
            {
                await WriteToStream(stream, "{ hero(name: heroName) { age } }");

                var request = await parser.Parse(new OwinRequest()
                {
                    ContentType = "application/graphql",
                    Body = stream,
                });

                request.Should().NotBeNull()
                    .And.Contain(x => x.Query == "{ hero(name: heroName) { age } }");
            }
        }

        [Fact]
        public async Task Parse_WithApplicationGraphQLContentType_IsBatchedIsFalse()
        {
            var parser = createSUT();

            using (var stream = new MemoryStream())
            {
                await WriteToStream(stream, "{ hero { name } }");

                var request = await parser.Parse(new OwinRequest()
                {
                    ContentType = "application/graphql",
                    Body = stream,
                });

                request.IsBatched.Should().BeFalse();
            }
        }

        [Fact]
        public async Task Parse_UnSupportedContentType_ReturnsNull()
        {
            var parser = createSUT();

            using (var stream = new MemoryStream())
            {
                var request = await parser.Parse(new OwinRequest()
                {
                    ContentType = "text/plain",
                    Body = stream,
                });

                request.Should().BeNull();
            }
        }

        private async Task WriteToStream(Stream stream, string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            await stream.WriteAsync(bytes, 0, bytes.Length);
            stream.Position = 0;
        }
    }
}
