# GraphQL for Umbraco

[![NuGet release](https://img.shields.io/nuget/v/Our.Umbraco.GraphQL.svg)](https://www.nuget.org/packages/Our.Umbraco.GraphQL)

## What is this

An implementation of [GraphQL](https://graphql.org) for Umbraco using [GraphQL for .NET](https://github.com/graphql-dotnet/graphql-dotnet).

Please note this **should not be used in production**, since there are **no security** and all you data will be **publicly available**.

## How does it work

An Owin middleware exposes Umbraco Published Content as a GraphQL endpoint.

GraphQL types are dynamically generated for all Umbraco document types (content and media), with all the properties as fields. They all implement an interface `PublishedContent` which implements the generic Umbraco properties as fields.

## Installation

The preferred way to install GraphQL for Umbraco is through NuGet

### Option 1: NuGet

GraphQL for Umbraco is available as a NuGet [package](https://www.nuget.org/packages/Our.Umbraco.GraphQL).

To install run the following command in the [Package Manager Console](https://docs.nuget.org/docs/start-here/using-the-package-manager-console)

```powershell
PM> Install-Package Our.Umbraco.GraphQL
```

### Option 2: From source

Clone the repository and run the Website (F5 in Visual Studio), install Umbraco with the starter kit and start exploring the API using the GraphQL Playground by opening `/umbraco/graphql`.

### Urls

| Method | Url              | Description        |
| ------ | ---------------- | ------------------ |
| GET    | /umbraco/graphql | GraphQL Playground |
| POST   | /umbraco/graphql | GraphQL endpoint   |

### Querying

The Umbraco queries/types can be found under the `umbraco` field.

```graphql
{
    umbraco {
        content {
            atRoot {
                all {
                    ...
                }
                ...
            }
            byId(id: "id") {
            }
            byType {
                ...
            }
            byUrl(url: "url") {
                ...
            }
        }
    }
}
```

### Extending the Schema with your own types

Take a look at the [Star Wars sample](samples/Website/Starwars).
