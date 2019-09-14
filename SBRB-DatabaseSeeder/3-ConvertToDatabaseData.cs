using SBRB.Models;
using SBRB.Seeder.DeserializedData;
using SBRB.Seeder.Extensions;
using SBRB_DatabaseSeeder.Extensions;
using SBRB_DatabaseSeeder.Workers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SBRB.Seeder
{
    partial class Program
    {
        // Queues containing the database appropriate elements
        static ConcurrentQueue<Item> _DBItems = new ConcurrentQueue<Item>();
        static ConcurrentQueue<Recipe> _DBRecipes = new ConcurrentQueue<Recipe>();

        // Locker ojects for a thread safe environment
        static object itemLocker = new object();
        static object recipeLocker = new object();

        // Variables used to determine the next free id for an element
        // The id is part of the item/recipe primary key/_id (the other part being their source mod steam id)
        // Items use them because in Starbound, an object and another item may use the same internal name
        // Recipes use them because there's nothing else to identify them by
        static uint nextItemId = 0;
        static uint nextRecipeId = 0;

        /// <summary>
        /// Initialize the proccess of converting classes into the database appropriate class.
        /// </summary>
        static void ConvertToDBData()
        {
            // Create a list to store all the conversion tasks
            List<Task> tasks = new List<Task>();

            // Create, index, and start an item converting task for each item in the deserialized item list
            foreach (var item in _deserializedItems)
            {
                var task = new Task(() => ConvertItem(item));
                tasks.Add(task);
                task.Start();
            }

            // Create, index, and start a recipe converting task for each recipe in the deserialized recipe list
            foreach (var recipe in _deserializedRecipes)
            {
                var task = new Task(() => ConvertRecipe(recipe));
                tasks.Add(task);
                task.Start();
            }

            // Wait for the conversion tasks to complete
            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Convert an item class into the database appropriate variant.
        /// </summary>
        /// <param name="dItem">The item to convert</param>
        static void ConvertItem(DeserializedItem dItem)
        {
            _logger.Log("Converting item: {0}", dItem.filePath.TrimPath(modPath));

            // Create the database appropriate item
            Item item = new Item { ID = new CompositeItemId { SourceModId = _mod.SteamId } };

            // Set the ItemID in a thread safe environment
            // Don't want situations where multiple threads read the nextItemId value at the same time
            lock (itemLocker)
            {
                item.ID.ItemId = nextItemId;
                nextItemId++;
            }

            // Set the data
            item.Description = dItem.description;
            item.ShortDescription = dItem.shortdescription?.RemoveFormatting();
            item.Category = dItem.category;
            item.Rarity = (Rarities)Enum.Parse(typeof(Rarities), dItem.rarity.ToLower());
            item.MaxStack = dItem.maxStack;
            item.Price = dItem.price;
            item.blueprintsOnPickup = dItem.learnBlueprintsOnPickup;

            // Add sensetive data if the item shouldn't be hidden
            if (!dItem.SBRBhidden)
            {
                item.InternalName = dItem.itemName;
                item.FilePath = dItem.filePath.TrimPath(modPath);
            }

            // Add item type specific data to the item
            // Separate if statement for armors because their item icon generation has an extra rule
            if (dItem is DeserializedArmor dArmor)
            {
                // Generate the icon through a separate method
                item.Icon = dItem.GenerateIconImage(dArmor.armorType);

                item.ItemType = ItemTypes.Armor;
                item.Armor = new ArmorData
                {
                    ArmorType = dArmor.armorType
                };
            }
            else
            {
                // Generate the icon through a separate method
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

            // Enqueue the recipe, to be added into the database later
            _DBItems.Enqueue(item);
        }

        /// <summary>
        /// Convert a recipe class into the database appropriate variant.
        /// </summary>
        /// <param name="dRecipe">The recipe to convert</param>
        static void ConvertRecipe(DeserializedRecipe dRecipe)
        {
            _logger.Log("Converting recipe: {0}", dRecipe.filePath.TrimPath(modPath));

            // Create the database appropriate recipe
            Recipe recipe = new Recipe { ID = new CompositeRecipeId { SourceModId = _mod.SteamId } }; ;


            // Set the RecipeID in a thread safe environment
            // Don't want situations where multiple threads read the nextRecipeId value at the same time
            lock (recipeLocker)
            {
                recipe.ID.RecipeId = nextRecipeId;
                nextRecipeId++;
            }

            // Set the data
            recipe.FilePath = dRecipe.filePath;
            recipe.OutputCount = dRecipe.output.count;
            recipe.OutputItemName = dRecipe.output.item;
            recipe.RecipeGroups = dRecipe.groups;

            // Add the inputs
            recipe.Inputs = new RecipeInputs[dRecipe.input.Length];
            for (int i = 0; i < dRecipe.input.Length; i++)
            {
                recipe.Inputs[i] = new RecipeInputs
                {
                    Count = dRecipe.input[i].count,
                    ItemName = dRecipe.input[i].item
                };
            }

            // Enqueue the recipe, to be added into the database later
            _DBRecipes.Enqueue(recipe);
        }
    }
}