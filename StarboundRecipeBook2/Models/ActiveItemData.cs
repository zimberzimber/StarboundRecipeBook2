namespace StarboundRecipeBook2.Models
{
    public class ActiveItemData
    {
        public int ActiveItemDataId { get; set; }
        public double Level { get; set; }
        public bool TwoHanded { get; set; }
        public int ItemId { get; set; }

        public virtual Item Item { get; set; }
    }
}
