using E_Library.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Library.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesController(AppDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            if (role.Name == null)
            {
                return RedirectToAction(nameof(Index));
            }
            await _roleManager.CreateAsync(role);
            TempData["Message"] = "Record created successfully";
            TempData["Flag"] = "alert-success";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            var role = await _roleManager.FindByIdAsync(Id);
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, IdentityRole model)
        {
            var role = await _roleManager.FindByIdAsync(Id);
            if (role == null)
            {
                TempData["Message"] = "Error finding record";
                TempData["Flag"] = "yellow";
                return RedirectToAction(nameof(Index));
            }
            role.Name = model.Name;
            await _roleManager.UpdateAsync(role);
            TempData["Message"] = "Record updated successfully";
            TempData["Flag"] = "green";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(String Id)
        {
            var role = await _roleManager.FindByIdAsync(Id);
            await _roleManager.DeleteAsync(role);
            TempData["Message"] = "Record deleted successfully";
            TempData["Flag"] = "green";
            return RedirectToAction(nameof(Index));
        }
    }
}
