using Microsoft.AspNetCore.Mvc;
using StarboundRecipeBook2.Models;
using System.Threading.Tasks;

namespace StarboundRecipeBook2.ViewComponents
{
    public class ItemReferenceViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(Item item)
            => Task.FromResult<IViewComponentResult>(View("Default", item));
    }
}
