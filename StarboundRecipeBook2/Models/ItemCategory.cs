using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarboundRecipeBook2.Models
{
    public class ItemCategory
    {
        public string CategoryName { get; set; } // PK

        public virtual ICollection<Item> Items { get; set; }
    }
}
