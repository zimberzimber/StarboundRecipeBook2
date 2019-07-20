﻿using Jil;
using Microsoft.EntityFrameworkCore;
using SBRB_DatabaseSeeder.DeserializedData;
using SBRB_DatabaseSeeder.Workers;
using StarboundRecipeBook2.Data;
using StarboundRecipeBook2.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

// NOTE:
// Certain edge cases may be discovered and not resolved yet.
// Look for them via Ctrl + F 'EDGE CASE' through the entire project

// NOTE:
// Raw queries seem to be faster. Should try using them when pulling data instead of throught EF core

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
        static readonly string[] ACCEPTABLE_ITEM_EXTENSIONS
            = new string[] { ".item", ".object", ".activeitem", ".legs", ".chest", ".head", ".back", ".consumable", ".beamaxe", ".lashlight", ".miningtool" };

        static bool silent = false;
        static FileStream logFile;

        static List<string> _itemFiles = new List<string>();
        static List<string> _recipeFiles = new List<string>();
        static List<DeserializedItem> _deserializedItems = new List<DeserializedItem>();
        static List<DeserializedRecipe> _deserializedRecipes = new List<DeserializedRecipe>();

        static List<Item> _DBItems = new List<Item>();
        static List<ArmorData> _DBArmorDatas = new List<ArmorData>();
        static List<ObjectData> _DBObjectDatas = new List<ObjectData>();
        static List<ActiveItemData> _DBActiveItemDatas = new List<ActiveItemData>();
        static List<ConsumableData> _DBConsumableDatas = new List<ConsumableData>();
        static List<FlashlightData> _DBFlashlightDatas = new List<FlashlightData>();
        static List<BeamaxeData> _DBBeamaxeDatas = new List<BeamaxeData>();
        static List<MiningtoolData> _DBMiningtoolDatas = new List<MiningtoolData>();
        static List<RecipeUnlock> _DBRecipeUnlocks = new List<RecipeUnlock>();
        static List<Recipe> _DBRecipes = new List<Recipe>();
        static List<RecipeInput> _DBRecipeInputs = new List<RecipeInput>();




        static List<string> _warningMessages = new List<string>();

        static void Main()
        {
            // Create a file to contain the logged messages
            Directory.CreateDirectory("logs");
            logFile = File.Create("logs\\" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".txt");

            JSON.SetDefaultOptions(Options.ExcludeNulls);

            string metaString;

            if (File.Exists($"{modPath}\\.metadata"))
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
            //RemoveModFromDB(_mod.SteamId);

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
            else if (ACCEPTABLE_ITEM_EXTENSIONS.Contains(extension))
                _itemFiles.Add(file);
        }

        static void BuildItemList()
        {
            for (int i = 0; i < _itemFiles.Count; i++)
            {
                Log($"Deserializing item '{_itemFiles[i]}'");

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
                    case ".head":
                        item = JSON.Deserialize<DeserializedArmor>(json);
                        item.itemType = DeserializedItem.ItemTypes.Armor;
                        (item as DeserializedArmor).armorType = ArmorData.ArmorType.Head;
                        break;
                    case ".chest":
                        item = JSON.Deserialize<DeserializedArmor>(json);
                        item.itemType = DeserializedItem.ItemTypes.Armor;
                        (item as DeserializedArmor).armorType = ArmorData.ArmorType.Chest;
                        break;
                    case ".legs":
                        item = JSON.Deserialize<DeserializedArmor>(json);
                        item.itemType = DeserializedItem.ItemTypes.Armor;
                        (item as DeserializedArmor).armorType = ArmorData.ArmorType.Legs;
                        break;
                    case ".back":
                        item = JSON.Deserialize<DeserializedArmor>(json);
                        item.itemType = DeserializedItem.ItemTypes.Armor;
                        (item as DeserializedArmor).armorType = ArmorData.ArmorType.Back;
                        break;
                    case ".flashlight":
                        item = JSON.Deserialize<DeserializedFlashlight>(json);
                        item.itemType = DeserializedItem.ItemTypes.Flashlight;
                        break;
                    case ".beamaxe":
                        item = JSON.Deserialize<DeserializedBeamaxe>(json);
                        item.itemType = DeserializedItem.ItemTypes.Beamaxe;
                        break;
                    case ".miningtool":
                        item = JSON.Deserialize<DeserializedMiningtool>(json);
                        item.itemType = DeserializedItem.ItemTypes.Miningtool;
                        break;
                    default:
                        AddWarning($"No handling method for item '{_itemFiles[i]}'");
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
                Log($"Deserializing recipe '{_recipeFiles[i]}'");

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
                Log($"Converting item '{dItem.filePath}'");

                Item item = new Item
                {
                    SourceModId = _mod.SteamId,
                    ItemId = i,
                    ShortDescription = dItem.shortdescription,
                    Description = dItem.description,
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
                    item.FilePath = dItem.filePath.ToReletivePath(modPath);
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
                    var consumableItem = new ConsumableData
                    {
                        SourceModId = _mod.SteamId,
                        ItemId = item.ItemId,
                        ConsumableDataId = _DBConsumableDatas.Count,
                        FoodValue = dconsumable.foodValue,
                    };

                    item.ConsumableDataId = consumableItem.ConsumableDataId;
                    _DBConsumableDatas.Add(consumableItem);
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
                else if (dItem is DeserializedFlashlight dFlashlight)
                {
                    var flaslightItem = new FlashlightData
                    {
                        SourceModId = _mod.SteamId,
                        ItemId = item.ItemId,
                        FlashlightDataID = _DBFlashlightDatas.Count,
                        BeamAmbience = dFlashlight.beamAmbience,
                        BeamLevel = dFlashlight.beamLevel,
                        LightColor = dFlashlight.lightColor.ToRGBString()
                    };

                    item.FlashlightDataId = flaslightItem.FlashlightDataID;
                    _DBFlashlightDatas.Add(flaslightItem);
                }
                else if (dItem is DeserializedBeamaxe dBeamaxe)
                {
                    var beamaxeItem = new BeamaxeData
                    {
                        SourceModId = _mod.SteamId,
                        ItemId = item.ItemId,
                        BeamaxeDataID = _DBBeamaxeDatas.Count,
                        BlockRadius = dBeamaxe.blockRadius,
                        FireTime = dBeamaxe.fireTime,
                        RangeBonus = dBeamaxe.rangeBonus,
                        TileDamage = dBeamaxe.tileDamage
                    };

                    item.BeamaxeDataId = beamaxeItem.BeamaxeDataID;
                    _DBBeamaxeDatas.Add(beamaxeItem);
                }
                else if (dItem is DeserializedMiningtool dMiningtool)
                {
                    var miningtoolItem = new MiningtoolData
                    {
                        SourceModId = _mod.SteamId,
                        ItemId = item.ItemId,
                        MiningtoolDataID = _DBMiningtoolDatas.Count,
                        BlockRadius = dMiningtool.blockRadius,
                        FireTime = dMiningtool.fireTime,
                        Durability = dMiningtool.durability,
                        DurabilityPerUse = dMiningtool.durabilityPerUse,
                        TwoHanded = dMiningtool.twoHanded
                    };

                    item.MiningtoolDataId = miningtoolItem.MiningtoolDataID;
                    _DBMiningtoolDatas.Add(miningtoolItem);
                }

                // Separate if statement here because the icon generation may be different for armors
                if (dItem is DeserializedArmor dArmor)
                {
                    var armorItem = new ArmorData
                    {
                        SourceModId = _mod.SteamId,
                        ItemId = item.ItemId,
                        Level = dArmor.level,
                        ArmorDataId = _DBArmorDatas.Count,
                        Type = dArmor.armorType
                    };

                    item.ArmorDataId = armorItem.ArmorDataId;
                    _DBArmorDatas.Add(armorItem);

                    item.Icon = dItem.GenerateIconImage(dArmor.armorType);
                }
                else
                {
                    item.Icon = dItem.GenerateIconImage();
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
            int recipeCount = 0;

            for (int i = 0; i < _deserializedRecipes.Count; i++)
            {
                DeserializedRecipe dRecipe = _deserializedRecipes[i];
                Log($"Converting recipe '{dRecipe.filePath}'");

                _DBRecipes.Add(new Recipe
                {
                    FilePath = dRecipe.filePath.ToReletivePath(modPath),
                    OutputCount = dRecipe.output.count,
                    OutputItemName = dRecipe.output.item,
                    SourceModId = _mod.SteamId,
                    RecipeId = i,
                });

                for (int j = 0; j < dRecipe.input.Length; j++)
                {
                    _DBRecipeInputs.Add(new RecipeInput
                    {
                        InputCount = dRecipe.input[j].count,
                        InputItemName = dRecipe.input[j].item,
                        RecipeId = i,
                        SourceModId = _mod.SteamId,
                        RecipeInputId = recipeCount
                    });
                    recipeCount++;
                }
            }
        }

        static void AddToDatabase()
        {
            using (var db = new DatabaseContext(new DbContextOptions<DatabaseContext>()))
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Mods.Add(_mod);

                foreach (var item in _DBItems)
                { db.Items.Add(item); }

                foreach (var item in _DBArmorDatas)
                { db.ArmorDatas.Add(item); }

                foreach (var item in _DBActiveItemDatas)
                { db.ActiveItemDatas.Add(item); }

                foreach (var item in _DBObjectDatas)
                { db.ObjectDatas.Add(item); }

                foreach (var item in _DBConsumableDatas)
                { db.ConsumableDatas.Add(item); }

                foreach (var item in _DBRecipeUnlocks)
                { db.RecipeUnlocks.Add(item); }

                foreach (var item in _DBRecipes)
                { db.Recipes.Add(item); }

                foreach (var item in _DBRecipeInputs)
                { db.RecipeInputs.Add(item); }

                foreach (var item in _DBBeamaxeDatas)
                { db.BeamaxeDatas.Add(item); }

                foreach (var item in _DBMiningtoolDatas)
                { db.MiningToolDatas.Add(item); }

                foreach (var item in _DBFlashlightDatas)
                { db.FlashlightDatas.Add(item); }

                var count = db.SaveChanges();
                Log("{0} records saved to database", count);
            }
        }

        static void RemoveModFromDB(int modId)
        {
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
