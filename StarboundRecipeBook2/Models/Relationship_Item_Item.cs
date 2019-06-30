namespace StarboundRecipeBook2.Models
{
    public class Relationship_Item_Item
    {
        public int UnlockingItemId { get; set; } // PPK + FK
        public int UnlockingItemSourceModId { get; set; } // PPK + FK

        public int UnlockedItemId { get; set; } // PPK + FK
        public int UnlockedItemSourceModId { get; set; } // PPK + FK

        public virtual Item UnlockingItem { get; set; }
        public virtual Item UnlockedItem { get; set; }
    }
}
