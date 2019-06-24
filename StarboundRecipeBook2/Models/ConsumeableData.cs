namespace StarboundRecipeBook2.Models
{
    public class ConsumeableData
    {
        public int FoodValue { get; set; }

        public string ItemName { get; set; } // PK + FK
        public virtual Item Item { get; set; }

        //public virtual ICollection<StatusEffect> StatusEffects { get; set; }
        //public virtual ICollection<IDKTBH> AppliedEffects { get; set; }
    }
}
