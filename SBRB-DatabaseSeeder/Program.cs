using Jil;
using Microsoft.EntityFrameworkCore;
using SBRB_DatabaseSeeder.DeserializedData;
using SBRB_DatabaseSeeder.Workers;
using StarboundRecipeBook2.Data;
using StarboundRecipeBook2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

// NOTE:
// Certain edge cases may be discovered and not resolved yet.
// Look for them via Ctrl + F 'EDGE CASE' through the entire project

namespace SBRB_DatabaseSeeder
{
    class Program
    {
        //public static string modPath = @"D:\Games\steamapps\common\Starbound\mods\Ztarbound";
        public static string modPath = @"D:\Games\steamapps\common\Starbound\mods\_FrackinUniverse-master";
        //public static string modPath = @"D:\Games\steamapps\common\Starbound\_UnpackedVanillaAssets";
        static Mod _mod;

        const string MOD_REMOVAL_QUERY = @"delete from Mods where SteamId = {0};
delete from ActiveItemDatas where SourceModId = {0};
delete from ConsumableDatas where SourceModId = {0};
delete from Items where SourceModId = {0};
delete from ObjectDatas where SourceModId = {0};
delete from RecipeInputs where SourceModId = {0};
delete from Recipes where SourceModId = {0};
delete from RecipeUnlocks where UnlockingItemSourceModId = {0};
delete from Relationship_Recipe_RecipeGroup where SourceModId = {0};";

        static bool silent = false;
        static FileStream logFile;

        static List<string> _itemFiles = new List<string>();
        static List<string> _recipeFiles = new List<string>();
        static List<DeserializedItem> _deserializedItems = new List<DeserializedItem>();
        static List<DeserializedRecipe> _deserializedRecipes = new List<DeserializedRecipe>();

        static List<Item> _DBItems = new List<Item>();
        static List<ObjectData> _DBObjectDatas = new List<ObjectData>();
        static List<ActiveItemData> _DBActiveItemDatas = new List<ActiveItemData>();
        static List<consumableData> _DBconsumableDatas = new List<consumableData>();
        static List<RecipeUnlock> _DBRecipeUnlocks = new List<RecipeUnlock>();

        static List<string> _warningMessages = new List<string>();

        static void Main()
        {
            // Create a file to contain the logged messages
            Directory.CreateDirectory("logs");
            logFile = File.Create("logs\\" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".txt");

            JSON.SetDefaultOptions(Options.ExcludeNulls);

            string metaString;

            if (File.Exists($"{ modPath}\\.metadata"))
                metaString = File.ReadAllText($"{modPath}\\.metadata");
            else if (File.Exists($"{modPath}\\_metadata"))
                metaString = File.ReadAllText($"{modPath}\\_metadata");
            else
            {
                Log("No metadata file detected.");
                Console.ReadKey();
                return;
            }

            Metadata meta = JSON.Deserialize<Metadata>(metaString);

            if (string.IsNullOrWhiteSpace(meta.steamContentId))
            {
                if (meta.author == "Chucklefish" && meta.name == "base")
                {
                    Log("Base game assets. ID is set to -1.");
                    meta.steamContentId = "-1";
                }
                else
                {
                    Log("No Steam ID detected. Press any key to exit program.");
                    Console.ReadKey();
                    return;
                }
            }
            else
                Log($"Accepted mod with Steam ID {meta.steamContentId}");

            _mod = meta.ToMod();

            Log("----------------------------------------");
            Log("Scanning and sorting mod files...");
            Log();
            ScanFiles(modPath);

            Log("----------------------------------------");
            Log("Building item and recipe lists...");
            Log();
            BuildItemList();
            BuildRecipeList();

            Log("----------------------------------------");
            Log("Converting to DB models...");
            Log();
            ConvertToDBItems();
            ConvertToDBRecipes();

            Log();
            if (_warningMessages.Count > 0)
            {
                for (int i = 0; i < _warningMessages.Count; i++)
                { Log(_warningMessages[i]); }

                Log("Warnings present. Press any key to continue...");
                Console.ReadKey();
            }
            else
                Log("No warnings, proceeding...");

            Log("----------------------------------------");
            Log("Removing old mod records from database...");
            Log();
            RemoveModFromDB(_mod.SteamId);

            Log("----------------------------------------");
            Log("Adding new records to database...");
            Log();
            AddToDatabase();
        }

