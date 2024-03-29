﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarboundRecipeBook2.Helpers;
using StarboundRecipeBook2.Services;

namespace StarboundRecipeBook2.Controllers
{
    public class ItemsController : Controller
    {
        IItemRepository _itemRepo;

        public ItemsController(IItemRepository itemRepo)
        { _itemRepo = itemRepo; }

        public IActionResult Index()
        {
            ItemSearchType searchOptions = ItemSearchType.None;

            bool? generic = Request.Cookies["filterGeneric"].ToBool();
            bool? objects = Request.Cookies["filterObjects"].ToBool();
            bool? activeItems = Request.Cookies["filterActiveItems"].ToBool();
            bool? consumables = Request.Cookies["filterConsumables"].ToBool();
            bool? partialNameMatch = Request.Cookies["partialNameMatch"].ToBool();
            string searchBySelected = Request.Cookies["searchBy"];

            bool hasItemSearch = HttpContext.Request.Query.TryGetValue("itemSearch", out var itemSearch);

            // Add flags based on check boxes
            if (generic == true)
                searchOptions |= ItemSearchType.Generic;

            if (objects == true)
                searchOptions |= ItemSearchType.Object;

            if (activeItems == true)
                searchOptions |= ItemSearchType.ActiveItem;

            if (consumables == true)
                searchOptions |= ItemSearchType.Consumable;

            // No ItemSearchOptions selected
            if (searchOptions == ItemSearchType.None)
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
            var item = _itemRepo.GetItemByIds(itemId, modId, ItemIncludeOptions.All);
            if (item == default)
                return RedirectToAction("Index", "Home");
            return View(item);
        }
    }
}