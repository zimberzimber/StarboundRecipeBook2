namespace StarboundRecipeBook2.Models
{
    public class BeamaxeData
    {
        public int SourceModId { get; set; } // PPK + FK
        public int BeamaxeDataID { get; set; } // PPK
        public double FireTime { get; set; }
        public double BlockRadius { get; set; }
        public double TileDamage { get; set; }
        public double RangeBonus { get; set; }

        public int ItemId { get; set; } // FK
        public virtual Item Item { get; set; }
    }
}
