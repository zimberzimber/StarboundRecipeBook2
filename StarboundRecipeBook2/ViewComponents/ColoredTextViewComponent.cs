using Microsoft.AspNetCore.Mvc;
using StarboundRecipeBook2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarboundRecipeBook2.ViewComponents
{
    public class ColoredTextViewComponent : ViewComponent
    {
        ITextColorResolver _colorResolver;

        public ColoredTextViewComponent(ITextColorResolver colorResolver)
        {
            _colorResolver = colorResolver;
        }

        public Task<IViewComponentResult> InvokeAsync(string raw)
        {
            return Task.FromResult<IViewComponentResult>(View("Default", _colorResolver.ResolveColor(raw)));
        }
    }
}
