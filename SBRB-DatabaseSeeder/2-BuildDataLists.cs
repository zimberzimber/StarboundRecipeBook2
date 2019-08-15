using Jil;
using SBRB.Models;
using SBRB.Seeder.DeserializedData;
using SBRB.Seeder.Workers;
using SBRB_DatabaseSeeder.Workers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SBRB.Seeder
{
    partial class Program
    {
        static ConcurrentQueue<DeserializedItem> _deserializedItems = new ConcurrentQueue<DeserializedItem>();
        static ConcurrentQueue<DeserializedRecipe> _deserializedRecipes = new ConcurrentQueue<DeserializedRecipe>();

        static void ProcessItem(string file)
        {
            Logging.Log("Processing item: {0}", file.ToReletivePath(modPath));

            DeserializedItem item = null;
            string json = File.ReadAllText(file).RemoveComments();

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
                    Logging.AddWarning("No handling method for item: {0}", file.ToReletivePath(modPath));
                    break;
            }

            if (item != null)
            {
                item.filePath = file;
                _deserializedItems.Enqueue(item);
            }
        }

        static void ProcessRecipe(string file)
        {
            Logging.Log("Deserializing recipe: {0}", file.ToReletivePath(modPath));

            string json = File.ReadAllText(file).RemoveComments();
            DeserializedRecipe recipe = JSON.Deserialize<DeserializedRecipe>(json);
            recipe.filePath = file;

            _deserializedRecipes.Enqueue(recipe);
        }

        static void BuildQueues()
        {
            List<Task> tasks = new List<Task>();

            foreach (var itemFile in _itemFiles)
            {
                var task = new Task(() => ProcessItem(itemFile));
                tasks.Add(task);
                task.Start();
            }

            foreach (var recipeFile in _recipeFiles)
            {
                var task = new Task(() => ProcessRecipe(recipeFile));
                tasks.Add(task);
                task.Start();
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}