namespace StarboundRecipeBook2.Models
{
    // NOTE: The resulting outputs cannot be resolved during seeding as an item in a mod may be unlocking a recipe for a different mod
    public class RecipeUnlock
    {
        public int UnlockingItemId { get; set; } // PPK + FK
        public int UnlockingItemSourceModId { get; set; } // PPK + FK
        public virtual string UnlockedItemName { get; set; } // PPK + FK

        public virtual Item UnlockingItem { get; set; }
    }
}
