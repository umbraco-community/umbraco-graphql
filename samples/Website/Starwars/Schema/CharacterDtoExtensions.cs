using System.Collections.Generic;
using Website.Starwars.Data;
using Website.Starwars.Schema.Types;

namespace Website.Starwars.Schema
{
    public static class CharacterDtoExtensions
    {
        public static IEnumerable<ICharacter> AsCharacters(this IEnumerable<CharacterDto> characters, StarWarsData data)
        {
            foreach (var character in characters)
            {
                switch (character)
                {
                    case HumanDto humanDto:
                        yield return new Human(humanDto, data);
                        break;

                    case DroidDto droidDto:
                        yield return new Droid(droidDto, data);
                        break;
                }
            }
        }
    }
}
