using Jil;
using SBRB.Models;
using SBRB.Seeder.DeserializedData;
using SBRB_DatabaseSeeder.Workers;
using System;
using System.IO;

// NOTE:
// Certain edge cases may be discovered and not resolved yet.
// Look for them via Ctrl + F 'EDGE CASE' through the entire project

// Add currency

// To tell whether a currency is used in a crafting recipe, first check if a currency exists with that name, and only then check items
namespace SBRB.Seeder
{
    partial class Program
    {
        //public static string modPath = @"D:\Games\steamapps\common\Starbound\mods\Ztarbound";
        public static string modPath = @"D:\Games\steamapps\common\Starbound\mods\_FrackinUniverse-master";
        //public static string modPath = @"D:\Games\steamapps\common\Starbound\_UnpackedVanillaAssets";
        static Mod _mod;
        const string BASE_GAME_ASSETS_STEAM_ID = "0";

        static void Main(string[] args)
        {
            //for (int i = 0; i < args.Length; i++)
            //{ Console.WriteLine(args[i]);}

            JSON.SetDefaultOptions(Options.ExcludeNulls);
            string metaString;

            try
            {
                if (File.Exists($"{modPath}\\.metadata"))
                    metaString = File.ReadAllText($"{modPath}\\.metadata");
                else if (File.Exists($"{modPath}\\_metadata"))
                    metaString = File.ReadAllText($"{modPath}\\_metadata");
                else
                {
                    Logging.Log("No metadata file detected.");
                    Logging.Log("Press any key to exit...");
                    Console.ReadKey();
                    return;
                }

                Metadata meta = JSON.Deserialize<Metadata>(metaString);

                if (string.IsNullOrWhiteSpace(meta.steamContentId))
                {
                    if (meta.author == "Chucklefish" && meta.name == "base")
                    {
                        Logging.Log("Base game assets. ID is set to {0}.", BASE_GAME_ASSETS_STEAM_ID);
                        meta.steamContentId = BASE_GAME_ASSETS_STEAM_ID;
                    }
                    else
                    {
                        Logging.Log("No Steam ID detected. Press any key to exit program.");
                        Console.ReadKey();
                        return;
                    }
                }
                else
                    Logging.Log($"Accepted mod with Steam ID {meta.steamContentId}");
                Logging.Log();

                _mod = meta.ToMod();

                Logging.Log("----------------------------------------");
                Logging.Log("Scanning and sorting mod files...");
                Logging.Log();
                ScanFiles(modPath);

                Logging.Log("----------------------------------------");
                Logging.Log("Building item and recipe lists...");
                Logging.Log();
                BuildQueues();

                Logging.Log("----------------------------------------");
                Logging.Log("Converting to DB models...");
                Logging.Log();
                ConvertToDBData();

                Logging.Log("----------------------------------------");
                bool hasWarnings = Logging.PrintWarnings();
                if (hasWarnings)
                {
                    Logging.Log("Warnings present. Press any key to continue...");
                    Console.ReadKey();
                }
                else
                    Logging.Log("No warnings, proceeding...");
                Logging.Log();

                Logging.Log("----------------------------------------");
                Logging.Log("Creating database connection...");
                Logging.Log();
                GetDatabaseConnection();

                Logging.Log("----------------------------------------");
                RemoveModFromDB(_mod.SteamId);
                Logging.Log();

                Logging.Log("----------------------------------------");
                AddToDatabase();
                Logging.Log();

                Logging.Log("----------------------------------------");
                Logging.Log("All done!");
            }
            catch (Exception e)
            {
                Logging.Log("An error has occured:");
                Logging.Log(e.Message);
            }
            finally
            {
                Logging.StopLogging();
            }

            Logging.Log("Press any key to exit the progmram...");
            Console.ReadKey();
        }
    }
}
