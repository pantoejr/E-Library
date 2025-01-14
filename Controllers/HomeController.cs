using E_Library.Data;
using E_Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        //public async Task<IActionResult> Index()
        //{
        //    var book = await _context.BookAccessLogs
        //        .Include(x => x.Book)
        //        .Where(log => log.Action == "Download")
        //        .GroupBy(log => new
        //        {
        //            log.BookID,
        //            log.Book.Title,
        //        }).OrderByDescending(group => group.Count()).Select(group => new
        //        {
        //            BookID = group.Key.BookID,
        //            BookTitle = group.Key.Title,
        //            DownloadCount = group.Count()
        //        }).FirstOrDefaultAsync();

        //    return View();
        //}

        public IActionResult Index()
        {
            var books = _context.Books.ToList();
            return View("Books", books);
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
