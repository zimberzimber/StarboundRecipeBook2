namespace SBRB.Models
{
    public enum ArmorTypes : byte { Head, Chest, Legs, Back };

    // Lower case because they're translated from a string
    public enum Rarities : byte { common, uncommon, rare, epic, legendary, essential }

    public enum ItemTypes : byte { Generic, ActiveItem, Consumable, Object, Armor, Augment, CurrencyItem, Tool, Flashlight, Material, Liquid, Instrument, Codex }

    public enum ToolTypes : byte { MiningTool, Beamaxe, HarvestingTool, PaintTool, WireTool, InspectionTool, TillingTool }
}
