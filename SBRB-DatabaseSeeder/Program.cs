using Jil;
using Microsoft.EntityFrameworkCore;
using SBRB_DatabaseSeeder.DeserializedData;
using SBRB_DatabaseSeeder.Workers;
using StarboundRecipeBook2.Data;
using StarboundRecipeBook2.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// NOTE:
// Certain edge cases may be discovered and not resolved yet.
// Look for them via Ctrl + F 'EDGE CASE' through the entire project

namespace SBRB_DatabaseSeeder
{
    class Program
    {
        public static string modPath = @"D:\Games\steamapps\common\Starbound\mods\Ztarbound";
        //public static string modPath = @"D:\Games\steamapps\common\Starbound\mods\_FrackinUniverse-master";
        static Mod _mod;

        static List<string> _itemFiles = new List<string>();
        static List<string> _recipeFiles = new List<string>();
        static List<DeserializedItem> _deserializedItems = new List<DeserializedItem>();
        static List<DeserializedRecipe> _recipes = new List<DeserializedRecipe>();

        static List<Item> _DBitems = new List<Item>();
        static List<ObjectData> _DBObjectDatas = new List<ObjectData>();
        static List<ActiveItemData> _DBActiveItemDatas = new List<ActiveItemData>();
        static List<ConsumeableData> _DBConsumeableDatas = new List<ConsumeableData>();

        static void Main()
        {
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
                Console.WriteLine("No Steam ID detected.");
                Console.ReadKey();
                return;
            }

            _mod = meta.ToMod();

            ScanFiles(modPath);
            BuildItemList();

            Console.WriteLine();
            Console.WriteLine("\tItems Scanned:");
            _deserializedItems.ForEach(i => Console.WriteLine($"{i.itemType.ToString()} - {i.itemName}"));
            Console.WriteLine();

            ConvertToDBItems();
            AddToDatabase();
            temp();
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
            else if (extension.Equals(".item") || extension.Equals(".object") || extension.Equals(".activeitem") || extension.Equals(".consumeable"))
                _itemFiles.Add(file);

            if (Path.GetExtension(file) == ".recipe")
            {
                //var x = JSON.Deserialize<DeserializedRecipe>(File.ReadAllText(file).RemoveComments());
            }
        }

        static void BuildItemList()
        {
            for (int i = 0; i < _itemFiles.Count; i++)
            {
                DeserializedItem item;
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
                    case ".consumeable":
                        item = JSON.Deserialize<DeserializedConsumeable>(json);
                        item.itemType = DeserializedItem.ItemTypes.Consumeable;
                        break;
                    case ".activeitem":
                        item = JSON.Deserialize<DeserializedActiveItem>(json);
                        item.itemType = DeserializedItem.ItemTypes.ActiveItem;
                        break;
                    default:
                        throw new Exception($"Not an item extension file received from '{_deserializedItems[i]}'");
                }

                item.filePath = _itemFiles[i];
                _deserializedItems.Add(item);
            }
        }

        static void ConvertToDBItems()
        {
            for (int i = 0; i < _deserializedItems.Count; i++)
            {
                DeserializedItem dItem = _deserializedItems[i];

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

                _DBitems.Add(item);

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
                else if (dItem is DeserializedConsumeable dConsumeable)
                {
                    var consumeableItem = new ConsumeableData
                    {
                        SourceModId = _mod.SteamId,
                        ItemId = item.ItemId,
                        ConsumeableDataId = _DBConsumeableDatas.Count,
                        FoodValue = dConsumeable.foodValue,
                    };

                    item.ConsumeableDataId = consumeableItem.ConsumeableDataId;
                    _DBConsumeableDatas.Add(consumeableItem);
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
            }
        }

        static void AddToDatabase()
        {
            using (var db = new DatabaseContext(new DbContextOptions<DatabaseContext>()))
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Mods.Add(_mod);

                foreach (var item in _DBitems)
                { db.Items.Add(item); }

                foreach (var item in _DBActiveItemDatas)
                { db.ActiveItemDatas.Add(item); }

                foreach (var item in _DBObjectDatas)
                { db.ObjectDatas.Add(item); }

                foreach (var item in _DBConsumeableDatas)
                { db.ConsumeableDatas.Add(item); }

                var count = db.SaveChanges();
                Console.WriteLine("{0} records saved to database", count);
            }
        }

        static void temp()
        {
            using (var db = new DatabaseContext(new DbContextOptions<DatabaseContext>()))
            {
                List<Mod> mods = db.Mods.Include(m => m.AddedItems).ToList();
                List<Item> items = db.Items.Include(i => i.ActiveItemData).Include(i => i.ConsumeableData).Include(i => i.ObjectData).ToList();
                List<Item> items2 = db.Items.Where(r => r.Rarity.ToString() == "common").ToList();
                var breakpoint = 5;
            }
        }
    }
}
