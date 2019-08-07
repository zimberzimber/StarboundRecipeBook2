using SBRB.Models;
using SBRB.Seeder.DeserializedData;
using SBRB.Seeder.Workers;
using System;
using System.Collections.Generic;

namespace SBRB.Seeder
{
    partial class Program
    {
        static List<Item> _DBItems = new List<Item>();
        static List<Recipe> _DBRecipes = new List<Recipe>();

        static void ConvertToDBItems()
        {
            for (int i = 0; i < _deserializedItems.Count; i++)
            {
                DeserializedItem dItem = _deserializedItems[i];
                Log($"Converting item '{dItem.filePath}'");

                Item item = new Item
                {
                    ID = new CompositeItemId { ItemId = (uint)i, SourceModId = _mod.SteamId },

                    Description = dItem.description,
                    ShortDescription = dItem.shortdescription,
                    Category = dItem.category,
                    Rarity = (Rarities)Enum.Parse(typeof(Rarities), dItem.rarity.ToLower()),
                    MaxStack = dItem.maxStack,
                    Price = dItem.price,
                    blueprintsOnPickup = dItem.learnBlueprintsOnPickup,
                };

                if (!dItem.SBRBhidden)
                {
                    item.InternalName = dItem.itemName;
                    item.FilePath = dItem.filePath.ToReletivePath(modPath);
                }

                _DBItems.Add(item);

                // Separate if statement for armors because their item generation has another rule
                if (dItem is DeserializedArmor dArmor)
                {
                    item.Icon = dItem.GenerateIconImage(dArmor.armorType);
                    item.ItemType = ItemTypes.Armor;
                    item.Armor = new ArmorData { ArmorType = dArmor.armorType };
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
            }
        }

        static void ConvertToDBRecipes()
        {
            for (int i = 0; i < _deserializedRecipes.Count; i++)
            {
                DeserializedRecipe dRecipe = _deserializedRecipes[i];
                Log($"Converting recipe '{dRecipe.filePath}'");
            }
        }
    }
}