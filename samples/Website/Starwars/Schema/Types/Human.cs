using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.GraphQL.Attributes;
using Our.Umbraco.GraphQL.Types;
using Our.Umbraco.GraphQL.Types.Relay;
using Website.Starwars.Data;

namespace Website.Starwars.Schema.Types
{
    public class Human : ICharacter
    {
        private readonly HumanDto _dto;
        private readonly StarWarsData _data;

        public Human(HumanDto dto, StarWarsData data)
        {
            _dto = dto ?? throw new ArgumentNullException(nameof(dto));
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }

        [NonNull]
        [Description("The id of the human.")]
        public Id Id => new Id(_dto.Id);

        [NonNull]
        [Description("The name of the human.")]
        public string Name => _dto.Name;

        [NonNull]
        [Description("A list of the human's friends.")]
        public Connection<ICharacter> Friends(int? first = null, string after = null, int? last = null, string before = null) =>
            _data.GetFriends(_dto).AsCharacters(_data).ToConnection(x => x.Id, first, after, last, before);

        [NonNull, NonNullItem]
        [Description("Which movie they appear in.")]
        public IEnumerable<Episode> AppearsIn => _dto.AppearsIn.Select(x => (Episode)x);

        [NonNull]
        [Description("The home planet of the human.")]
        public string HomePlanet => _dto.HomePlanet;
    }
}
