namespace StarboundRecipeBook2.Models
{
    public class Relationship_Item_Item
    {
        public string UnlockingItemName { get; set; } // PPK + FK
        public string UnlockedItemName { get; set; } // PPK + FK

        public virtual Item UnlockingItem { get; set; }
        public virtual Item UnlockedItem { get; set; }
    }
}
