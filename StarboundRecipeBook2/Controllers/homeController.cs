﻿using Microsoft.AspNetCore.Mvc;

namespace StarboundRecipeBook2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}