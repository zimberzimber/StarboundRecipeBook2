namespace SBRB.Models
{
    /// <summary>
    /// Class used to compose an Items unique ID within the database
    /// </summary>
    public class CompositeItemId
    {
        public uint ItemId { get; set; }
        public uint SourceModId { get; set; }
    }

    public class ArmorData
    {
        public ArmorTypes ArmorType { get; set; }
    }

    public class AugmentData
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }

    public class ConsumableData
    {
        public double? FoodValue { get; set; }
    }

    public class CurrencyData
    {
        public int CurrencyValue { get; set; }
        public string CurrencyName { get; set; }
    }

    public class FlashlightData
    {
        public string LightColor { get; set; }
        public double? BeamLevel { get; set; }
        public double? BeamAmbience { get; set; }
    }

    public class LiquidData
    {
        public string Liquid { get; set; }
    }

    public class MaterialData
    {
        public uint MaterialId { get; set; }
    }

    public class ObjectData
    {
        public string Race { get; set; } = "generic";
        public bool? Printable { get; set; }
        public string[] ColonyTags { get; set; }
    }

    public class ToolData
    {
        public ToolTypes ToolType { get; set; }
        public uint? BlockRadius { get; set; }
        public double? FireTime { get; set; }
        public double? RangeBonus { get; set; }
        public double? TileDamage { get; set; }
    }

}
