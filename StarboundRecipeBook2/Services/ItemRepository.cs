using MongoDB.Driver;
using SBRB.Models;
using StarboundRecipeBook2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarboundRecipeBook2.Services
{
    /// <summary>
    /// Enum containing what the items can be search by when searching by string.
    /// </summary>
    public enum ItemSearchBy : byte
    {
        InternalName,
        ShortDescription
    }

    /// <summary>
    /// Flag enum containing item types which can be searched for.
    /// </summary>
    [Flags]
    public enum ItemSearchType : ushort
    {
        None = 0,
        Generic = 1,
        ActiveItem = 2,
        Consumable = 4,
        Object = 8,
        Armor = 16,
        Augment = 32,
        CurrencyItem = 64,
        Tool = 128,
        Flashlight = 256,
        Material = 512,
        Liquid = 1024,
        Instrument = 2048,
        All = 4095
    }

    public interface IItemRepository : IBaseRepository<Item>
    {
        /// <summary>
        /// Look for an item by ID
        /// </summary>
        /// <param name="itemId">The items id within the database</param>
        /// <param name="sourceModId">The items source mod ID</param>
        /// <returns>An item matching the IDs, or null if it wasn't found</returns>
        Item GetItemById(uint itemId, uint sourceModId);

        /// <summary>
        /// Look for an item by ID
        /// </summary>
        /// <param name="compositeItemId">A composite item ID, containing both the database ID and the source mod ID</param>
        /// <returns>An item matching the composite ID, or null if it wasn't found</returns>
        Item GetItemById(CompositeItemId compositeItemId);

        /// <summary>
        /// Get all items without applying any filters
        /// </summary>
        /// <param name="skip">How many should be skipped</param>
        /// <param name="take">How many should be taken</param>
        /// <returns>A list containing the items</returns>
        List<Item> GetAllItems(int skip = 0, int take = int.MaxValue);

        /// <summary>
        /// Get items fitting the parameters
        /// </summary>
        /// <param name="searching">The string to search by</param>
        /// <param name="partialMatch">Whether the search should look in the middle of an items name</param>
        /// <param name="skip">How many should be skipped</param>
        /// <param name="take">How many should be taken</param>
        /// <param name="searchBy">Search by internal name or short description</param>
        /// <param name="searchType">Item types to search for</param>
        /// <returns>A list containing the items matching the searching criteria</returns>
        List<Item> GetItems(string searching, bool partialMatch = false, int skip = 0, int take = int.MaxValue, ItemSearchBy searchBy = ItemSearchBy.InternalName, ItemSearchType searchType = ItemSearchType.All);
    }

    public class ItemRepository : BaseRepository<Item>, IItemRepository
    {
        public override IQueryable<Item> BaseQuery => _db.Items.AsQueryable();

        public Item GetItemById(uint itemId, uint sourceModId)
            => BaseQuery.FirstOrDefault(i => i.ID.ItemId == itemId && i.ID.SourceModId == sourceModId);

        public Item GetItemById(CompositeItemId compositeItemId)
            => GetItemById(compositeItemId.ItemId, compositeItemId.SourceModId);

        IQueryable<Item> SearchByType(IQueryable<Item> baseQueriable, ItemSearchType searchOptions)
        {
            if (searchOptions == ItemSearchType.None)
                return baseQueriable.Take(0);

            if (searchOptions.HasFlag(ItemSearchType.All))
                return baseQueriable;

            var resultQ = baseQueriable.Take(0);

            foreach (ItemSearchType option in Enum.GetValues(typeof(ItemSearchType)))
                if (searchOptions.HasFlag(option))
                    resultQ.Union(baseQueriable.Where(i => i.ItemType.ToString().Equals(option.ToString())));

            return resultQ;
        }

        IQueryable<Item> SearchByName(IQueryable<Item> baseQueriable, string searching, bool partialMatch, ItemSearchBy order)
        {
            IQueryable<Item> resultQ;

            switch (order)
            {
                case ItemSearchBy.ShortDescription:
                    resultQ = baseQueriable
                                .If(partialMatch, q => q.Where(i => i.ShortDescription.RemoveFormatting().StartsWith(searching, StringComparison.OrdinalIgnoreCase)))
                                .If(!partialMatch, q => q.Where(i => i.ShortDescription.RemoveFormatting().Equals(searching, StringComparison.OrdinalIgnoreCase)))
                                .OrderBy(i => i.ShortDescription.RemoveFormatting());
                    break;
                case ItemSearchBy.InternalName:
                    resultQ = baseQueriable
                                .If(partialMatch, q => q.Where(i => i.InternalName.StartsWith(searching, StringComparison.OrdinalIgnoreCase)))
                                .If(!partialMatch, q => q.Where(i => i.InternalName.Equals(searching, StringComparison.OrdinalIgnoreCase)))
                                .OrderBy(i => i.InternalName);
                    break;
                default:
                    throw new ArgumentException("Received an unhandled value for the 'order' arguement under the 'SearchByName' method.");
            }

            return resultQ;
        }

        public List<Item> GetAllItems(int skip = 0, int take = int.MaxValue)
            => SkipTake(BaseQuery, skip, take).ToList();

        public List<Item> GetItems(string searching, bool partialMatch = false, int skip = 0, int take = int.MaxValue, ItemSearchBy searchBy = ItemSearchBy.InternalName, ItemSearchType searchType = ItemSearchType.All)
        {
            var baseQ = SkipTake(BaseQuery, skip, take);
            baseQ = SearchByType(baseQ, searchType);
            baseQ = SearchByName(baseQ, searching, partialMatch, searchBy);
            return baseQ.ToList();
        }
    }
}