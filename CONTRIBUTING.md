# Contributing

## Bug Reports & Feature Requests

Please use the [issue tracker](https://github.com/rasmusjp/umbraco-graphql/issues) to report any bugs or file feature requests.

## Developing

### Prerequisites

-   Visual Studio
-   IIS Express

### Setup

First build the Solution

Run the `samples/Website` project, this will start IIS Express listeninng on `http://localhost:49937`

Install Umbraco with the starter kit, so you have some sample data.

### Folder structure

```
.
├── samples                            # Sample projects
│   └── Website                        # Sample/development site
├── src                                # Source Files
│   └── Our.Umbraco.GraphQL            # Main Project
├── test                               # Tests Projects
│   └── Our.Umbraco.GraphQL.Tests      # Unit Tests
└── tools                              # Build tools
```
