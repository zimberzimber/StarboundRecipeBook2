using System.Collections.Generic;

namespace StarboundRecipeBook2.Models
{
    public class ItemCategory
    {
        public int ItemCategoryId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Item> Items { get; set; }
    }
}
