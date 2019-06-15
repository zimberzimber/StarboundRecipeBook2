using System.Collections.Generic;

namespace StarboundRecipeBook2.Models
{
    public class ItemType
    {
        public int ItemTypeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Item> Items { get; set; }
    }
}
