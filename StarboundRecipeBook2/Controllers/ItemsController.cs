using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarboundRecipeBook2.Helpers;
using StarboundRecipeBook2.Services;

namespace StarboundRecipeBook2.Controllers
{
    public class ItemsController : Controller
    {
        IItemRepository _itemRepo;

        public ItemsController(IItemRepository itemRepo)
        {
            _itemRepo = itemRepo;

            // Set default search options
            //bool? generic = Request.Cookies["filterGeneric"]?.ToBool();
            //bool? objects = Request.Cookies["filterObjects"]?.ToBool();
            //bool? activeItems = Request.Cookies["filterActiveItems"]?.ToBool();
            //bool? consumables = Request.Cookies["filterConsumables"]?.ToBool();
            //bool? partialNameMatch = Request.Cookies["partialNameMatch"]?.ToBool();

            //if (generic == null) Response.Cookies.Append("filterGeneric", "true");
            //if (objects == null) Response.Cookies.Append("filterObjects", "true");
            //if (activeItems == null) Response.Cookies.Append("filterActiveItems", "true");
            //if (consumables == null) Response.Cookies.Append("filterConsumables", "true");
            //if (partialNameMatch == null) Response.Cookies.Append("partialNameMatch", "true");
        }

        public IActionResult Index()
        {
            ItemSearchOptions searchOptions = ItemSearchOptions.None;

            bool? generic = Request.Cookies["filterGeneric"].ToBool();
            bool? objects = Request.Cookies["filterObjects"].ToBool();
            bool? activeItems = Request.Cookies["filterActiveItems"].ToBool();
            bool? consumables = Request.Cookies["filterConsumables"].ToBool();
            bool? partialNameMatch = Request.Cookies["partialNameMatch"].ToBool();
            string searchBySelected = Request.Cookies["searchBy"];

            // searchByInternalName
            // searchByDisplayedName

            bool hasItemSearch = HttpContext.Request.Query.TryGetValue("itemSearch", out var itemSearch);

            // Add flags based on check boxes
            if (generic == true)
                searchOptions |= ItemSearchOptions.Generic;

            if (objects == true)
                searchOptions |= ItemSearchOptions.Object;

            if (activeItems == true)
                searchOptions |= ItemSearchOptions.ActiveItem;

            if (consumables == true)
                searchOptions |= ItemSearchOptions.Consumable;

            // No ItemSearchOptions selected
            if (searchOptions == ItemSearchOptions.None)
                return View();

            // Empty text box
            else if (!hasItemSearch || string.IsNullOrWhiteSpace(itemSearch))
                return View();

            // Not an empty search string
            else
            {
                ViewBag.itemSearch = itemSearch;

                // Default to displayed name if no option is picked
                if (searchBySelected == "searchByInternalName")
                    return View(_itemRepo.GetItemsByInternalName(itemSearch, partialMatch: partialNameMatch.GetValueOrDefault(), searchOptions: searchOptions));
                else
                    return View(_itemRepo.GetItemsByShortDescription(itemSearch, partialMatch: partialNameMatch.GetValueOrDefault(), searchOptions: searchOptions));
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