# GraphQL for Umbraco

## What is this
An experimental implementation of [GraphQL](https://graphql.org) for Umbraco using [GraphQL for .NET](https://github.com/graphql-dotnet/graphql-dotnet).

If you're interested in getting GraphQL into Umbraco Core please join the discussion on [Our](https://our.umbraco.org/forum/extending-umbraco-and-using-the-api/92236-getting-graphql-into-umbraco) and on the [issue tracker](http://issues.umbraco.org/issue/U4-11389).

Please note this **should not be used in production**, since there are **no security** and all you data will be **publicly available**.

## How does it work
An Owin middleware exposes Umbraco Published Content as a GraphQL endpoint.

GraphQL types are dynamically generated for all Umbraco document types (content and media), with all the properties as fields. They all implement an interface `PublishedContent` which implements the generic Umbraco properties as fields.



## Getting started
Clone the repository and run the Website (F5 in Visual Studio), install Umbraco with the starter kit and start exploring the API using GraphiQL by opening `/umbraco/graphiql`.

There's also a [download](https://drive.google.com/file/d/1L67kZV7u6tXy45zknLih421Rlbrx3fh3/view) which contains a prebuilt website with some sample data based on the starter kit. Login `admin@example.org`/`1234567890`. It's based on the Umbraco starter kit, where `People`, `Products` and `Blog` has been moved to the root of the tree.

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
