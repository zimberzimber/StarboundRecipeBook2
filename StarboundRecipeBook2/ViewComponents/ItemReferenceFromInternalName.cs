using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace StarboundRecipeBook2.ViewComponents
{
    public class ItemReferenceFromInternalNameViewComponent : ViewComponent
    {
        // Not making the item list and passing it to the view because the item
        // we're trying to get may not exist in the current collection of mods,
        // so I'd like to at least show its name
        public Task<IViewComponentResult> InvokeAsync(string internalName)
            => Task.FromResult<IViewComponentResult>(View("Default", internalName));
    }
}
