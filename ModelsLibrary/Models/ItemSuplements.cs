namespace SBRB.Models
{

    public class ArmorData
    {
        public ArmorTypes ArmorType;
    }

    public class AugmentData
    {
        public string Type;
        public string Name;
        public string DisplayName;
    }

    public class ConsumableData
    {
        public double? FoodValue;
    }

    public class CurrencyData
    {
        public int CurrencyValue;
        public string CurrencyName;
    }

    public class FlashlightData
    {
        public string LightColor;
        public double? BeamLevel;
        public double? BeamAmbience;
    }

    public class LiquidData
    {
        public string Liquid;
    }

    public class MaterialData
    {
        public uint MaterialId;
    }

    public class ObjectData
    {
        public string Race = "generic";
        public bool? Printable;
        public string[] ColonyTags;
    }

    public class ToolData
    {
        public ToolTypes ToolType;
        public uint? BlockRadius;
        public double? FireTime;
        public double? RangeBonus;
        public double? TileDamage;
    }

}
