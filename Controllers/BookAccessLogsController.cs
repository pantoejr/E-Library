using E_Library.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Library.Controllers
{
    [Authorize]
    public class BookAccessLogsController : Controller
    {
        private readonly AppDbContext _context;
        public BookAccessLogsController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var bookAccessLogs = await _context.BookAccessLogs
                .Include(x=>x.Book)
                .Include(x=>x.User)
                .ToListAsync();
            return View(bookAccessLogs);
        }
    }
}
