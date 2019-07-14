using Microsoft.AspNetCore.Mvc;
using StarboundRecipeBook2.Services;

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

            bool hasItemSearch = HttpContext.Request.Query.TryGetValue("itemSearch", out var itemSearch);
            bool generic = HttpContext.Request.Query.TryGetValue("filterGeneric", out var _);
            bool objects = HttpContext.Request.Query.TryGetValue("filterObjects", out var _);
            bool activeItems = HttpContext.Request.Query.TryGetValue("filterActiveItems", out var _);
            bool consumables = HttpContext.Request.Query.TryGetValue("filterConsumables", out var _);
            bool partialNameMatch = HttpContext.Request.Query.TryGetValue("partialNameMatch", out var _);
            bool searchBySelected = HttpContext.Request.Query.TryGetValue("searchBy", out var searchBy);

            // Add flags based on check boxes
            if (generic)
            {
                ViewBag.filterGeneric = true;
                searchOptions |= ItemSearchOptions.Generic;
            }
            else
                ViewBag.filterGeneric = null;

            if (objects)
            {
                ViewBag.filterObjects = true;
                searchOptions |= ItemSearchOptions.Object;
            }
            else
                ViewBag.filterObjects = null;

            if (activeItems)
            {
                ViewBag.filterActiveItems = true;
                searchOptions |= ItemSearchOptions.ActiveItem;
            }
            else
                ViewBag.filterActiveItems = null;

            if (consumables)
            {
                ViewBag.filterConsumables = true;
                searchOptions |= ItemSearchOptions.consumable;
            }
            else
                ViewBag.filterConsumables = null;

            if (partialNameMatch)
                ViewBag.partialNameMatch = true;
            else
                ViewBag.partialNameMatch = null;

            if (searchBySelected && searchBy == "internalName")
                ViewBag.searchBySelected = "internalName";
            else
                ViewBag.searchBySelected = "displayedName";

            // No ItemSearchOptions selected  -OR-  partialNameMatch is off, and searching for empty string
            if (searchOptions == ItemSearchOptions.None || (!partialNameMatch && (!hasItemSearch || string.IsNullOrWhiteSpace(itemSearch))))
                return View();

            // Not an empty search string
            else if (hasItemSearch && !string.IsNullOrWhiteSpace(itemSearch))
            {
                ViewBag.itemSearch = itemSearch;

                // Default to displayed name if no option is picked
                if (searchBySelected && searchBy == "internalName")
                    return View(_itemRepo.GetItemsByInternalName(itemSearch, partialMatch: partialNameMatch, searchOptions: searchOptions));
                else
                    return View(_itemRepo.GetItemsByShortDescription(itemSearch, partialMatch: partialNameMatch, searchOptions: searchOptions));
            }

            else
            {
                // Default to displayed name if no option is picked
                if (searchBySelected && searchBy == "internalName")
                    return View(_itemRepo.GetItemsByInternalName("", partialMatch: true, searchOptions: searchOptions));
                else
                    return View(_itemRepo.GetItemsByShortDescription("", partialMatch: true, searchOptions: searchOptions));
            }
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