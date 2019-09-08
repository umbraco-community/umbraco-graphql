using System;
using System.Linq;
using Our.Umbraco.GraphQL.Attributes;
using Our.Umbraco.GraphQL.Types;
using Our.Umbraco.GraphQL.Types.Relay;
using Website.Starwars.Data;
using Website.Starwars.Schema.Types;

namespace Website.Starwars.Schema
{
    [Description("The Star Wars API.")]
    public class StarWarsQuery
    {
        private readonly StarWarsData _data;

        public StarWarsQuery(StarWarsData data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public ICharacter Hero() => Droid("3");

        [NonNull]
        [Description("Returns all characters.")]
        public Connection<ICharacter> Characters(int? first = null, string after = null, int? last = null, string before = null) =>
            _data.GetAllCharacters().AsCharacters(_data).ToConnection(x => x.Id, first, after, last, before);

        public Droid Droid(Id id)
        {
            var dto = _data.GetDroidById(id);
            return dto == null ? null : new Droid(dto, _data);
        }

        [NonNull]
        public Connection<Droid> Droids(int? first = null, string after = null, int? last = null, string before = null) =>
            _data.GetAllDroids().AsCharacters(_data).Cast<Droid>().ToConnection(x => x.Id, first, after, last, before);

        public ICharacter Human(Id id)
        {
            var dto = _data.GetHumanById(id);
            return dto == null ? null : new Human(dto, _data);
        }

        [NonNull]
        [Name("humans")]
        public Connection<Human> GetAllHumans(int? first = null, string after = null, int? last = null, string before = null) =>
            _data.GetAllHumans().AsCharacters(_data).Cast<Human>().ToConnection(x => x.Id, first, after, last, before);
    }
}
