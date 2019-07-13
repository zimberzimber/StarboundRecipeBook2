using Microsoft.AspNetCore.Mvc;
using StarboundRecipeBook2.Services;
using System.Linq;

namespace StarboundRecipeBook2.Controllers
{
    public class ItemsController : Controller
    {
        IItemRepository _itemRepo;

        public ItemsController(IItemRepository itemRepo)
        {
            _itemRepo = itemRepo;
        }

        public IActionResult Index()
        {
            ItemSearchOptions searchOptions = ItemSearchOptions.None;

            bool seachingName = HttpContext.Request.Query.TryGetValue("itemSearch", out var itemSearch);
            bool generic = HttpContext.Request.Query.TryGetValue("filterGeneric", out var _);
            bool objects = HttpContext.Request.Query.TryGetValue("filterObjects", out var _);
            bool activeItems = HttpContext.Request.Query.TryGetValue("filterActiveItems", out var _);
            bool consumables = HttpContext.Request.Query.TryGetValue("filterconsumables", out var _);
            bool partialNameMatch = HttpContext.Request.Query.TryGetValue("partialNameMatch", out var _);

            // Add flags based on check boxes
            if (generic) searchOptions |= ItemSearchOptions.Generic;
            if (objects) searchOptions |= ItemSearchOptions.Object;
            if (activeItems) searchOptions |= ItemSearchOptions.ActiveItem;
            if (consumables) searchOptions |= ItemSearchOptions.consumable;

            // No ItemSearchOptions selected  -OR-  partialNameMatch is off, and searching for empty string
            if (searchOptions == ItemSearchOptions.None || (!partialNameMatch && (!seachingName || string.IsNullOrWhiteSpace(itemSearch))))
                return View();

            // Not an empty search string
            else if (seachingName && !string.IsNullOrWhiteSpace(itemSearch))
                return View(_itemRepo.GetItemsByShortDescription(itemSearch, partialMatch: partialNameMatch, searchOptions: searchOptions));

            // Not all item types
            else if (searchOptions != ItemSearchOptions.All)
                return View(_itemRepo.GetItemsByShortDescription("", partialMatch: true, searchOptions: searchOptions));

            // Everything
            else
                return View(_itemRepo.GetAllItems());
        }

        public IActionResult Item(int modId, int itemId)
        {
            var item = _itemRepo.GetItemByIds(itemId, modId, ItemIncludeOptions.Unlocks);
            if (item == default)
                return RedirectToAction("Index", "Home");
            return View(item);
        }
    }
}