namespace StarboundRecipeBook2.Models
{
    public class MiningtoolData
    {
        public int SourceModId { get; set; } // PPK + FK
        public int MiningtoolDataID { get; set; } // PPK
        public int BlockRadius { get; set; }
        public double Durability { get; set; }
        public double DurabilityPerUse { get; set; }
        public bool TwoHanded { get; set; }
        public double FireTime { get; set; }

        public int ItemId { get; set; } // FK
        public virtual Item Item { get; set; }
    }
}
