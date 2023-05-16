# GraphQL for Umbraco

[![NuGet release](https://img.shields.io/nuget/v/Our.Umbraco.GraphQL.svg)](https://www.nuget.org/packages/Our.Umbraco.GraphQL)

> **NOTE**
> This branch is for the version of this plugin that supports Umbraco version **10**.

For other versions, check out:

-   [v7](https://github.com/umbraco-community/umbraco-graphql/blob/v7/dev/README.md)
-   [v8](https://github.com/umbraco-community/umbraco-graphql/blob/v8/dev/README.md)
-   [v9](https://github.com/umbraco-community/umbraco-graphql/blob/v9/dev/README.md)
-   v10 - THIS BRANCH
-   [develop](https://github.com/umbraco-community/umbraco-graphql/blob/develop/README.md)

## What is this

An implementation of [GraphQL](https://graphql.org) for Umbraco using [GraphQL for .NET](https://github.com/graphql-dotnet/graphql-dotnet).

Please note this **should not be used in production**, since there are **no security** and all you data will be **publicly available**.

## How does it work

GraphQL types are dynamically generated for all Umbraco document types (content and media), with all the properties as fields.

## Installation

The preferred way to install GraphQL for Umbraco is through NuGet

### Option 1: NuGet

GraphQL for Umbraco is available as a [NuGet package](https://www.nuget.org/packages/Our.Umbraco.GraphQL).

To install run the following command in the [Package Manager Console](https://docs.nuget.org/docs/start-here/using-the-package-manager-console)

```powershell
PM> Install-Package Our.Umbraco.GraphQL
```

### Option 2: From source

Clone the repository and run the Website (F5 in Visual Studio), install Umbraco with the starter kit and start exploring the API using the GraphQL Playground by opening `/umbraco/graphql`.

## Docs

The docs can be found [here](docs/index.md)

## TODO

-   [x] GraphQL Playground
-   [x] Schema Stitching (extending types)
-   [x] Metrics
-   [x] Published Content
-   [ ] Published Media
-   [ ] Dictionary
-   [ ] Statistics (field usage etc.)
-   [ ] Deprecation (Content Types and Properties)
-   [ ] API Tokens (OAUTH) with permissions (for content types and properties)
-   [ ] Data Types
-   [ ] Document Types
-   [ ] Media Types
-   [ ] Member Types
-   [ ] Content
-   [ ] Media
-   [ ] Members
-   [ ] Documentation

## Contributing

Anyone can help make this project better - check out our [Contributing guide](CONTRIBUTING.md)

## Authors

-   [Rasmus John Pedersen](https://www.github.com/rasmusjp)

## License

Copyright © 2018 Rasmus John Pedersen

GraphQL for Umbraco is released under the [MIT License](LICENSE)
