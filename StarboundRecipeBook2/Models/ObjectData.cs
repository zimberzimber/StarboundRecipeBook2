using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarboundRecipeBook2.Models
{
    public class ObjectData
    {
        public int SourceModId { get; set; } // PPK + FK
        public int ObjectDataId { get; set; } // PPK
        public string Race { get; set; }
        public string ColonyTags { get; set; }
        public bool Printable { get; set; }

        public int ItemId { get; set; } // FK
        public virtual Item Item { get; set; }
    }
}
