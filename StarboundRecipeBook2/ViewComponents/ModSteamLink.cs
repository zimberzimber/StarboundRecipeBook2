using Microsoft.AspNetCore.Mvc;
using StarboundRecipeBook2.Services;
using System.Threading.Tasks;

namespace StarboundRecipeBook2.ViewComponents
{
    public class ModSteamLinkViewComponent : ViewComponent
    {
        IModRepository _modRepo;

        public ModSteamLinkViewComponent(IModRepository modRepo)
        { _modRepo = modRepo; }

        public Task<IViewComponentResult> InvokeAsync(int steamID)
        {
            if (steamID < 0)
                return Task.FromResult<IViewComponentResult>(View("BaseAssets"));
            else
                return Task.FromResult<IViewComponentResult>(View("Default", _modRepo.GetModById(steamID)));
        }
    }
}
