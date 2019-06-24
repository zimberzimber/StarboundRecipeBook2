using System.Collections.Generic;

namespace StarboundRecipeBook2.Models
{
    public class Rarity
    {
        public string RarityName { get; set; } // PK

        public virtual ICollection<Item> Items { get; set; }
    }
}
