using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarboundRecipeBook2.Helpers;
using StarboundRecipeBook2.Services;
using System;

namespace StarboundRecipeBook2.Controllers
{
    public class ItemsController : Controller
    {
        IItemRepository _itemRepo;

        public ItemsController(IItemRepository itemRepo)
            => _itemRepo = itemRepo;

        public IActionResult Index()
        {
            ItemSearchType searchOptions = ItemSearchType.None;

            // Get parameters
            bool hasItemSearch = HttpContext.Request.Query.TryGetValue("itemSearch", out var itemSearch);
            bool? partialNameMatch = Request.Cookies["partialNameMatch"].ToBool();
            string searchBySelected = Request.Cookies["searchBy"];

            // Get a list of item search types selected by the user
            foreach (ItemSearchType option in Enum.GetValues(typeof(ItemSearchType)))
                if (Request.Cookies[$"filter-{option.ToString()}"].ToBool() == true)
                    searchOptions |= option;

            // No ItemSearchOptions selected
            if (searchOptions == ItemSearchType.None)
                return View();

            // Empty text box
            else if (!hasItemSearch || string.IsNullOrWhiteSpace(itemSearch))
                return View();

            ViewBag.itemSearch = itemSearch;
            ItemSearchBy searchBy;

            if (searchBySelected == "searchByInternalName")
                searchBy = ItemSearchBy.InternalName;
            else
                searchBy = ItemSearchBy.ShortDescription;

            return View(_itemRepo.GetItems(itemSearch, partialMatch: partialNameMatch.GetValueOrDefault(), searchBy: searchBy, searchType: searchOptions));
        }

        public IActionResult Item(uint modId, uint itemId)
        {
            var item = _itemRepo.GetItemById(itemId, modId);
            if (item == default)
                return RedirectToAction("Index", "Home");
            return View(item);
        }
    }
}