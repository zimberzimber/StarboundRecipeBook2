using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StarboundRecipeBook2.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        public string InternalName { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public byte[] Icon { get; set; }
        public int Price { get; set; }
        public int MaxStack { get; set; }
        public string ExtraData { get; set; }
        public string FilePath { get; set; }

        public int SourceModId { get; set; }
        public int RarityId { get; set; }
        public int ItemTypeId { get; set; }
        public int ItemCategoryId { get; set; }
        public int? ObjectDataId { get; set; }
        public int? ActiveItemDataId { get; set; }
        public int? ConsumeableDataId { get; set; }

        public virtual Mod SourceMod { get; set; }
        public virtual Rarity Rarity { get; set; }
        public virtual ItemType ItemType { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        public virtual ObjectData ObjectData { get; set; }
        public virtual ActiveItemData ActiveItemData { get; set; }
        public virtual ConsumeableData ConsumeableData { get; set; }

        public virtual ICollection<RecipeInput> RecipesUsedIn { get; set; }
        public virtual ICollection<Recipe> RecipesCraftedFrom { get; set; }
        public virtual ICollection<Relationship_Item_Item> Unlocks { get; set; }
        public virtual ICollection<Relationship_Item_Item> UnlockedBy { get; set; }
    }
}
