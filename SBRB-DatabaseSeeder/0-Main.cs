using Jil;
using SBRB_DatabaseSeeder.DeserializedData;
using StarboundRecipeBook2.MongoModels;
using System;
using System.IO;

// NOTE:
// Certain edge cases may be discovered and not resolved yet.
// Look for them via Ctrl + F 'EDGE CASE' through the entire project

// NOTE:
// Raw queries seem to be faster. Should try using them when pulling data instead of throught EF core

// Add currency
// Add thrownitem
// Add blocks
// Add liquids

// To tell whether a currency is used in a crafting recipe, first check if a currency exists with that name, and only then check items
namespace SBRB_DatabaseSeeder
{
    partial class Program
    {
        //public static string modPath = @"D:\Games\steamapps\common\Starbound\mods\Ztarbound";
        //public static string modPath = @"D:\Games\steamapps\common\Starbound\mods\_FrackinUniverse-master";
        public static string modPath = @"D:\Games\steamapps\common\Starbound\_UnpackedVanillaAssets";
        static Mod _mod;

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
    }
}
