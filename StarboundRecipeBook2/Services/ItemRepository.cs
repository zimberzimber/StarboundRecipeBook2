using Microsoft.EntityFrameworkCore;
using StarboundRecipeBook2.Data;
using StarboundRecipeBook2.Helpers;
using StarboundRecipeBook2.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarboundRecipeBook2.Services
{
    [Flags]
    public enum ItemIncludeOptions : byte
    {
        None = 0,
        Mods = 1,
        Unlocks = 2,
        All = 3
    };

    [Flags]
    public enum ItemSearchOptions : byte
    {
        None = 0,
        Generic = 1,
        Object = 2,
        consumable = 4,
        ActiveItem = 8,
        All = 15
    }

    public interface IItemRepository
    {
        /// <summary>Get an item based on ID and source mod ID.</summary>
        /// <param name="itemId">Items ID</param>
        /// <param name="sourceModId">Source mods ID</param>
        /// <param name="options">Include options for the query</param>
        /// <returns></returns>
        Item GetItemByIds(int itemId, int sourceModId, ItemIncludeOptions options = ItemIncludeOptions.None);

        /// <summary>Get a queriable of items matching the short description (visible name)</summary>
        /// <param name="shortDescription">Short description to search by</param>
        /// <param name="partialMatch">Whether the names must fully match (excluding case)</param>
        /// <param name="options">Include options for the query</param>
        /// <returns></returns>
        List<Item> GetItemsByShortDescription(string shortDescription, bool partialMatch = false, ItemIncludeOptions options = ItemIncludeOptions.None, ItemSearchOptions searchOptions = ItemSearchOptions.All);

        /// <summary>Get a queriable of items matching the internal name</summary>
        /// <param name="internalName">Internal name to search by</param>
        /// <param name="partialMatch">Whether the names must fully match (excluding case)</param>
        /// <param name="options">Include options for the query</param>
        /// <returns></returns>
        List<Item> GetItemsByInternalName(string internalName, bool partialMatch = false, ItemIncludeOptions options = ItemIncludeOptions.None, ItemSearchOptions searchOptions = ItemSearchOptions.All);

        /// <summary>Get a queriable list of all the items.</summary>
        /// <param name="skip">Number of items to skip over</param>
        /// <param name="count">Number of items to take</param>
        /// <param name="options">Include options for the query</param>
        /// <returns></returns>
        List<Item> GetAllItems(int skip = 0, int count = int.MaxValue, ItemIncludeOptions options = ItemIncludeOptions.None);
    }

    public class ItemRepository : IItemRepository
    {
        DatabaseContext _context;

        public ItemRepository(DatabaseContext context)
        { _context = context; }

        // Helper method to create an initial query thats used by most other methods
        IQueryable<Item> InitialQuery(int skip = 0, int count = int.MaxValue, ItemIncludeOptions includeOptions = ItemIncludeOptions.None)
        {
            return _context.Items
                .Include(i => i.ObjectData)
                .Include(i => i.consumableData)
                .Include(i => i.ActiveItemData)
                .If(includeOptions.HasFlag(ItemIncludeOptions.Mods), q => q.Include(i => i.SourceMod))
                .If(includeOptions.HasFlag(ItemIncludeOptions.Unlocks), q => q.Include(i => i.Unlocks))
                .Skip(skip)
                .Take(count);
        }

        // Helper method that queries searches by type
        IQueryable<Item> SearchByType(IQueryable<Item> baseQueriable, ItemSearchOptions searchOptions = ItemSearchOptions.All)
        {
            var baseQ = baseQueriable; // Keep a copy of the original query, for pulling out relevant data and adding it into the returned query

            if (searchOptions == ItemSearchOptions.All)
                return baseQueriable;

            //var zzz = baseQ.Where(x => x.Type == Item.ItemTypes.consumableItem);

            return baseQ.Take(0)
                .If(searchOptions.HasFlag(ItemSearchOptions.Generic),
                    q => q.Union(baseQ.Where(i => i.Type == Item.ItemTypes.genericItem)))

                .If(searchOptions.HasFlag(ItemSearchOptions.Object),
                    q => q.Union(baseQ.Where(i => i.Type == Item.ItemTypes.objectItem)))

                .If(searchOptions.HasFlag(ItemSearchOptions.ActiveItem),
                    q => q.Union(baseQ.Where(i => i.Type == Item.ItemTypes.activeItem)))

                .If(searchOptions.HasFlag(ItemSearchOptions.consumable),
                    q => q.Union(baseQ.Where(i => i.Type == Item.ItemTypes.consumableItem)));
        }

        public Item GetItemByIds(int itemId, int sourceModId, ItemIncludeOptions options = ItemIncludeOptions.None)
        {
            return InitialQuery(includeOptions: options)
                .FirstOrDefault(i => i.ItemId == itemId && i.SourceModId == sourceModId);
        }

        public List<Item> GetAllItems(int skip = 0, int count = int.MaxValue, ItemIncludeOptions options = ItemIncludeOptions.None)
        {
            return InitialQuery(skip, count, options).OrderBy(i => i.ShortDescription.RemoveFormatting()).ToList();
        }

        public List<Item> GetItemsByShortDescription(string shortDescription, bool partialMatch = false, ItemIncludeOptions options = ItemIncludeOptions.None, ItemSearchOptions searchOptions = ItemSearchOptions.All)
        {
            var queriable = InitialQuery(includeOptions: options)
                    .If(partialMatch, q => q.Where(i => i.ShortDescription.RemoveFormatting().ToLower().StartsWith(shortDescription.ToLower())))
                    .If(!partialMatch, q => q.Where(i => i.ShortDescription.RemoveFormatting().ToLower().Equals(shortDescription.ToLower())))
                    .OrderBy(i => i.ShortDescription.RemoveFormatting());

            return SearchByType(queriable, searchOptions).ToList();
        }

        public List<Item> GetItemsByInternalName(string internalName, bool partialMatch = false, ItemIncludeOptions options = ItemIncludeOptions.None, ItemSearchOptions searchOptions = ItemSearchOptions.All)
        {
            var queriable = InitialQuery(includeOptions: options)
                    .If(partialMatch, q => q.Where(i => i.InternalName.ToLower().StartsWith(internalName.ToLower())))
                    .If(!partialMatch, q => q.Where(i => i.InternalName.ToLower().Equals(internalName.ToLower())))
                    .OrderBy(i => i.InternalName);

            return SearchByType(queriable, searchOptions).ToList();
        }
    }
}
