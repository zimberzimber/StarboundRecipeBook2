using SBRB.Models;
using SBRB.Seeder.DeserializedData;
using SBRB.Seeder.Workers;
using SBRB_DatabaseSeeder.Workers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SBRB.Seeder
{
    partial class Program
    {
        static ConcurrentQueue<Item> _DBItems = new ConcurrentQueue<Item>();
        static ConcurrentQueue<Recipe> _DBRecipes = new ConcurrentQueue<Recipe>();

        static object itemLocker = new object();
        static object recipeLocker = new object();
        static uint nextItemId = 0;
        static uint nextRecipeId = 0;

        static void ConvertToDBData()
        {
            List<Task> tasks = new List<Task>();

            foreach (var item in _deserializedItems)
            {
                var task = new Task(() => ConvertItem(item));
                tasks.Add(task);
                task.Start();
            }

            foreach (var recipe in _deserializedRecipes)
            {
                var task = new Task(() => ConvertRecipe(recipe));
                tasks.Add(task);
                task.Start();
            }

            Task.WaitAll(tasks.ToArray());
        }

        static void ConvertItem(DeserializedItem dItem)
        {
            Logging.Log("Converting item: {0}", dItem.filePath.ToReletivePath(modPath));
            Item item;

            lock (itemLocker)
            {
                item = new Item
                { ID = new CompositeItemId { SourceModId = _mod.SteamId, ItemId = nextItemId } };
                nextItemId++;
            }

            item.Description = dItem.description;
            item.ShortDescription = dItem.shortdescription;
            item.Category = dItem.category;
            item.Rarity = (Rarities)Enum.Parse(typeof(Rarities), dItem.rarity.ToLower());
            item.MaxStack = dItem.maxStack;
            item.Price = dItem.price;
            item.blueprintsOnPickup = dItem.learnBlueprintsOnPickup;

            if (!dItem.SBRBhidden)
            {
                item.InternalName = dItem.itemName;
                item.FilePath = dItem.filePath.ToReletivePath(modPath);
            }

            // Separate if statement for armors because their item generation has another rule
            if (dItem is DeserializedArmor dArmor)
            {
                item.Icon = dItem.GenerateIconImage(dArmor.armorType);
                item.ItemType = ItemTypes.Armor;
                item.Armor = new ArmorData
                {
                    ArmorType = dArmor.armorType
                };
            }
            else
            {
                item.Icon = dItem.GenerateIconImage();

                if (dItem is DeserializedActiveItem dActiveItem)
                {
                    item.ItemType = ItemTypes.ActiveItem;
                    item.TwoHanded = dActiveItem.twoHanded;
                    item.Level = dActiveItem.level;
                }
                else if (dItem is DeserializedConsumable dConsumable)
                {
                    item.ItemType = ItemTypes.Consumable;
                    item.Consumable = new ConsumableData
                    {
                        FoodValue = dConsumable.foodValue,
                    };
                }
                else if (dItem is DeserializedObject dObject)
                {
                    item.ItemType = ItemTypes.Object;
                    item.Object = new ObjectData
                    {
                        ColonyTags = dObject.colonyTags,
                        Printable = dObject.printable,
                        Race = dObject.race
                    };
                }
                else if (dItem is DeserializedFlashlight dFlashlight)
                {
                    item.ItemType = ItemTypes.Flashlight;
                    item.Flashlight = new FlashlightData
                    {
                        BeamAmbience = dFlashlight.beamAmbience,
                        BeamLevel = dFlashlight.beamLevel,
                        LightColor = dFlashlight.lightColor.ToRGBString(),
                    };
                }
                else if (dItem is DeserializedTool dTool)
                {
                    item.ItemType = ItemTypes.Tool;
                    item.Tool = new ToolData
                    {
                        BlockRadius = dTool.blockRadius,
                        FireTime = dTool.fireTime,
                        RangeBonus = dTool.rangeBonus,
                        TileDamage = dTool.tileDamage,
                        ToolType = dTool.ToolType,
                    };
                }
                else if (dItem is DeserializedAugment dAugment)
                {
                    item.ItemType = ItemTypes.Augment;

                    if (dAugment.augment != null)
                    {
                        item.Augment = new AugmentData
                        {
                            DisplayName = dAugment.augment.displayName,
                            Name = dAugment.augment.name,
                            Type = dAugment.augment.type,
                        };
                    }
                }
                else if (dItem is DeserializedCurrencyItem dCurrencyItem)
                {
                    item.ItemType = ItemTypes.CurrencyItem;
                    item.Currency = new CurrencyData
                    {
                        CurrencyName = dCurrencyItem.currency,
                        CurrencyValue = dCurrencyItem.value,
                    };
                }
                else if (dItem is DeserializedMaterialItem dMaterial)
                {
                    item.ItemType = ItemTypes.Material;
                    item.Material = new MaterialData
                    {
                        MaterialId = dMaterial.materialId
                    };
                }
                else if (dItem is DeserializedLiquidItem dLiquid)
                {
                    item.ItemType = ItemTypes.Liquid;
                    item.Liquid = new LiquidData
                    {
                        Liquid = dLiquid.liquid
                    };
                }
                else if (dItem is DeserializedInstrument dInstrument)
                {
                    item.ItemType = ItemTypes.Instrument;
                }
            }

            _DBItems.Enqueue(item);
        }

        static void ConvertRecipe(DeserializedRecipe dRecipe)
        {
            Logging.Log("Converting recipe: {0}", dRecipe.filePath.ToReletivePath(modPath));
            Recipe recipe;

            lock (recipeLocker)
            {
                recipe = new Recipe
                { ID = new CompositeRecipeId { SourceModId = _mod.SteamId, RecipeId = nextRecipeId } };
                nextRecipeId++;
            }

            recipe.FilePath = dRecipe.filePath;
            recipe.OutputCount = dRecipe.output.count;
            recipe.OutputItemName = dRecipe.output.item;
            recipe.RecipeGroups = dRecipe.groups;

            recipe.Inputs = new RecipeInputs[dRecipe.input.Length];
            for (int i = 0; i < dRecipe.input.Length; i++)
            {
                recipe.Inputs[i] = new RecipeInputs
                {
                    Count = dRecipe.input[i].count,
                    ItemName = dRecipe.input[i].item
                };
            }

            _DBRecipes.Enqueue(recipe);
        }
    }
}