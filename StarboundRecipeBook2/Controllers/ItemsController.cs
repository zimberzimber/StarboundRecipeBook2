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
            return View(_itemRepo.GetAllItems());
        }

        public IActionResult Item(int modId, int itemId)
        {
            var item = _itemRepo.GetItem(itemId, modId);
            if (item == default)
                return RedirectToAction("Index", "Home");
            return View(item);
        }
    }
}