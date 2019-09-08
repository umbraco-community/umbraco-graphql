using Our.Umbraco.GraphQL.Attributes;

namespace Website.Starwars.Schema
{
    public class StarWarsRootQuery
    {
        [NonNull]
        public StarWarsQuery StarWars([Inject] StarWarsQuery query) => query;
    }
}
