using E_Library.Data;
using E_Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_Library.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.Include(b => b.Category).ToListAsync();
            return View(books);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.Include(b => b.Category)
                                           .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null) return NotFound();
            ViewData["CategoryID"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryID);
            return View(book);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book, IFormFile coverImageFile, IFormFile pdfFile)
        {
            if (book != null)
            {
                if (coverImageFile != null)
                {
                    using var ms = new MemoryStream();
                    await coverImageFile.CopyToAsync(ms);
                    book.CoverImage = ms.ToArray();
                }

                if (pdfFile != null)
                {
                    var filePath = Path.Combine("wwwroot/files", Guid.NewGuid().ToString() + Path.GetExtension(pdfFile.FileName));
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await pdfFile.CopyToAsync(stream);

                    book.FilePath = filePath;
                    book.FileSize = pdfFile.Length;
                }

                _context.Add(book);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Book uploaded successfully";
                TempData["Flag"] = "alert-success";
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryID"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryID);
            return View(book);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            ViewData["CategoryID"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryID);
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book, IFormFile coverImageFile, IFormFile pdfFile)
        {
            if (id != book.Id) return NotFound();
            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook != null)
            {
                System.IO.File.Delete(existingBook.FilePath);
                if (coverImageFile != null)
                {
                    using var ms = new MemoryStream();
                    await coverImageFile.CopyToAsync(ms);
                    existingBook.CoverImage = ms.ToArray();
                }

                if (pdfFile != null)
                {
                    var filePath = Path.Combine("wwwroot/files", Guid.NewGuid().ToString() + Path.GetExtension(pdfFile.FileName));
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await pdfFile.CopyToAsync(stream);

                    existingBook.FilePath = filePath;
                    existingBook.FileSize = pdfFile.Length;
                }

                _context.Books.Update(existingBook);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Book Updated Successfully";
                TempData["Flag"] = "alert-success";
                return RedirectToAction(nameof(Index));
            }

            TempData["Message"] = "Error updating book";
            TempData["Flag"] = "alert-danger";
            ViewData["CategoryID"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryID);
            return View(book);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int Id)
        {
            var book = await _context.Books.FindAsync(Id);
            if (book != null)
            {
                System.IO.File.Delete(book.FilePath);
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            TempData["Message"] = "Record deleted successfully";
            TempData["Flag"] = "alert-success";
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }

        public IActionResult Read(int Id)
        {
            var book = _context.Books.Find(Id);
            if (book == null || String.IsNullOrEmpty(book.FilePath))
            {
                return NotFound();
            }
            book.Views++;
            _context.SaveChanges();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), book.FilePath);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var fileStream = System.IO.File.OpenRead(filePath);
            return File(fileStream, "application/pdf");
        }

        public IActionResult Download(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null || string.IsNullOrEmpty(book.FilePath))
            {
                return NotFound();
            }
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), book.FilePath);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var fileName = Path.GetFileName(book.FilePath);
            return File(System.IO.File.ReadAllBytes(filePath), "application/pdf", fileName);
        }


        [HttpGet]
        public JsonResult SearchBooks(string searchTerm)
        {
            var books = _context.Books
                .Include(b => b.Category)
                .Where(b => b.Title.Contains(searchTerm) || b.Description.Contains(searchTerm) || b.Category.Name.Contains(searchTerm))
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Description,
                    CategoryName = b.Category.Name,
                    CoverImage = Convert.ToBase64String(b.CoverImage)
                })
                .ToList();

            return Json(books);
        }

    }
}
