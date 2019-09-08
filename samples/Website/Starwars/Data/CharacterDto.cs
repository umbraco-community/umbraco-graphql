using System.Collections.Generic;

namespace Website.Starwars.Data
{
    public abstract class CharacterDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Friends { get; set; }
        public int[] AppearsIn { get; set; }
    }
}
