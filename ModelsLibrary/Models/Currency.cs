using MongoDB.Bson.Serialization.Attributes;

namespace SBRB.Models
{
    public class CompositeCurrencyId
    {
        public uint SourceModId { get; set; }
        public string CurrencyName { get; set; }
    }

    public class Currency
    {
        [BsonId]
        public int Id { get; set; }

        public string RepresentativeItem { get; set; }
        public uint PlayerMax { get; set; }
    }
}
