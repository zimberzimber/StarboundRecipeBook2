using System.Collections.Generic;

namespace StarboundRecipeBook2.Models
{
    public class Item
    {
        public enum Rarities : byte { common, uncommon, rare, epic, legendary, essential }
        public enum ItemTypes : byte { genericItem, activeItem, consumableItem, objectItem, armorItem }

        public int ItemId { get; set; } // PPK
        public string InternalName { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public byte[] Icon { get; set; }
        public int Price { get; set; }
        public int MaxStack { get; set; }
        public string ExtraData { get; set; }
        public string FilePath { get; set; }
        public Rarities Rarity { get; set; }
        public ItemTypes Type { get; set; }
        public string Category { get; set; }

        // FKs
        public int SourceModId { get; set; } // PPK
        public int? ArmorDataId { get; set; }
        public int? ObjectDataId { get; set; }
        public int? ActiveItemDataId { get; set; }
        public int? ConsumableDataId { get; set; }
        public int? MiningtoolDataId { get; set; }
        public int? FlashlightDataId { get; set; }
        public int? ToolDataId { get; set; }
        public int? AugmentDataId { get; set; }

        public virtual Mod SourceMod { get; set; }
        public virtual ArmorData ArmorData { get; set; }
        public virtual ObjectData ObjectData { get; set; }
        public virtual ActiveItemData ActiveItemData { get; set; }
        public virtual ConsumableData ConsumableData { get; set; }
        public virtual FlashlightData FlashlightData { get; set; }
        public virtual ToolData ToolData { get; set; }
        public virtual AugmentData AugmentData { get; set; }

        public virtual ICollection<RecipeUnlock> Unlocks { get; set; }
    }
}
