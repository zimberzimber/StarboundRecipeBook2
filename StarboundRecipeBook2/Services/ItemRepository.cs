using Microsoft.EntityFrameworkCore;
using StarboundRecipeBook2.Data;
using StarboundRecipeBook2.Helpers;
using StarboundRecipeBook2.Models;
using System.Linq;

namespace StarboundRecipeBook2.Services
{
    public interface IItemRepository
    {
        /// <summary>Get an item based on ID and source mod ID.</summary>
        /// <param name="itemId">Items ID</param>
        /// <param name="sourceModId">Source mods ID</param>
        /// <param name="includeMod">Whether the source mod should be included</param>
        /// <param name="includeRecipes">Whether recipes should be included</param>
        /// <param name="includeUnlocks">Whether unlocks should be included</param>
        /// <returns></returns>
        Item GetItem(int itemId, int sourceModId, bool includeMod = false, bool includeRecipes = false, bool includeUnlocks = false);

        /// <summary>Get a queriable list of all the items.</summary>
        /// <param name="skip">Number of items to skip over</param>
        /// <param name="count">Number of items to take</param>
        /// <param name="includeMod">Whether the source mod should be included</param>
        /// <param name="includeRecipes">Whether recipes should be included</param>
        /// <param name="includeUnlocks">Whether unlocks should be included</param>
        /// <returns></returns>
        IQueryable<Item> GetAllItems(int skip = 0, int count = int.MaxValue, bool includeMod = false, bool includeRecipes = false, bool includeUnlocks = false);
    }

    public class ItemRepository : IItemRepository
    {
        DatabaseContext _context;

        public ItemRepository(DatabaseContext context)
        { _context = context; }

        public Item GetItem(int itemId, int sourceModId, bool includeMod = false, bool includeRecipes = false, bool includeUnlocks = false)
        {
            return GetAllItems(includeMod: includeMod, includeRecipes: includeRecipes, includeUnlocks: includeUnlocks)
                .FirstOrDefault(i => i.ItemId == itemId && i.SourceModId == sourceModId);
        }

        public IQueryable<Item> GetAllItems(int skip = 0, int count = int.MaxValue, bool includeMod = false, bool includeRecipes = false, bool includeUnlocks = false)
        {
            return _context.Items
                .Include(i => i.ObjectData)
                .Include(i => i.ConsumeableData)
                .Include(i => i.ActiveItemData)
                .If(includeMod, i => i.Include(i2 => i2.SourceMod))
                .If(includeRecipes, i => i.Include(i2 => i2.RecipesUsedIn).Include(i2 => i2.RecipesCraftedFrom))
                .If(includeUnlocks, i => i.Include(i2 => i2.UnlockedBy).Include(i2 => i2.Unlocks))
                .Skip(skip)
                .Take(count);
        }
    }
}
