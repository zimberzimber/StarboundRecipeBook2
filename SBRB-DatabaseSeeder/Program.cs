using Jil;
using SBRB_DatabaseSeeder.DeserializedData;
using SBRB_DatabaseSeeder.Workers;
using StarboundRecipeBook2.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace SBRB_DatabaseSeeder
{
    class Program
    {
        public static string modPath = @"D:\Games\steamapps\common\Starbound\mods\Ztarbound";
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

            DeserializedItem asd = JSON.Deserialize<DeserializedItem>(File.ReadAllText(@"D:\Games\steamapps\common\Starbound\mods\_FrackinUniverse-master\items\active\weapons\other\drillspear\drillspear.activeitem"));
            string path = asd.GetIconPath();

            string metaString;

            if (File.Exists($"{modPath}\\.metadata"))
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
            ConvertToDBItems();

            Console.WriteLine("\tItems:");
            _deserializedItems.ForEach(i => Console.WriteLine($"{i.itemType.ToString()} - {i.itemName}"));

            var z = 5;
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
                        item.itemType = DeserializedItem.ItemType.Generic;
                        break;
                    case ".object":
                        item = JSON.Deserialize<DeserializedObject>(json);
                        item.itemType = DeserializedItem.ItemType.Object;
                        break;
                    case ".consumeable":
                        item = JSON.Deserialize<DeserializedConsumeable>(json);
                        item.itemType = DeserializedItem.ItemType.Consumeable;
                        break;
                    case ".activeitem":
                        item = JSON.Deserialize<DeserializedActiveItem>(json);
                        item.itemType = DeserializedItem.ItemType.ActiveItem;
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
                var dItem = _deserializedItems[i];
                byte[] iconByteArray;

                using (FileStream fs = new FileStream(dItem.GetIconPath(), FileMode.Open))
                using (BinaryReader reader = new BinaryReader(fs))
                { iconByteArray = reader.ReadBytes((int)reader.BaseStream.Length); }

                using (FileStream fs = new FileStream(Path.GetFileNameWithoutExtension(dItem.filePath) + ".png", FileMode.Create))
                using (BinaryWriter writer = new BinaryWriter(fs))
                { writer.Write(iconByteArray); }

                Item item = new Item
                {
                    InternalName = dItem.itemName,
                    ShortDescription = dItem.shortdescription,
                    Description = dItem.shortdescription,
                    //Icon = iconByteArray,
                    Price = dItem.price,
                    MaxStack = dItem.maxStack,
                    ExtraData = "",
                    FilePath = dItem.filePath,
                    FileExtension = Path.GetExtension(dItem.filePath),
                    RarityName = dItem.rarity,

                    SourceModId = _mod.SteamId,
                };
                _DBitems.Add(item);

                if (dItem is DeserializedActiveItem dActiveItem)
                {
                    var activeItem = new ActiveItemData
                    {
                        Level = dActiveItem.level,
                        TwoHanded = dActiveItem.twoHanded,
                        ItemName = dActiveItem.itemName
                    };

                    item.ActiveItemData = activeItem;
                    activeItem.Item = item;

                    _DBActiveItemDatas.Add(activeItem);
                }
                else if (dItem is DeserializedConsumeable dConsumeable)
                {
                    var consumeableItem = new ConsumeableData
                    {
                        ItemName = item.InternalName,
                        FoodValue = dConsumeable.foodValue
                    };

                    item.ConsumeableData = consumeableItem;
                    consumeableItem.Item = item;

                    _DBConsumeableDatas.Add(consumeableItem);
                }
                else if (dItem is DeserializedObject dObject)
                {
                    var objectItem = new ObjectData
                    {
                        ItemName = item.InternalName,
                        Printable = dObject.printable,
                        Race = dObject.race,
                        ColonyTags = null
                    };

                    item.ObjectData = objectItem;
                    objectItem.Item = item;

                    _DBObjectDatas.Add(objectItem);
                }
            }
        }
    }
}
