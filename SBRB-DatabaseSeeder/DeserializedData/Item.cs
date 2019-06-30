using Jil;
using SBRB_DatabaseSeeder.Workers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace SBRB_DatabaseSeeder.DeserializedData
{
    /// <summary>
    /// Class storing deserialized item contents. Including active items, consumeables, and objects.
    /// </summary>
    class DeserializedItem
    {
        // TO DO:
        // 'ImageBuilder's method 'AddLayer' return an enum for error handling. Add the right checks.

        // Nested class for composite icon deserialization.
        class CompositeIconComponent
        {
            public string image { get; set; }
        }

        public enum ItemType { Generic, Object, Consumeable, ActiveItem };

        public string itemName { get; set; }
        public string shortdescription { get; set; }
        public string description { get; set; }
        public string rarity { get; set; }
        public string category { get; set; }
        public int price { get; set; }
        public int maxStack { get; set; } = 1;
        public string tooltipKind { get; set; }
        public dynamic inventoryIcon { get; set; }

        public ItemType itemType { get; set; }
        public string filePath { get; set; }

        public byte[] GenerateIconImage()
        {
            // Create the initial image, with 1x1 marking it as uninitialized.
            // Because I highly doubt anyone is going to use a smaller image.

            // EDGE CASE: Someone actually using a 1x1 sized icon
            // RESULT: The image will have its dimensions set to 1x1 everytime it layers a 1x1 sized component from the composite
            // I'll be sacrificing virgins to Khorne so he'd earase humanity if someone unironically uses a 1x1 composite image-
            // Or uses a composite with multiple dimensions where one of them is 1x1

            // EDGE CASE: Composite image consist of images with different sizes.
            // RESULT: Not going to account for that, as its also incorrect usage in Starbounds case.


            using (Image<Rgba32> fullImage = new Image<Rgba32>(1, 1))
            {
                // Return null if there's no predefined icon.
                // Either a generic item who's icon is generated through a builder script, or a faulty item definition.
                if (inventoryIcon == null)
                    // TO DO
                    return null;

                // If the image is composite, run 'AddLayer' on each piece.
                if (inventoryIcon.ToString().Contains("[{")) // Collection of paths
                {
                    string[] componentsJSON = inventoryIcon.ToString().Replace("[", "").Replace("]", "").Split(',');

                    for (int i = 0; i < componentsJSON.Length; i++)
                    {
                        var component = JSON.Deserialize<CompositeIconComponent>(componentsJSON[i]);
                        fullImage.AddLayer(component.image, filePath);
                    }
                }

                // Single image, 'AddLayer' it.
                else
                {
                    string path = inventoryIcon.ToString().Replace("\"", string.Empty);
                    fullImage.AddLayer(path, filePath);
                }

                // If the dimensions remained 1x1, the image failed to generate, and is empty.
                if (fullImage.Height == 1 && fullImage.Width == 1)
                    return null;

                // Create a memory stream to contain the raw image data.
                using (MemoryStream mem = new MemoryStream())
                {
                    // Convert the image into the memory stream, and reset the memory streams position for reading later.
                    fullImage.SaveAsPng(mem);
                    mem.Position = 0;

                    // Create a binary stream to write the image stored within the memory stream into a byte array, and return the result.
                    using (BinaryReader bnr = new BinaryReader(mem))
                        return bnr.ReadBytes((int)mem.Length);
                }
            }
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
