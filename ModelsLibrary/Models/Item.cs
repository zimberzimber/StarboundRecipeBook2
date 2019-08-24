using MongoDB.Bson.Serialization.Attributes;

namespace SBRB.Models
{
    /// <summary>
    /// Class for storing all possible Starbound items (Generic, tools, armors, blocks, objects, etc...)
    /// </summary>
    public class Item
    {
        [BsonId]
        public CompositeItemId ID;

        public string FilePath;
        public string InternalName;
        public string Description;
        public string ShortDescription;
        public byte[] Icon;
        public string Category;

        public ItemTypes ItemType = ItemTypes.Generic;
        public Rarities Rarity = Rarities.common;
        public uint? MaxStack;
        public uint? Price;

        public bool? TwoHanded;
        public double? Level;

        public string[] blueprintsOnPickup;

        public ArmorData Armor;
        public AugmentData Augment;
        public ConsumableData Consumable;
        public CurrencyData Currency;
        public FlashlightData Flashlight;
        public LiquidData Liquid;
        public MaterialData Material;
        public ObjectData Object;
        public ToolData Tool;
    }
}
