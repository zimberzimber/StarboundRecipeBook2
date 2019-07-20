namespace StarboundRecipeBook2.Models
{
    public class FlashlightData
    {
        public int SourceModId { get; set; } // PPK + FK
        public int FlashlightDataID { get; set; } // PPK
        public string LightColor { get; set; }
        public double BeamLevel { get; set; }
        public double BeamAmbience { get; set; }

        public int ItemId { get; set; } // FK
        public virtual Item Item { get; set; }
    }
}
