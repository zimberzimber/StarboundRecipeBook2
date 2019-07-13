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
            if (false)
            {
                RemoveModFromDB(729480149);
                return;
            }

            JSON.SetDefaultOptions(Options.ExcludeNulls);

            string metaString;

            if (File.Exists($"{ modPath}\\.metadata"))
                metaString = File.ReadAllText($"{modPath}\\.metadata");
            else if (File.Exists($"{modPath}\\_metadata"))
                metaString = File.ReadAllText($"{modPath}\\_metadata");
            else
            {
                Console.WriteLine("No metadata file detected.");
                Console.ReadKey();
                return;
            }

            Metadata meta = JSON.Deserialize<Metadata>(metaString);

            if (string.IsNullOrEmpty(meta.steamContentId))
            {
                if (meta.author == "Chucklefish" && meta.name == "base")
                {
                    Console.WriteLine("Base game assets. ID is set to -1.");
                    meta.steamContentId = "-1";
                }
                else
                {
                    Console.WriteLine("No Steam ID detected.");
                    Console.ReadKey();
                    return;
                }
            }

            _mod = meta.ToMod();

            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Scanning and sorting mod files...");
            Console.WriteLine();
            ScanFiles(modPath);

            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Building item and recipe lists...");
            Console.WriteLine();
            BuildItemList();
            BuildRecipeList();

            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Converting to DB models...");
            Console.WriteLine();
            ConvertToDBItems();
            ConvertToDBRecipes();

            Console.WriteLine();
            if (_warningMessages.Count > 0)
            {
                for (int i = 0; i < _warningMessages.Count; i++)
                { Console.WriteLine(_warningMessages[i]); }

                Console.WriteLine("Warnings present. Press any key to continue...");
                Console.ReadKey();
            }
            else
                Console.WriteLine("No warnings, proceeding...");

            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Adding to database...");
            Console.WriteLine();
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
                Console.WriteLine($"Deserializing file '{_itemFiles[i]}'");

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
                Console.WriteLine($"Working on '{dItem.itemName}'...");

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
            Console.WriteLine();
            Console.WriteLine("Adding data to database...");

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
                Console.WriteLine("{0} records saved to database", count);
            }
        }

        static void RemoveModFromDB(int modId)
        {
            Console.WriteLine();
            Console.WriteLine($"Removing mod with ID {modId}");

            using (SqlConnection connection = new SqlConnection(DatabaseContext.CONNECTION_STRING))
            using (SqlCommand command = new SqlCommand(string.Format(MOD_REMOVAL_QUERY, modId), connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.WriteLine(reader.GetValue(i));
                        }
                        Console.WriteLine();
                    }
                };
                connection.Close();
            }
        }

        static void AddWarning(string warning)
              => _warningMessages.Add($"WARNING - {warning}");
    }
}
