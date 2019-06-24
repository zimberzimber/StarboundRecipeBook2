using System.Collections;
using System.IO;

namespace SBRB_DatabaseSeeder.DeserializedData
{
    partial class DeserializedItem
    {
        public enum ItemType { Generic, Object, Consumeable, ActiveItem };

        public string itemName { get; set; }
        public string shortdescription { get; set; }
        public string description { get; set; }
        public string rarity { get; set; }
        public string category { get; set; }
        public int price { get; set; }
        public int maxStack { get; set; }
        public string tooltipKind { get; set; }
        public dynamic inventoryIcon { get; set; }

        public ItemType itemType { get; set; }
        public string filePath { get; set; }

        public string GetIconPath()
        {
            //if (inventoryIcon is string iconPath)
            if (inventoryIcon.ToString().Contains("[{")) // Collection of paths
            {

            }
            else // Single path
            {
                string path = inventoryIcon.ToString().Replace("\"", string.Empty);
                // Path from root
                if (path.StartsWith('/'))
                    return $"{Program.modPath}\\{path.Replace('/', '\\')}";

                // Reletive path
                else
                    return $"{Path.GetDirectoryName(filePath)}\\{path}";
            }
            return "";
        }
    }

    class DeserializedObject : DeserializedItem
    {
        public string race { get; set; }
        public bool printable { get; set; }
        public string[] colonyTags { get; set; }

        // Objects use 'objectName' instead of 'itemName'
        public string objectName { get => itemName; set => itemName = value; }
    }

    class DeserializedConsumeable : DeserializedItem
    {
        public int foodValue { get; set; }
    }

    class DeserializedActiveItem : DeserializedItem
    {
        public double level { get; set; }
        public bool twoHanded { get; set; }
        public string elementalType { get; set; }
    }
}
