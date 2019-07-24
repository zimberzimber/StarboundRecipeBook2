using Jil;
using SBRB_DatabaseSeeder.Workers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using static StarboundRecipeBook2.Models.ArmorData;
using static StarboundRecipeBook2.Models.ToolData;

namespace SBRB_DatabaseSeeder.DeserializedData
{
    /// <summary>Class storing deserialized item contents. Including active items, consumables, and objects.</summary>
    class DeserializedItem
    {
        // Nested class for composite icon deserialization.
        class CompositeIconComponent
        {
            public string image { get; set; }
        }

        public enum ItemTypes { Generic, Object, Consumable, ActiveItem, Armor, Flashlight, Tool, Augment };

        public string itemName { get; set; }
        public string shortdescription { get; set; }
        public string description { get; set; }
        public string rarity { get; set; } = "common";
        public string category { get; set; }
        public int price { get; set; } = 0;
        public int maxStack { get; set; } = 1;
        public string tooltipKind { get; set; }
        public dynamic inventoryIcon { get; set; }
        public string[] learnBlueprintsOnPickup { get; set; }
        public bool SBRBhidden { get; set; }

        public ItemTypes itemType { get; set; }
        public string filePath { get; set; }

        public byte[] GenerateIconImage(ArmorType? armorType = null)
        {
            // Create the initial image, with 1x1 marking it as uninitialized.
            // Because I highly doubt anyone is going to use a smaller image.

            // EDGE CASE: Someone actually using a 1x1 sized icon
            // RESULT: The image will have its dimensions set to 1x1 everytime it layers a 1x1 sized component from the composite
            // I'll be sacrificing virgins to Khorne so he'd earase humanity if someone unironically uses a 1x1 composite image-
            // Or uses a composite with multiple dimensions where one of them is 1x1

            // EDGE CASE: Composite image consist of images with different sizes.
            // RESULT: Not going to account for that, as its also incorrect usage in Starbounds case.

            // EDGE CASE: The image the item is pointing at does not exist in its mod.
            // RESULT: The image will be use the missing asset placeholder

            using (Image<Rgba32> fullImage = new Image<Rgba32>(1, 1))
            {
                // Return null if there's no predefined icon.
                // Either a generic item who's icon is generated through a builder script, or a faulty item definition.
                if (inventoryIcon == null)
                    return null;

                // If the image is composite, run 'AddLayer' on each piece.
                if (inventoryIcon.ToString().Contains("[{")) // Collection of paths
                {
                    string[] componentsJSON = inventoryIcon.ToString().Replace("[", "").Replace("]", "").Split(',');

                    for (int i = 0; i < componentsJSON.Length; i++)
                    {
                        var component = JSON.Deserialize<CompositeIconComponent>(componentsJSON[i]);
                        fullImage.AddLayer(component.image, filePath, armorType);
                    }
                }

                // Single image, 'AddLayer' it.
                else
                {
                    string path = inventoryIcon.ToString().Replace("\"", string.Empty);
                    fullImage.AddLayer(path, filePath, armorType);
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

    class DeserializedConsumable : DeserializedItem
    {
        public double foodValue { get; set; }
    }

    class DeserializedActiveItem : DeserializedItem
    {
        public double level { get; set; }
        public bool twoHanded { get; set; } = false;
        public string elementalType { get; set; }
    }

    class DeserializedArmor : DeserializedItem
    {
        public double level { get; set; }
        public ArmorType armorType { get; set; }
    }

    class DeserializedFlashlight : DeserializedItem
    {
        public int[] lightColor { get; set; }
        public double beamLevel { get; set; }
        public double beamAmbience { get; set; }
    }

    class DeserializedTool : DeserializedItem
    {
        public ToolType ToolType { get; set; }

        public int? blockRadius { get; set; }
        public bool twoHanded { get; set; } = false;
        public double? fireTime { get; set; }
        public double? tileDamage { get; set; }
        public double? rangeBonus { get; set; }

        public double? durability { get; set; }
        public double? durabilityPerUse { get; set; }
    }

    class DeserializedAugment : DeserializedItem
    {
        public DeserializedAugmentData augment { get; set; }
    }

    // Starbound has the actual augment data stored in a table within the items definitions
    // It will be deserialized into this
    public class DeserializedAugmentData
    {
        public string type { get; set; }
        public string name { get; set; }
        public string displayName { get; set; }
        public string displayIcon { get; set; }
        //public DeserializedAugmentEffects[] effects { get; set; }
    }
}
