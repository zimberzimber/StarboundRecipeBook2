namespace StarboundRecipeBook2.Models
{
    public class ArmorData
    {
        public enum ArmorType { Head, Chest, Legs, Back };

        public int SourceModId { get; set; } // PPK + FK
        public int ArmorDataId { get; set; } // PPK
        public double Level { get; set; }
        public ArmorType Type { get; set; }

        public int ItemId { get; set; } // FK
        public virtual Item Item { get; set; }

        // Add status modifiers later
    }
}
