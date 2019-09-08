using System.Collections.Generic;
using System.Linq;

namespace Website.Starwars.Data
{
    public class StarWarsData
    {
        private readonly List<CharacterDto> _characters = new List<CharacterDto>();

        public StarWarsData()
        {
            _characters.Add(new HumanDto
            {
                Id = "1",
                Name = "Luke",
                Friends = new List<string> { "3", "4" },
                AppearsIn = new[] { 4, 5, 6 },
                HomePlanet = "Tatooine",
            });
            _characters.Add(new HumanDto
            {
                Id = "2",
                Name = "Vader",
                AppearsIn = new[] { 4, 5, 6 },
                HomePlanet = "Tatooine",
            });

            _characters.Add(new DroidDto
            {
                Id = "3",
                Name = "R2-D2",
                Friends = new List<string> { "1", "4" },
                AppearsIn = new[] { 4, 5, 6 },
                PrimaryFunction = "Astromech",
            });
            _characters.Add(new DroidDto
            {
                Id = "4",
                Name = "C-3PO",
                AppearsIn = new[] { 4, 5, 6 },
                PrimaryFunction = "Protocol",
            });
        }

        public IEnumerable<CharacterDto> GetFriends(CharacterDto character)
        {
            if (character == null)
            {
                return null;
            }

            var friends = new List<CharacterDto>();
            var lookup = character.Friends;
            if (lookup != null)
            {
                foreach (var potentialFriend in _characters)
                {
                    if (lookup.Contains(potentialFriend.Id))
                        friends.Add(potentialFriend);
                }
            }
            return friends;
        }

        public HumanDto GetHumanById(string id) => GetAllHumans().FirstOrDefault(x => x.Id == id);
        public DroidDto GetDroidById(string id) => GetAllDroids().FirstOrDefault(x => x.Id == id);

        public IEnumerable<HumanDto> GetAllHumans() => _characters.OfType<HumanDto>();
        public IEnumerable<DroidDto> GetAllDroids() => _characters.OfType<DroidDto>();

        public IEnumerable<CharacterDto> GetAllCharacters() => _characters.AsEnumerable();
    }
}
