using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarboundRecipeBook2.Data;
using StarboundRecipeBook2.Services;

namespace StarboundRecipeBook2.Controllers
{
    public class HomeController : Controller
    {
        IItemRepository _itemRepo;

        public HomeController(IItemRepository itemRepo)
        {
            _itemRepo = itemRepo;
        }

        public IActionResult Index()
        {
            return View(_itemRepo.GetAllItems(options: ItemIncludeOptions.Unlocks).Where(i => i.Unlocks.Count > 0).ToList());
        }
    }
}