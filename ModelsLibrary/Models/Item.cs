using MongoDB.Bson.Serialization.Attributes;

namespace SBRB.Models
{
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
        public uint? MaxStack { get; set; }
        public uint? Price { get; set; }

        public bool? TwoHanded { get; set; }
        public double? Level { get; set; }

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
