using Jil;
using SBRB.Models;
using SBRB.Seeder.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace SBRB.Seeder.DeserializedData
{
    /// <summary>Class storing deserialized item contents. Including active items, consumables, and objects.</summary>
    class DeserializedItem
    {
        // Nested class for composite icon deserialization.
        class CompositeIconComponent
        {
            public string image;
        }

        public string itemName;
        public string shortdescription;
        public string description;
        public string rarity = "common";
        public string category;
        public uint? price;
        public uint? maxStack;
        public string tooltipKind;
        public dynamic inventoryIcon;
        public string[] learnBlueprintsOnPickup;
        public bool SBRBhidden = false;

        public string filePath;

        public byte[] GenerateIconImage(ArmorTypes? armorType = null)
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
            // RESULT: The image will use the missing asset placeholder

            using (Image<Rgba32> fullImage = new Image<Rgba32>(1, 1))
            {
                // Return null if there's no predefined icon.
                // Either a generic item who's icon is generated through a builder script, or a faulty item definition.
                if (inventoryIcon == null)
                    return null;

                // If the image is composite, run 'AddLayer' on each piece.
                if (inventoryIcon.ToString().Contains("[{")) // Collection of paths (Hacky, but dynamic objects are ass :v)
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
        public string race;
        public bool? printable;
        public string[] colonyTags;

        // Objects use 'objectName' instead of 'itemName'
        public string objectName { get => itemName; set => itemName = value; }
    }

    class DeserializedConsumable : DeserializedItem
    {
        public double? foodValue;
    }

    class DeserializedActiveItem : DeserializedItem
    {
        public double? level;
        public bool? twoHanded;
        public string elementalType;
    }

    class DeserializedArmor : DeserializedItem
    {
        public double? level;
        public ArmorTypes armorType;
    }

    class DeserializedFlashlight : DeserializedItem
    {
        public byte[] lightColor;
        public double beamLevel;
        public double beamAmbience;
    }

    class DeserializedTool : DeserializedItem
    {
        public ToolTypes ToolType;

        public uint? blockRadius;
        public bool? twoHanded;
        public double? fireTime;
        public double? tileDamage;
        public double? rangeBonus;

        public double? durability;
        public double? durabilityPerUse;
    }

    class DeserializedAugment : DeserializedItem
    {
        public DeserializedAugmentData augment;
    }

    // Starbound has the actual augment data stored in a table within the items definitions
    // It will be deserialized into this
    class DeserializedAugmentData
    {
        public string type;
        public string name;
        public string displayName;
        public string displayIcon;
    }

    class DeserializedCurrencyItem : DeserializedItem
    {
        public string currency;
        public int value;
    }

    class DeserializedMaterialItem : DeserializedItem
    {
        public uint materialId;
    }

    class DeserializedLiquidItem : DeserializedItem
    {
        public string liquid;
    }

    class DeserializedInstrument : DeserializedItem
    {

    }
}
