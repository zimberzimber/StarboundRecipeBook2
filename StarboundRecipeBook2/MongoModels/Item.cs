using MongoDB.Bson.Serialization.Attributes;

// missing exts: .instrument .liquid .matitem .throwitem

namespace StarboundRecipeBook2.MongoModels
{
    public enum ArmorTypes : byte { Head, Chest, Legs, Back };
    public enum Rarities : byte { common, uncommon, rare, epic, legendary, essential } // Lower case because they're translated from a string
    public enum ItemTypes : byte { Generic, ActiveItem, Consumable, Object, Armor, Augment, CurrencyItem, Tool, Flashlight }
    public enum ToolTypes : byte { MiningTool, Beamaxe, HarvestingTool, PaintTool, WireTool, InspectionTool, TillingTool }

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
        public double FoodValue { get; set; } = 0;
    }

    public class CurrencyData
    {
        public int CurrencyValue { get; set; } = 1;
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
        public bool Printable { get; set; } = false;
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

    /// <summary>
    /// Class for storing all possible Starbound items (Generic, tools, armors, blocks, objects, etc...)
    /// </summary>
    public class Item
    {
        [BsonId]
        public CompositeItemId ID { get; set; }

        public string FilePath { get; set; }
        public string InternalName { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public byte[] Icon { get; set; }
        public string Category { get; set; }

        public ItemTypes ItemType { get; set; } = ItemTypes.Generic;
        public Rarities Rarity { get; set; } = Rarities.common;
        public uint MaxStack { get; set; } = 1;
        public uint Price { get; set; } = 0;

        public bool TwoHanded { get; set; } = false;
        public double Level { get; set; } = 0;

        public string[] blueprintsOnPickup { get; set; }

        public ArmorData Armor { get; set; }
        public AugmentData Augment { get; set; }
        public ConsumableData Consumable { get; set; }
        public CurrencyData Currency { get; set; }
        public FlashlightData Flashlight { get; set; }
        public LiquidData Liquid { get; set; }
        public MaterialData Material { get; set; }
        public ObjectData Object { get; set; }
        public ToolData Tool { get; set; }
    }
}
