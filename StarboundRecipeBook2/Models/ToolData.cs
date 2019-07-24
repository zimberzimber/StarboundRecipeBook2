namespace StarboundRecipeBook2.Models
{
    public class ToolData
    {
        public enum ToolType { MiningTool, Beamaxe, HarvestingTool, PaintTool, WireTool, InspectionTool, TillingTool }

        public int SourceModId { get; set; }
        public int ToolDataId { get; set; }
        public ToolType Type { get; set; }

        public bool TwoHanded { get; set; } = false;
        public int? BlockRadius { get; set; }
        public double? FireTime { get; set; }
        public double? RangeBonus { get; set; }
        public double? TileDamage { get; set; }

        // Mining Tool
        public double? Durability { get; set; }
        public double? DurabilityPerUse { get; set; }

        public int ItemId { get; set; } // FK
        public virtual Item Item { get; set; }
    }
}
