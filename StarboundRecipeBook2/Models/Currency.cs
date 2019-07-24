namespace StarboundRecipeBook2.Models
{
    public class Currency
    {
        public int SourceModId { get; set; } // PPK + FK
        public string Name { get; set; } // PPK
        public int PlayerMax { get; set; }

        public string RepresentativeItemName { get; set; } // FK
        public Item RepresentativeItem{ get; set; }
    }
}
