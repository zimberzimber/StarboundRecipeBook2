using System.Collections.Generic;

namespace StarboundRecipeBook2.Models
{
    public class Rarity
    {
        public int RarityId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Item> Items { get; set; }
    }
}
