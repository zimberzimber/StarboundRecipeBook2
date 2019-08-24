using Microsoft.AspNetCore.Mvc;
using StarboundRecipeBook2.Helpers;
using System.Threading.Tasks;

namespace StarboundRecipeBook2.ViewComponents
{
    public class FormatlessTextViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(string raw)
            => Task.FromResult<IViewComponentResult>(View("Default", raw.RemoveFormatting()));
    }
}
