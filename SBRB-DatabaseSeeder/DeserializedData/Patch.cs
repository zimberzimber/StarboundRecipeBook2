namespace SBRB_DatabaseSeeder.DeserializedData
{
    public class DeserializedPatchFile
    {
        public DeserializedPatchContents[] contents { get; set; }
    }

    public class DeserializedPatchContents
    {
        public string op { get; set; }
        public string path { get; set; }
        public dynamic value { get; set; }
    }
}
