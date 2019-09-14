using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SBRB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

// MongoDB queriables don't support some string methods (.Equals / .StartsWith), or custom extensions (.RemoveFormatting)

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
            if (searchOptions.HasFlag(ItemSearchType.All))
                return baseQueriable;

            var resultQ = baseQueriable.Take(0);

            if (searchOptions != ItemSearchType.None)
                foreach (ItemSearchType option in Enum.GetValues(typeof(ItemSearchType)))
                    if (searchOptions.HasFlag(option))
                        resultQ.Union(baseQueriable.Where(i => i.ItemType.ToString().Equals(option.ToString())));

            return resultQ;
        }

        // Mongo C# driver cannot work with string operations inside the query (i.e  i => i.ShortDescription.ToLower().Equals(searchingBy))
        // So I have to resort to using a filter
        FilterDefinition<Item> GetSearchByNameFilter(string searching, bool partialMatch, ItemSearchBy searchBy)
        {
            FilterDefinition<Item> resultFilter;
            BsonRegularExpression regex;

            if (partialMatch)
                regex = new BsonRegularExpression($"^{searching}", "i");
            else
                regex = new BsonRegularExpression($"^{searching}$", "i");

            if (searchBy == ItemSearchBy.InternalName)
                resultFilter = Builders<Item>.Filter.Regex(i => i.InternalName, regex);
            else
                resultFilter = Builders<Item>.Filter.Regex(i => i.ShortDescription, regex);

            return resultFilter;

            // Old code:
            /*
            IMongoQueryable<Item> resultQ;
            switch (searchBy)
            {
                case ItemSearchBy.ShortDescription:
                    if (partialMatch)
                        resultQ = baseQueriable.Where(i => i.ShortDescription.StartsWith(searching, StringComparison.OrdinalIgnoreCase));
                    else
                        resultQ = baseQueriable.Where(i => i.ShortDescription.Equals(searching, StringComparison.OrdinalIgnoreCase));

                    resultQ.OrderBy(i => i.ShortDescription);
                    break;

                case ItemSearchBy.InternalName:
                    if (partialMatch)
                        resultQ = baseQueriable.Where(i => i.InternalName.StartsWith(searching, StringComparison.OrdinalIgnoreCase));
                    else
                        resultQ = baseQueriable.Where(i => i.InternalName.ToLower() == searching.ToLower());

                    resultQ.OrderBy(i => i.InternalName);
                    break;

                default:
                    throw new ArgumentException("Received an unhandled value for the 'order' arguement under the 'SearchByName' method.");
            }

            return resultQ;
            */
        }

        public List<Item> GetAllItems(int skip, int take)
            => SkipTake(BaseQuery, skip, take).ToList();

        public List<Item> GetItems(string searching, bool partialMatch, int skip, int take, ItemSearchBy searchBy, ItemSearchType searchType)
        {
            IQueryable<Item> baseQ;

            var filter = GetSearchByNameFilter(searching, partialMatch, searchBy);
            var found = _db.Items.Find(filter);

            if (searchBy == ItemSearchBy.ShortDescription)
                baseQ = found.SortBy(i => i.ShortDescription).ToList().AsQueryable();
            else
                baseQ = found.SortBy(i => i.InternalName).ToList().AsQueryable();

            baseQ = SearchByType(baseQ, searchType);
            baseQ = SkipTake(baseQ, skip, take);

            return baseQ.ToList();
        }
    }
}