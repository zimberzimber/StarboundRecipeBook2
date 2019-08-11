using Jil;
using SBRB.Models;
using SBRB.Seeder.DeserializedData;
using SBRB.Seeder.Workers;
using SBRB_DatabaseSeeder.Workers;
using System.Collections.Generic;
using System.IO;

namespace SBRB.Seeder
{
    partial class Program
    {
        static List<DeserializedItem> _deserializedItems = new List<DeserializedItem>();
        static List<DeserializedRecipe> _deserializedRecipes = new List<DeserializedRecipe>();

        static void BuildItemList()
        {
            for (int i = 0; i < _itemFiles.Count; i++)
            {
                Logging.Log("Deserializing item: {0}", _itemFiles[i].ToReletivePath(modPath));

                DeserializedItem item = null;
                string json = File.ReadAllText(_itemFiles[i]).RemoveComments();

                switch (Path.GetExtension(_itemFiles[i]))
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
                        Logging.AddWarning("No handling method for item: {0}", _itemFiles[i].ToReletivePath(modPath));
                        break;
                }

                if (item != null)
                {
                    item.filePath = _itemFiles[i];
                    _deserializedItems.Add(item);
                }
            }
        }

        static void BuildRecipeList()
        {
            for (int i = 0; i < _recipeFiles.Count; i++)
            {
                Logging.Log("Deserializing recipe: {0}", _recipeFiles[i].ToReletivePath(modPath));

                string json = File.ReadAllText(_recipeFiles[i]).RemoveComments();
                DeserializedRecipe recipe = JSON.Deserialize<DeserializedRecipe>(json);
                recipe.filePath = _recipeFiles[i];

                _deserializedRecipes.Add(recipe);
            }
        }
    }
}