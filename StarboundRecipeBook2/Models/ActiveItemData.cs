namespace StarboundRecipeBook2.Models
{
    public class ActiveItemData
    {
        public double Level { get; set; }
        public bool TwoHanded { get; set; }

        public string ItemName { get; set; } // PK + FK
        public virtual Item Item { get; set; }
    }
}
