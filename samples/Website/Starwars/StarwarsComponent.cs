using System;
using Our.Umbraco.GraphQL.Adapters.Types.Resolution;
using Our.Umbraco.GraphQL.Types;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Website.Starwars.Data;
using Website.Starwars.Schema;

namespace Website
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class StarwarsComposer : ComponentComposer<StarwarsComponent>, IUserComposer
    {
        public override void Compose(Composition composition)
        {
            base.Compose(composition);

            composition.Register<StarWarsData>(Lifetime.Singleton);
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
