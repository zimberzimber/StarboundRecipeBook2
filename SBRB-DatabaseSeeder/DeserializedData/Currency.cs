namespace SBRB.Seeder.DeserializedData
{
    class DeserializedCurrency
    {
        public string representativeItem;
        public uint playerMax;
    }

    // Base game specific currency classes
    class DeserializedBaseGameCurrencyFile
    {
        public DeserializedMoney money;
        public DeserializedEssence essence;
    }

    class DeserializedMoney : DeserializedCurrency { }
    class DeserializedEssence : DeserializedCurrency { }
}
