using System.Collections.Generic;

namespace StarboundRecipeBook2.Models
{
    public class ItemType
    {
        public string FileExtension { get; set; } // PK
        public string TypeName { get; set; }

        public virtual ICollection<Item> Items { get; set; }
    }
}
