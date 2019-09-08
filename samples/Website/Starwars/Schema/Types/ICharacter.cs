using System.Collections.Generic;
using Our.Umbraco.GraphQL.Attributes;
using Our.Umbraco.GraphQL.Types;
using Our.Umbraco.GraphQL.Types.Relay;

namespace Website.Starwars.Schema.Types
{
    [Name("Character")]
    public interface ICharacter
    {
        [NonNull]
        [Description("The id of the character.")]
        Id Id { get; }

        [NonNull]
        [Description("The name of the character.")]
        string Name { get; }

        [NonNull]
        [Description("A list of a character's friends.")]
        Connection<ICharacter> Friends(int? first = null, string after = null, int? last = null, string before = null);

        [NonNull]
        [Description("Which movie they appear in.")]
        IEnumerable<Episode> AppearsIn { get; }
    }
}
