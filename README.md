# GraphQL for Umbraco
[![NuGet release](https://img.shields.io/nuget/v/Our.Umbraco.GraphQL.svg)](https://www.nuget.org/packages/Our.Umbraco.GraphQL)

## What is this
An experimental implementation of [GraphQL](https://graphql.org) for Umbraco using [GraphQL for .NET](https://github.com/graphql-dotnet/graphql-dotnet).

If you're interested in getting GraphQL into Umbraco Core please join the discussion on [Our](https://our.umbraco.org/forum/extending-umbraco-and-using-the-api/92236-getting-graphql-into-umbraco) and on the [issue tracker](http://issues.umbraco.org/issue/U4-11389).

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
Clone the repository and run the Website (F5 in Visual Studio), install Umbraco with the starter kit and start exploring the API using GraphiQL by opening `/umbraco/graphiql`.

### Urls
| Url | Description |
| --- | ----------- |
| /umbraco/graphiql | GraphiQL interface |
| /umbraco/graphql | GraphQL endpoint |
| /umbraco/graphql/schema | The generated schema |

### Querying
Query examples based on The Starter Kit
```graphql
{
  content {
    byType {
      People(id: "1116") {
        pageTitle
        _contentData {
          children {
            items {
              ... on Person {
                _contentData {
                  name
                }
                department
                photo {
                  _contentData {
                    url
                  }
                }
              }
            }
          }
        }
      }
    }
  }
}

```

We can also do some simple filtering and sorting, ([Inspired by the Grahpcool filtering](https://www.graph.cool/docs/reference/graphql-api/query-api-nia9nushae#query-arguments)) like geting all children of people that starts with the letter `J`
```graphql
{
  content {
    byType {
      People(id: "1116") {
        pageTitle
        _contentData {
          peopleStartsWithJ: children(filter: {name_starts_with: "J"}, orderBy: name_ASC) {
            items {
              ... on Person {
                _contentData {
                  name
                }
                department
                photo {
                  _contentData {
                    url
                  }
                }
              }
            }
          }
        }
      }
    }
  }
}

```

And even query for multiple types at the same time
```graphql
{
  content {
    byType {
      People(id: "1116") {
        pageTitle
        _contentData {
          peopleStartsWithJ: children(filter: {name_starts_with: "J"}, orderBy: name_ASC) {
            items {
              ...SimplePerson
            }
          }
        }
      }
      Products(id: "1107") {
        pageTitle
        defaultCurrency
        featuredProducts {
          ...SimpleProduct
        }
        _contentData {
          children {
            items {
              ...SimpleProduct
            }
          }
        }
      }
    }
  }
}

fragment SimplePerson on Person {
  _contentData {
    name
  }
  department
  photo {
    _contentData {
      url
    }
  }
}

fragment SimpleProduct on Product {
  _contentData {
    name
  }
  price
  sku
  photos {
    _contentData {
      url
    }
  }
}
```
