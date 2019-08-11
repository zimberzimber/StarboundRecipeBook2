using SBRB.Seeder.Workers;
using SBRB_DatabaseSeeder.Workers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SBRB.Seeder
{
    partial class Program
    {
        static readonly string[] ACCEPTABLE_ITEM_EXTENSIONS = new string[]
        {   ".item", ".object", ".activeitem", ".legs", ".chest", ".head",
            ".back", ".consumable", ".beamaxe", ".flashlight", ".miningtool",
            ".harvestingtool", ".painttool", ".wiretool", ".inspectiontool",
            ".tillingtool", ".augment", ".currency", ".instrument", ".liquid",
            ".matitem", ".throwitem" };

        static List<string> _itemFiles = new List<string>();
        static List<string> _recipeFiles = new List<string>();

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
            {
                _recipeFiles.Add(file);
                Logging.Log("Found recipe file:\t{0}", file.ToReletivePath(modPath));
            }
            else if (ACCEPTABLE_ITEM_EXTENSIONS.Contains(extension))
            {
                _itemFiles.Add(file);
                Logging.Log("Found item file:\t{0}", file.ToReletivePath(modPath));
            }
        }
    }
}