        static void ScanFiles(string path)
        {
            string[] directories = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);

            for (int i = 0; i < directories.Length; i++)
                ScanFiles(directories[i]);

            for (int i = 0; i < files.Length; i++)
                SortFile(files[i]);
        }

        static void SortFile(string file)
        {
            string extension = Path.GetExtension(file);

            if (extension.Equals(".recipe"))
                _recipeFiles.Add(file);
            else if (extension.Equals(".item") || extension.Equals(".object") || extension.Equals(".activeitem") || extension.Equals(".consumable"))
                _itemFiles.Add(file);
        }

        static void BuildItemList()
        {
            for (int i = 0; i < _itemFiles.Count; i++)
            {
                Log($"Deserializing file '{_itemFiles[i]}'");

                DeserializedItem item = null;
                string json = File.ReadAllText(_itemFiles[i]).RemoveComments();

                switch (Path.GetExtension(_itemFiles[i]))
                {
                    case ".item":
                        item = JSON.Deserialize<DeserializedItem>(json);
                        item.itemType = DeserializedItem.ItemTypes.Generic;
                        break;
                    case ".object":
                        item = JSON.Deserialize<DeserializedObject>(json);
                        item.itemType = DeserializedItem.ItemTypes.Object;
                        break;
                    case ".consumable":
                        item = JSON.Deserialize<DeserializedConsumable>(json);
                        item.itemType = DeserializedItem.ItemTypes.Consumable;
                        break;
                    case ".activeitem":
                        item = JSON.Deserialize<DeserializedActiveItem>(json);
                        item.itemType = DeserializedItem.ItemTypes.ActiveItem;
                        break;
                    default:
                        AddWarning($"Not an item extension file received from '{_deserializedItems[i]}'");
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
                string json = File.ReadAllText(_recipeFiles[i]).RemoveComments();
                DeserializedRecipe recipe = JSON.Deserialize<DeserializedRecipe>(json);
                recipe.filePath = _recipeFiles[i];

                _deserializedRecipes.Add(recipe);
            }
        }

        static void ConvertToDBItems()
        {
            for (int i = 0; i < _deserializedItems.Count; i++)
            {
                DeserializedItem dItem = _deserializedItems[i];
                Log($"Working on '{dItem.itemName}'...");

                Item item = new Item
                {
                    SourceModId = _mod.SteamId,
                    ItemId = i,
                    ShortDescription = dItem.shortdescription,
                    Description = dItem.description,
                    Icon = dItem.GenerateIconImage(),
                    Price = dItem.price,
                    MaxStack = dItem.maxStack,
                    ExtraData = "",
                    Type = dItem.filePath.FilePathToItemTypeEnum(),
                    Category = dItem.category?.ToLower(),
                    Rarity = (Item.Rarities)Enum.Parse(typeof(Item.Rarities), dItem.rarity.ToLower()),
                };

                if (!dItem.SBRBhidden)
                {
                    item.InternalName = dItem.itemName;
                    item.FilePath = dItem.filePath.Split(modPath)[1];
                }

                _DBItems.Add(item);

                if (dItem is DeserializedActiveItem dActiveItem)
                {
                    var activeItem = new ActiveItemData
                    {
                        SourceModId = _mod.SteamId,
                        ItemId = item.ItemId,
                        ActiveItemDataId = _DBActiveItemDatas.Count,
                        Level = dActiveItem.level,
                        TwoHanded = dActiveItem.twoHanded,
                    };

                    item.ActiveItemDataId = activeItem.ActiveItemDataId;
                    _DBActiveItemDatas.Add(activeItem);
                }
                else if (dItem is DeserializedConsumable dconsumable)
                {
                    var consumableItem = new consumableData
                    {
                        SourceModId = _mod.SteamId,
                        ItemId = item.ItemId,
                        consumableDataId = _DBconsumableDatas.Count,
                        FoodValue = dconsumable.foodValue,
                    };

                    item.consumableDataId = consumableItem.consumableDataId;
                    _DBconsumableDatas.Add(consumableItem);
                }
                else if (dItem is DeserializedObject dObject)
                {
                    var objectItem = new ObjectData
                    {
                        SourceModId = _mod.SteamId,
                        ItemId = item.ItemId,
                        ObjectDataId = _DBObjectDatas.Count,
                        Printable = dObject.printable,
                        Race = dObject.race,
                        ColonyTags = "some tags"
                    };

                    item.ObjectDataId = objectItem.ObjectDataId;
                    _DBObjectDatas.Add(objectItem);
                }

                if (dItem.learnBlueprintsOnPickup != null)
                {
                    for (int j = 0; j < dItem.learnBlueprintsOnPickup.Length; j++)
                    {
                        string unlockedItemName = dItem.learnBlueprintsOnPickup[j];
                        if (_DBRecipeUnlocks.FirstOrDefault(u => u.UnlockedItemName == unlockedItemName &&
                                                                    u.UnlockingItemId == i &&
                                                                    u.UnlockingItemSourceModId == _mod.SteamId) != null)
                        {
                            AddWarning($"Duplicate unlock for '{unlockedItemName}' from '{dItem.itemName}' not added.\n\tItem path: '{dItem.filePath}'");
                        }
                        else
                        {
                            _DBRecipeUnlocks.Add(new RecipeUnlock
                            {
                                UnlockedItemName = unlockedItemName,
                                UnlockingItemId = i,
                                UnlockingItemSourceModId = _mod.SteamId
                            });
                        }
                    }
                }
            }
        }

        static void ConvertToDBRecipes()
        {

        }

        static void AddToDatabase()
        {
            Log();
            Log("Adding data to database...");

            using (var db = new DatabaseContext(new DbContextOptions<DatabaseContext>()))
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Mods.Add(_mod);

                foreach (var item in _DBItems)
                { db.Items.Add(item); }

                foreach (var item in _DBActiveItemDatas)
                { db.ActiveItemDatas.Add(item); }

                foreach (var item in _DBObjectDatas)
                { db.ObjectDatas.Add(item); }

                foreach (var item in _DBconsumableDatas)
                { db.ConsumableDatas.Add(item); }

                foreach (var item in _DBRecipeUnlocks)
                { db.RecipeUnlocks.Add(item); }

                var count = db.SaveChanges();
                Log("{0} records saved to database", count);
            }
        }

        static void RemoveModFromDB(int modId)
        {
            Log($"Removing mod with ID {modId}");

            using (SqlConnection connection = new SqlConnection(DatabaseContext.CONNECTION_STRING))
            using (SqlCommand command = new SqlCommand(string.Format(MOD_REMOVAL_QUERY, modId), connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                    while (reader.Read()) { };
                connection.Close();
            }
        }


        static void Log(string message)
        {
            WriteToFile(message);
            if (!silent)
                Console.WriteLine(message);
        }

        static void Log()
            => Log("\n");

        static void Log(string message, params object[] args)
            => Log(string.Format(message, args));

        static void WriteToFile(string message)
        {
            byte[] buffer = new UTF8Encoding(true).GetBytes($"\n{message}");
            logFile.Write(buffer, 0, buffer.Length);
        }

        static void AddWarning(string warning)
              => _warningMessages.Add($"WARNING - {warning}");
    }
}
