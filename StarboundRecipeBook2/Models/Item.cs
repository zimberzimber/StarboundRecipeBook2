using System.Collections.Generic;

namespace StarboundRecipeBook2.Models
{
    public class Item
    {
        public enum Rarities : byte { common, uncommon, rare, epic, legendary, essential }
        public enum ItemTypes : byte { genericItem, activeItem, consumeableItem, objectItem}

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
        public int SourceModId { get; set; } // PPK + FK
        public int? ObjectDataId { get; set; }
        public int? ActiveItemDataId { get; set; }
        public int? ConsumeableDataId { get; set; }

        public virtual Mod SourceMod { get; set; }
        public virtual ObjectData ObjectData { get; set; }
        public virtual ActiveItemData ActiveItemData { get; set; }
        public virtual ConsumeableData ConsumeableData { get; set; }

        public virtual ICollection<RecipeInput> RecipesUsedIn { get; set; }
        public virtual ICollection<Recipe> RecipesCraftedFrom { get; set; }
        public virtual ICollection<RecipeUnlock> Unlocks { get; set; }
    }
}
