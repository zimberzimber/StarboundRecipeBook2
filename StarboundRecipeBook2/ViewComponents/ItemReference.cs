using Microsoft.AspNetCore.Mvc;
using SBRB.Models;
using System.Threading.Tasks;

namespace StarboundRecipeBook2.ViewComponents
{
    public class ItemReferenceViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(Item item)
            => Task.FromResult<IViewComponentResult>(View("Default", item));
    }
}
