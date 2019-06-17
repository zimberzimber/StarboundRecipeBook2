namespace StarboundRecipeBook2.Models
{
    public class Relationship_Item_Item
    {
        public int UnlockingItemId { get; set; }
        public int UnlockedItemId { get; set; }

        public virtual Item UnlockingItem { get; set; }
        public virtual Item UnlockedItem { get; set; }
    }
}
