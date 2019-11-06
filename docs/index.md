# Documentation

## Configuration

When installing the [NuGet package](https://www.nuget.org/packages/Our.Umbraco.GraphQL) a new file `App_Start/GraphQLComponent.cs` is added to the project, it contains the bootstrapping code for adding GraphQL to the project and the default configuration

```csharp
var path = $"/{_globalSettings.GetUmbracoMvcArea()}/graphql";

app.UseUmbracoGraphQL(path, _factory, opts =>
{
    opts.Debug = HostingEnvironment.IsDevelopmentEnvironment;
    opts.EnableMetrics = true;
    opts.EnableMiniProfiler = false;
    opts.EnablePlayground = true;
});
```

The configuration options are:

| Option                           | Description                                                                                              |
| -------------------------------- | -------------------------------------------------------------------------------------------------------- |
| CorsPolicyProvider               | The Cors Policy Provider                                                                                 |
| Debug                            | Should exceptions be exposed in the response                                                             |
| EnableMetrics                    | Should metrics be enabled                                                                                |
| EnableMiniProfiler               | Should MiniProfiler be enabled                                                                           |
| EnablePlayground                 | Should the GraphQL Playground be enabled                                                                 |
| PlaygroundSettings               | Custom settings for the [GraphQL Playground](https://github.com/prisma-labs/graphql-playground#settings) |
| SetCorsPolicy(CorsPolicy policy) | Sets the Cors Policy                                                                                     |

## Default Urls

| Method | Url              | Description        |
| ------ | ---------------- | ------------------ |
| GET    | /umbraco/graphql | GraphQL Playground |
| POST   | /umbraco/graphql | GraphQL endpoint   |

## Querying

The Umbraco queries/types can be found under the `umbraco` field.

```graphql
{
    umbraco {
        content {
            atRoot {
                all {
                }
                # ... document types are added as fields
            }
            byId(id: "id") {
            }
            byType {
                # ... document types are added as fields
            }
            byUrl(url: "url") {
            }
        }
    }
}
```

## Metrics

[Apollo Tracing](https://github.com/apollographql/apollo-tracing) is enabled by default and is displayed in the GraphQL Playground, it collects the execution time for each field.

If you need more insight, [Miniprofiler](https://miniprofiler.com/dotnet/) can be enabled by setting the option `EnableMiniprofiler=true`, MiniProfiler is implemented throughout the Umbraco code base and collects a lot of metrics. The data will be accessiblo in the `extensions.miniProfiler` field in the response.
