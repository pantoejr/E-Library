using E_Library.Data;
using E_Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace E_Library.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Books()
        {
            var books = _context.Books.ToList();
            return View(books);
        }

        public IActionResult Error(int? code)
        {
            if (code == 404)
            {
                return View("Error404");
            }
            else if (code == 500)
            {
                return View("Error500");
            }

            return View("Error");
        }
    }
}
