using E_Library.Data;
using E_Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Library.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (category.Name != null)
            {
                var newCategory = new Category()
                {
                    Name = category.Name,
                    Description = category.Description,
                };
                await _context.Categories.AddAsync(newCategory);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Record created successfully ";
                TempData["Flag"] = "alert-success";
                return RedirectToAction(nameof(Index));
            }
            TempData["Message"] = "Error creating record";
            TempData["Flag"] = "alert-danger";
            return View(category);
        }

        public async Task<IActionResult> Edit(int Id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x=>x.Id == Id);
            if (category == null)
            {
                TempData["Message"] = "Error finding record";
                TempData["Flag"] = "alert-danger";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, Category category)
        {
            var existingCategory = await _context.Categories.FindAsync(Id);
            if (existingCategory == null)
            {
                TempData["Message"] = "Error finding record";
                TempData["Flag"] = "alert-danger";
                return RedirectToAction(nameof(Index));
            }
            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            _context.Categories.Update(existingCategory);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Record updated successfully";
            TempData["Flag"] = "alert-success";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var existingCategory = await _context.Categories.FindAsync(Id);
            _context.Categories.Remove(existingCategory);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Record deleted successfully";
            TempData["Flag"] = "alert-success";
            return RedirectToAction(nameof(Index));
        }
    }
}
