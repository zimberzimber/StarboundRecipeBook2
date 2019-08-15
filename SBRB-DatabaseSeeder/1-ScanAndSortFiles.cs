using SBRB.Seeder.Workers;
using SBRB_DatabaseSeeder.Workers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        static ConcurrentQueue<string> _itemFiles = new ConcurrentQueue<string>();
        static ConcurrentQueue<string> _recipeFiles = new ConcurrentQueue<string>();

        static void ScanFiles(string path)
        {
            string[] directories = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);
            List<Task> tasks = new List<Task>();

            foreach (var dir in directories)
            {
                var task = new Task(() => ScanFiles(dir));
                tasks.Add(task);
                task.Start();
            }

            foreach (var file in files)
            {
                var task = new Task(() => SortFile(file));
                tasks.Add(task);
                task.Start();
            }

            Task.WaitAll(tasks.ToArray());
        }

        static void SortFile(string file)
        {
            string extension = Path.GetExtension(file);

            if (extension.Equals(".recipe"))
            {
                _recipeFiles.Enqueue(file);
                Logging.Log("Found recipe file:\t{0}", file.ToReletivePath(modPath));
            }
            else if (ACCEPTABLE_ITEM_EXTENSIONS.Contains(extension))
            {
                _itemFiles.Enqueue(file);
                Logging.Log("Found item file:\t{0}", file.ToReletivePath(modPath));
            }
        }
    }
}
