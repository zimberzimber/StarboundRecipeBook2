using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarboundRecipeBook2.Services;

namespace StarboundRecipeBook2.Controllers
{
    public class ModsController : Controller
    {
        IModRepository _modRepo;

        public ModsController(IModRepository modRepo)
        {
            _modRepo = modRepo;
        }

        public IActionResult Index()
        {
            return View(_modRepo.GetAllMods(ModIncludeOptions.All));
        }
    }
}