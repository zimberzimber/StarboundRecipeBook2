using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SBRB.Models;
using System.Collections.Generic;
using System.Linq;

namespace StarboundRecipeBook2.Services
{
    public interface IRecipeRepository
    {
    }

    public class RecipeRepository : BaseRepository<Recipe>, IRecipeRepository
    {
        IItemRepository _itemRepo;

        public RecipeRepository(IItemRepository itemRepo) : base()
            => _itemRepo = itemRepo;

        IQueryable<Recipe> BaseQuery { get => _db.Recipes.AsQueryable(); }

        public List<Recipe> GetRecipesForItem(string internalItemName)
        {
            List<Item>
        }

        public List<Recipe> GetRecipesCraftedWithItem(string internalItemName)
        {
            return _context.Recipes
                           .Where(r => r.RecipeInputs.Where(ri => ri.InputItemName == internalItemName).Count() > 0)
                           .Include(r => r.RecipeInputs)
                           .ToList();
        }
    }
}
