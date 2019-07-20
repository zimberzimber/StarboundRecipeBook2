namespace StarboundRecipeBook2.Models
{
    public class ConsumableData
    {
        public int SourceModId { get; set; } // PPK + FK
        public int ConsumableDataId { get; set; } // PPK
        public double FoodValue { get; set; }

        public int ItemId { get; set; } // FK
        public virtual Item Item { get; set; }

        //public virtual ICollection<StatusEffect> StatusEffects { get; set; }
        //public virtual ICollection<IDKTBH> AppliedEffects { get; set; }
    }
}
