using System;
using Our.Umbraco.GraphQL.Adapters.Types.Resolution;
using Our.Umbraco.GraphQL.Types;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Website.Starwars.Data;
using Website.Starwars.Schema;

namespace Website
{
    public class StarwarsComposer : ComponentComposer<StarwarsComponent>, IUserComposer
    {
        public override void Compose(IUmbracoBuilder builder)
        {
            base.Compose(builder);

            builder.Services.AddUnique<StarWarsData>();
        }
    }

    public class StarwarsComponent : IComponent
    {
        private readonly ITypeRegistry _typeRegistry;

        public StarwarsComponent(ITypeRegistry typeRegistry)
        {
            _typeRegistry = typeRegistry ?? throw new ArgumentNullException(nameof(typeRegistry));
        }

        public void Initialize()
        {
            _typeRegistry.Extend<Query, StarWarsRootQuery>();
        }

        public void Terminate()
        {
        }
    }
}
