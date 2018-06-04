# Umbraco GraphQL

## What is this
An experimental implementation of [GraphQL](https://graphql.org) for Umbraco using [GraphQL for .NET](https://github.com/graphql-dotnet/graphql-dotnet).

Please note this **should not be used in production**, since there a **no security** and all you data will be **publicly available**.

## How does it work
An Owin middleware exposes Umbraco Published Content as a GraphQL endpoint.

GraphQL types are dynamically generated for all Umbraco document types (content and media), with all the properties as fields. They all implement an interface `PublishedContent` which implements the generic Umbraco properties as fields.

If a document type is alloweded at root, a field on the query is generated with the same name as the document type alias.

There are also two generic fields `content(id: ID!)` and `contentAtRoot` which can be used to query by `id` or getting all root content.

## Getting started
Clone the repository and run the Website (F5 in Visual Studio), install Umbraco with the starter kit and start exploring the API using GraphiQL by opening `/umbraco/graphiql`.

There's also a [downloaded](https://drive.google.com/file/d/1L67kZV7u6tXy45zknLih421Rlbrx3fh3/view) which contains a prebuilt website with some sample data based on the starter kit. Login `admin@example.org`/`1234567890`. It's based on the Umbraco starter kit, where `People`, `Products` and `Blog` has been moved to the root of the tree.

### Urls
| Url | Description |
| --- | ----------- |
| /umbraco/graphiql | GraphiQL interface |
| /umbraco/graphql | GraphQL endpoint |
| /umbraco/graphql/schema | The generated schema |

### Querying
Query examples based on the download above

```graphql
{
  people {
    pageTitle
    children {
      items {
        ... on Person {
          name
          department
          photo {
            url
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
  people {
    pageTitle
    peopleStartsWithJ:children(filter: { name_starts_with:"J"}, orderBy: name_ASC) {
      items {
        ... on Person {
          name
          department
          photo {
            url
          }
        }
      }
    }
  }
}
```

And even query for multiple roots at the same time
```graphql
{
  people {
    pageTitle
    peopleStartsWithJ: children(filter: {name_starts_with: "J"}, orderBy: name_ASC) {
      items {
        ...SimplePerson
      }
    }
  }
  products {
    pageTitle
    defaultCurrency
    featuredProducts {
      ...SimpleProduct
    }
    children {
      items {
        ...SimpleProduct
      }
    }
  }
}

fragment SimplePerson on Person {
  name
  department
  photo {
    url
  }
}

fragment SimpleProduct on Product {
  name
  price
  sku
  photos {
    url
  }
}

```
