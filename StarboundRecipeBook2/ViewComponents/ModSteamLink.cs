using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using StarboundRecipeBook2.Services;
using System.Threading.Tasks;

namespace StarboundRecipeBook2.ViewComponents
{
    public class ModSteamLinkViewComponent : ViewComponent
    {
        IModRepository _modRepo;

        public ModSteamLinkViewComponent(IModRepository modRepo)
            => _modRepo = modRepo;

        public Task<IViewComponentResult> InvokeAsync(uint steamID)
        {
            ViewViewComponentResult view;

            if (steamID < 0)
                view = View("BaseAssets");
            else
                view = View("Default", _modRepo.GetModById(steamID));

            return Task.FromResult<IViewComponentResult>(view);
        }
    }
}
