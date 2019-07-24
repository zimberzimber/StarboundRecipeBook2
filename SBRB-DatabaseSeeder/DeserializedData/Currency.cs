using System.Collections.Generic;

namespace SBRB_DatabaseSeeder.DeserializedData
{
    class DeserializedCurrencyCollection
    {
        public Dictionary<string, DeserializedCurrency> Collection { get; set; }
    }

    class DeserializedCurrency
    {
        public string representativeItem { get; set; }
        public int playerMax { get; set; }
    }
}
