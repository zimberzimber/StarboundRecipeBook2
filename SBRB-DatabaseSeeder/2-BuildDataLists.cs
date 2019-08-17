using Jil;
using SBRB.Models;
using SBRB.Seeder.DeserializedData;
using SBRB.Seeder.Extensions;
using SBRB_DatabaseSeeder.Workers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SBRB.Seeder
{
    partial class Program
    {
        // Queues containing the deserialized items and recipes
        static ConcurrentQueue<DeserializedItem> _deserializedItems = new ConcurrentQueue<DeserializedItem>();
        static ConcurrentQueue<DeserializedRecipe> _deserializedRecipes = new ConcurrentQueue<DeserializedRecipe>();

        /// <summary>
        /// Initialize the file deserialization proccess, converting them into appropriate classes.
        /// </summary>
        static void BuildQueues()
        {
            // Create a list containing all the deserialization tasks
            List<Task> tasks = new List<Task>();

            // Create, index, and start item deserialization tasks
            foreach (var itemFile in _itemFiles)
            {
                var task = new Task(() => ProcessItem(itemFile));
                tasks.Add(task);
                task.Start();
            }

            // Create, index, and start recipe deserialization tasks
            foreach (var recipeFile in _recipeFiles)
            {
                var task = new Task(() => ProcessRecipe(recipeFile));
                tasks.Add(task);
                task.Start();
            }

            // Wait for the deserialization tasks to complete.
            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Deserialize an item file into the appropriate class.
        /// </summary>
        /// <param name="file">Path to file</param>
        static void ProcessItem(string file)
        {
            Logging.Log("Processing item: {0}", file.TrimPath(modPath));

            // Create a placeholder for the deserialized item
            DeserializedItem item = null;

            // Get the JSON within the file, and remove comments. (As they're not supported by the deserializer)
            string json = File.ReadAllText(file).RemoveComments();

            // Create the item using a subclass based on the extension, and contain it within the previously created placeholder.
            switch (Path.GetExtension(file))
            {
                case ".item":
                    item = JSON.Deserialize<DeserializedItem>(json);
                    break;
                case ".object":
                    item = JSON.Deserialize<DeserializedObject>(json);
                    break;
                case ".consumable":
                    item = JSON.Deserialize<DeserializedConsumable>(json);
                    break;
                case ".activeitem":
                    item = JSON.Deserialize<DeserializedActiveItem>(json);
                    break;
                case ".flashlight":
                    item = JSON.Deserialize<DeserializedFlashlight>(json);
                    break;
                case ".augment":
                    item = JSON.Deserialize<DeserializedAugment>(json);
                    break;
                case ".currency":
                    item = JSON.Deserialize<DeserializedCurrencyItem>(json);
                    break;
                case ".matitem":
                    item = JSON.Deserialize<DeserializedMaterialItem>(json);
                    break;
                case ".liquid":
                    item = JSON.Deserialize<DeserializedLiquidItem>(json);
                    break;
                case ".instrument":
                    item = JSON.Deserialize<DeserializedInstrument>(json);
                    break;


                // Armors
                case ".head":
                    item = JSON.Deserialize<DeserializedArmor>(json);
                    (item as DeserializedArmor).armorType = ArmorTypes.Head;
                    break;
                case ".chest":
                    item = JSON.Deserialize<DeserializedArmor>(json);
                    (item as DeserializedArmor).armorType = ArmorTypes.Chest;
                    break;
                case ".legs":
                    item = JSON.Deserialize<DeserializedArmor>(json);
                    (item as DeserializedArmor).armorType = ArmorTypes.Legs;
                    break;
                case ".back":
                    item = JSON.Deserialize<DeserializedArmor>(json);
                    (item as DeserializedArmor).armorType = ArmorTypes.Back;
                    break;


                // Tools
                case ".beamaxe":
                    item = JSON.Deserialize<DeserializedTool>(json);
                    (item as DeserializedTool).ToolType = ToolTypes.Beamaxe;
                    break;
                case ".miningtool":
                    item = JSON.Deserialize<DeserializedTool>(json);
                    (item as DeserializedTool).ToolType = ToolTypes.MiningTool;
                    break;
                case ".harvestingtool":
                    item = JSON.Deserialize<DeserializedTool>(json);
                    (item as DeserializedTool).ToolType = ToolTypes.HarvestingTool;
                    break;
                case ".painttool":
                    item = JSON.Deserialize<DeserializedTool>(json);
                    (item as DeserializedTool).ToolType = ToolTypes.PaintTool;
                    break;
                case ".wiretool":
                    item = JSON.Deserialize<DeserializedTool>(json);
                    (item as DeserializedTool).ToolType = ToolTypes.WireTool;
                    break;
                case ".inspectiontool":
                    item = JSON.Deserialize<DeserializedTool>(json);
                    (item as DeserializedTool).ToolType = ToolTypes.InspectionTool;
                    break;
                case ".tillingtool":
                    item = JSON.Deserialize<DeserializedTool>(json);
                    (item as DeserializedTool).ToolType = ToolTypes.TillingTool;
                    break;

                default:
                    Logging.AddWarning("No handling method for item: {0}", file.TrimPath(modPath));
                    break;
            }

            // Set the items filepath, and enqueue it for later conversion to a database appropriate format if the item was created.
            // An item will not be created if theres no handling for the received extension.
            if (item != null)
            {
                item.filePath = file;
                _deserializedItems.Enqueue(item);
            }
        }

        /// <summary>
        /// Deserialize a recipe file into the appropriate class.
        /// </summary>
        /// <param name="file">Path to file</param>
        static void ProcessRecipe(string file)
        {
            Logging.Log("Deserializing recipe: {0}", file.TrimPath(modPath));

            // Deserialize the recipe file
            string json = File.ReadAllText(file).RemoveComments();
            DeserializedRecipe recipe = JSON.Deserialize<DeserializedRecipe>(json);
            recipe.filePath = file;

            // Enqueue the recipe for later conversion into a database appropriate format
            _deserializedRecipes.Enqueue(recipe);
        }
    }
}