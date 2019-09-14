using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SBRB.Models;
using System.Collections.Generic;
using System.Linq;

namespace StarboundRecipeBook2.Services
{
    public interface IRecipeRepository : IBaseRepository<Recipe>
    {
        /// <summary>
        /// Get a list of recipes which craft the given item
        /// </summary>
        /// <param name="internalItemName">Internal item name to search by</param>
        /// <returns>List of recipes crafting the item</returns>
        List<Recipe> GetRecipesForItem(string internalItemName);

        /// <summary>
        /// Get a list of recipes which are crafted with the given item
        /// </summary>
        /// <param name="internalItemName">Internal item name to search by</param>
        /// <returns>List of recipes crafted with the item</returns>
        List<Recipe> GetRecipesCraftedWithItem(string internalItemName);
    }

    public class RecipeRepository : BaseRepository<Recipe>, IRecipeRepository
    {
        public override IQueryable<Recipe> BaseQuery => _db.Recipes.AsQueryable();

        public List<Recipe> GetRecipesForItem(string internalItemName)
            => BaseQuery.Where(r => r.OutputItemName == internalItemName).ToList();

        public List<Recipe> GetRecipesCraftedWithItem(string internalItemName)
            => BaseQuery.Where(r => r.Inputs.FirstOrDefault(i => i.ItemName == internalItemName) != null).ToList();
    }
}
