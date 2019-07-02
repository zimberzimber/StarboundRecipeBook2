using Microsoft.AspNetCore.Mvc;
using StarboundRecipeBook2.Services;

namespace StarboundRecipeBook2.Controllers
{
    public class ItemsController : Controller
    {
        ItemRepository _itemRepo;

        public ItemsController(ItemRepository itemRepo)
        {
            _itemRepo = itemRepo;
        }

        public IActionResult Index()
        {
            return View(_itemRepo.GetAllItems());
        }
    }
}