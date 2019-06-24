using Jil;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System.Collections.Generic;
using System.IO;
using SBRB_DatabaseSeeder.Workers;

namespace SBRB_DatabaseSeeder.DeserializedData
{
    class DeserializedItem
    {
        class CompositeIconComponent
        {
            public string image { get; set; }
        }

        class FramesFile
        {
            public Framegrid frameGrid { get; set; }
            public Dictionary<string, string> aliases { get; set; }
        }

        class Framegrid
        {
            public int[] size { get; set; }
            public int[] dimensions { get; set; }
            public string[][] names { get; set; }
        }

        public enum ItemType { Generic, Object, Consumeable, ActiveItem };

        public string itemName { get; set; }
        public string shortdescription { get; set; }
        public string description { get; set; }
        public string rarity { get; set; }
        public string category { get; set; }
        public int price { get; set; }
        public int maxStack { get; set; }
        public string tooltipKind { get; set; }
        public dynamic inventoryIcon { get; set; }

        public ItemType itemType { get; set; }
        public string filePath { get; set; }

        public byte[] GenerateIconImage()
        {
            Image<Rgba32> fullImage = new Image<Rgba32>(1, 1);
            byte[] output;

            try
            {
                // No defined icon. Usually a generic item whose icon is generated through a builder.
                if (inventoryIcon == null)
                    // TO DO
                    return null;

                // Collection of paths
                if (inventoryIcon.ToString().Contains("[{")) // Collection of paths
                {
                    string[] componentsJSON = inventoryIcon.ToString().Replace("[", "").Replace("]", "").Split(',');

                    for (int i = 0; i < componentsJSON.Length; i++)
                    {
                        var component = JSON.Deserialize<CompositeIconComponent>(componentsJSON[i]);
                        fullImage.AddLayer(component.image, filePath);
                    }
                }

                // Single path
                else
                {
                    string path = inventoryIcon.ToString().Replace("\"", string.Empty);
                    fullImage.AddLayer(path, filePath);
                }

                using (MemoryStream mem = new MemoryStream())
                {
                    fullImage.SaveAsPng(mem);
                    mem.Position = 0;
                    using (BinaryReader bnr = new BinaryReader(mem))
                    {
                        output = bnr.ReadBytes((int)mem.Length);

                        using (FileStream fs = new FileStream("test2.png", FileMode.Create))
                        using (BinaryWriter writer = new BinaryWriter(fs))
                        { writer.Write(output); }
                    }
                }

                return output;
            }
            finally
            { fullImage.Dispose(); }
        }
    }

    class DeserializedObject : DeserializedItem
    {
        public string race { get; set; }
        public bool printable { get; set; }
        public string[] colonyTags { get; set; }

        // Objects use 'objectName' instead of 'itemName'
        public string objectName { get => itemName; set => itemName = value; }
    }

    class DeserializedConsumeable : DeserializedItem
    {
        public int foodValue { get; set; }
    }

    class DeserializedActiveItem : DeserializedItem
    {
        public double level { get; set; }
        public bool twoHanded { get; set; }
        public string elementalType { get; set; }
    }
}
