namespace StarboundRecipeBook2.Models
{
    public class AugmentData
    {
        public int SourceModId { get; set; } // PPK + FK
        public int AugmentDataId { get; set; } // PPK

        public string AugmentType { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        //public AugmentEffects[] Effects { get; set; }

        public int ItemId { get; set; } // FK
        public virtual Item Item { get; set; }
    }
}
