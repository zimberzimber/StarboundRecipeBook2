using Microsoft.AspNetCore.Mvc;
using StarboundRecipeBook2.Services;
using System.Threading.Tasks;

namespace StarboundRecipeBook2.ViewComponents
{
    public class ItemReferenceFromInternalNameViewComponent : ViewComponent
    {
        IItemRepository _itemRepo;

        public ItemReferenceFromInternalNameViewComponent(IItemRepository itemRepo)
        {
            _itemRepo = itemRepo;
        }

        public Task<IViewComponentResult> InvokeAsync(string internalName)
        {
            // Not making the item list and passing it to the view because the item
            // we're trying to get may not exist in the current collection of mods,
            // so I'd like to at least show its name

            return Task.FromResult<IViewComponentResult>(View("Default", internalName));
        }
    }
}
