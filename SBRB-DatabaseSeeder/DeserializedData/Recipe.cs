using Jil;
using StarboundRecipeBook2.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SBRB_DatabaseSeeder.DeserializedData
{
    /*static class DeserializedRecipes
    {
        public static Dictionary<string, List<DeserializedRecipe>> _recipes = new Dictionary<string, List<DeserializedRecipe>>(0);
        public static int totalRecipes = 0;
        public static int scannedFiles = 0;

        static object _threadLockAnchor = new object();

        public static void Add(DeserializedRecipe rec)
        {
            lock (_threadLockAnchor)
            {
                scannedFiles++;

                if (!_recipes.ContainsKey(rec.output.item))
                    _recipes.Add(rec.output.item, new List<DeserializedRecipe>(0));
                _recipes[rec.output.item].Add(rec);

                totalRecipes++;
                //Groups.Add(rec.groups, rec.output.item);

                // Since the recipe is stored under the output items name, and there's only one relevant value left
                // I'm saving it into a separate value instead of keeping it inside an object
                rec.outputCount = rec.output.count;
                rec.output = null;
            }
        }

        public static void WriteToFile()
        {
            //string str = JSON.Serialize(_recipes);
            //using (StreamWriter file = new StreamWriter(string.Format("{0}{1}\\{2}\\{3}", AppDomain.CurrentDomain.BaseDirectory, Strings.OUTPUT_DIRECTORY_NAME, Strings.modName, Strings.RECIPE_BOOK_FILE_NAME)))
            //{
            //file.Write(Strings.PATCH_FILE_OPENING_LINE);
            //file.Write(str);
            //file.Write(Strings.PATCH_FILE_CLOSING_LINE);
            //}
        }
    }*/

    class DeserializedRecipe
    {
        public DeserializedInputOutput[] input;
        public DeserializedInputOutput output;
        public string[] groups;
        public int outputCount;

        /*public Recipe ToRecipe()
        {
            return new Recipe
            {
                FilePath = "",
                OutputCount = outputCount,
                OutputItem = null,
                OutputItemId = 0,
                RecipeGroups = null,
                RecipeId = 0,
                RecipeInputs = null,
                SourceMod = null,
                SourceModId = 0
            };
        }*/
    }

    class DeserializedInputOutput
    {
        string _item; // Just the data holder, explanation further down...
        public int count;

        // For whatever reason, Starbounds recipes accept "name" and "item" in the input/output fields...
        // And despite "item" being used way more often than "name", the game prioritizes the latter
        public string name { set { _item = value; } }

        // Redirect the de/serializer here. Serialization will occur normally
        // Deserialization would prefer setting _item to name as per the 'set' in here
        // It is only relevant if a JSON has both 'item', and 'name, but 'name' was read before 'item'
        // So it would follow SBs name preference rules
        public string item
        {
            get { return _item; }
            set
            {
                if (_item == default)
                    _item = value;
            }
        }
    }
}
