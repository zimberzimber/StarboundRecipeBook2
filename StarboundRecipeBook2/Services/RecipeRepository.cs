using Microsoft.EntityFrameworkCore;
using StarboundRecipeBook2.Data;
using StarboundRecipeBook2.Models;
using System.Collections.Generic;
using System.Linq;

namespace StarboundRecipeBook2.Services
{
    public interface IRecipeRepository
    {
        /// <summary>Get a list of recipes that create the given item.</summary>
        /// <param name="internalItemName">Internal item name</param>
        List<Recipe> GetRecipesForItem(string internalItemName);

        /// <summary>Get a list of recipes that use the given item.</summary>
        /// <param name="internalItemName">Internal item name</param>
        List<Recipe> GetRecipesCraftedWithItem(string internalItemName);
    }

    public class RecipeRepository : IRecipeRepository
    {
        DatabaseContext _context;

        public RecipeRepository(DatabaseContext context)
        { _context = context; }

        public List<Recipe> GetRecipesForItem(string internalItemName)
            => _context.Recipes.Where(recipe => recipe.OutputItemName == internalItemName).Include(r => r.RecipeInputs).ToList();

        public List<Recipe> GetRecipesCraftedWithItem(string internalItemName)
        {
            return _context.Recipes
                           .Where(r => r.RecipeInputs.Where(ri => ri.InputItemName == internalItemName).Count() > 0)
                           .Include(r => r.RecipeInputs)
                           .ToList();
        }
    }
}
