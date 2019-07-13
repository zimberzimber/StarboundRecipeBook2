using System;
using System.IO;
using static StarboundRecipeBook2.Models.Item;

namespace SBRB_DatabaseSeeder.Workers
{
    static class FileExtensionToItemType
    {
        public static ItemTypes ExtensionToItemTypeEnum(this string extension)
        {
            string lower = extension.ToLower();
            switch (lower)
            {
                case ".object":
                    return ItemTypes.objectItem;
                case ".consumable":
                    return ItemTypes.consumableItem;
                case ".activeitem":
                    return ItemTypes.activeItem;
                default:
                    return ItemTypes.genericItem;
            }
        }

        public static ItemTypes FilePathToItemTypeEnum(this string filePath)
            => Path.GetExtension(filePath).ExtensionToItemTypeEnum();
    }
}
