using System.ComponentModel.DataAnnotations.Schema;

namespace StarboundRecipeBook2.Models
{
    public class consumableData
    {
        public int SourceModId { get; set; } // PPK + FK
        public int consumableDataId { get; set; } // PPK
        public double FoodValue { get; set; }

        public int ItemId { get; set; } // FK
        public virtual Item Item { get; set; }

        //public virtual ICollection<StatusEffect> StatusEffects { get; set; }
        //public virtual ICollection<IDKTBH> AppliedEffects { get; set; }
    }
}
