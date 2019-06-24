using SBRB_DatabaseSeeder.DeserializedData;
using StarboundRecipeBook2.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SBRB_DatabaseSeeder.Workers
{
    static class RecipeDeserializer
    {
        static object _threadLockAnchor = new object();

        public static void AddRecipe(this List<DeserializedRecipe> recipeList, DeserializedRecipe recipe)
        {
            lock (_threadLockAnchor)
            {
                recipeList.Add(recipe);

                // Since the recipe is stored under the output items name, and there's only one relevant value left
                // I'm saving it into a separate value instead of keeping it inside an object
                recipe.outputCount = recipe.output.count;
                recipe.output = null;
            }
        }
    }
}
