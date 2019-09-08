using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.GraphQL.Attributes;
using Our.Umbraco.GraphQL.Types;
using Our.Umbraco.GraphQL.Types.Relay;
using Website.Starwars.Data;

namespace Website.Starwars.Schema.Types
{
    [Description("A mechanical creature in the Star Wars universe.")]
    public class Droid : ICharacter
    {
        private readonly DroidDto _dto;
        private readonly StarWarsData _data;

        public Droid(DroidDto dto, StarWarsData data)
        {
            _dto = dto ?? throw new ArgumentNullException(nameof(dto));
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }

        [NonNull]
        [Description("The id of the droid.")]
        public Id Id => new Id(_dto.Id);

        [NonNull]
        [Description("The name of the droid.")]
        public string Name => _dto.Name;

        [NonNull]
        [Description("A list of the droid's friends.")]
        public Connection<ICharacter> Friends(int? first = null, string after = null, int? last = null, string before = null) =>
            _data.GetFriends(_dto).AsCharacters(_data).ToConnection(x => x.Id, first, after, last, before);

        [NonNull, NonNullItem]
        [Description("Which movie they appear in.")]
        public IEnumerable<Episode> AppearsIn => _dto.AppearsIn.Select(x => (Episode)x);

        [NonNull]
        [Description("The primary function of the droid.")]
        public string PrimaryFunction => _dto.PrimaryFunction;
    }
}
