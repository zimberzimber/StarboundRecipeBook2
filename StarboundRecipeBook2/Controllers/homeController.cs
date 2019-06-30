using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarboundRecipeBook2.Data;

namespace StarboundRecipeBook2.Controllers
{
    public class HomeController : Controller
    {
        DatabaseContext _context;

        public HomeController(DatabaseContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Items.ToList());
        }
    }
}