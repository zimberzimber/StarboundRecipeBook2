using MongoDB.Bson.Serialization.Attributes;

namespace SBRB.Models
{
    public class CompositeCurrencyId : ModDependant
    {
        public string CurrencyName;
    }

    public class Currency
    {
        [BsonId]
        public CompositeCurrencyId ID;

        public string RepresentativeItem;
        public uint PlayerMax;
    }
}
