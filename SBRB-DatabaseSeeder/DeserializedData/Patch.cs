namespace SBRB.Seeder.DeserializedData
{
    public class DeserializedPatchFile
    {
        public string filePath;
        public DeserializedPatchContents[] contents { get; set; }
    }

    public class DeserializedPatchContents
    {
        public string op { get; set; }
        public string path { get; set; }
        public dynamic value { get; set; }
    }
}
