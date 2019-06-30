using System.ComponentModel.DataAnnotations.Schema;

namespace StarboundRecipeBook2.Models
{
    public class ActiveItemData
    {
        public int SourceModId { get; set; } // PPK + FK
        public int ActiveItemDataId { get; set; } // PPK
        public double Level { get; set; }
        public bool TwoHanded { get; set; }

        public int ItemId { get; set; } // FK
        public virtual Item Item { get; set; }
    }
}
