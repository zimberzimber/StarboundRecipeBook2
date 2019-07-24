namespace StarboundRecipeBook2.Models
{
    public class CurrencyItemData
    {
        public int SourceModId { get; set; } // PPK + FK
        public int CurrencyItemDataId { get; set; } // PPK
        public int Value { get; set; }
        public string CurrencyName { get; set; } // FK
        //public Currency Currency { get; set; }

        public int ItemId { get; set; } // FK
        public virtual Item Item { get; set; }
    }
}
