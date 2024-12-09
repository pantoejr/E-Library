using E_Library.Data;
using E_Library.Helpers;
using E_Library.Models;
using E_Library.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace E_Library.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        public UsersController(UserManager<AppUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["GroupID"] = new SelectList(_context.Groups, "Id", "Name");
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Error creating user";
                TempData["Flag"] = "alert-danger";
                ViewData["GroupID"] = new SelectList(_context.Groups, "Id", "Name", model.GroupID);
            }
            var appUser = new AppUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Email,
                LoginHint = model.ConfirmPassword,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                IsActive = true,
            };
            var groupRoles = await _context.GroupRoles.Include(x => x.IdentityRole).Select(x => x.IdentityRole.Name).ToListAsync();
            var result = await _userManager.CreateAsync(appUser, model.ConfirmPassword);
            if (result.Succeeded)
            {
                var userGroup = new GroupUser
                {
                    GroupID = model.GroupID,
                    UserID = appUser.Id,
                };
                await _context.GroupUsers.AddAsync(userGroup);
                await _userManager.AddToRolesAsync(appUser, groupRoles);
                await _context.SaveChangesAsync();
                TempData["Message"] = "User created successfully";
                TempData["Flag"] = "alert-success";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(String Id)
        {
            var existingUser = await _userManager.FindByIdAsync(Id);

            var group = await _context.GroupUsers.Include(x => x.Group)
                .Where(x => x.UserID.Equals(existingUser.Id))
                .Select(x => x.Group)
                .FirstOrDefaultAsync();

            ViewData["GroupID"] = new SelectList(_context.Groups, "Id", "Name", group.Id);

            var user = new UserViewModel
            {
                FirstName = existingUser.FirstName,
                LastName = existingUser.LastName,
                Email = existingUser.Email,
                PhoneNumber = existingUser.PhoneNumber,
                Password = existingUser.LoginHint,
                GroupID = group.Id
            };
            return View(user);
        }

        public async Task<IActionResult> Edit(string Id, UserViewModel model)
        {
            var existingUser = await _userManager.FindByIdAsync(Id);
            existingUser.FirstName = model.FirstName;
            existingUser.LastName = model.LastName;
            existingUser.PhoneNumber = model.PhoneNumber;
            existingUser.Email = model.Email;
            await _userManager.UpdateAsync(existingUser);
            await _context.UpdateUserRoles(model.GroupID, existingUser.Id);
            TempData["Message"] = "User updated successfully";
            TempData["Flag"] = "alert-success";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string Id)
        {
            var existingUser = await _userManager.FindByIdAsync(Id);

            var group = await _context.GroupUsers.Include(x => x.Group)
                .Where(x => x.UserID.Equals(existingUser.Id))
                .Select(x => x.Group)
                .FirstOrDefaultAsync();

            ViewData["GroupID"] = new SelectList(_context.Groups, "Id", "Name", group.Id);

            var user = new UserViewModel
            {
                UserID = existingUser.Id,
                FirstName = existingUser.FirstName,
                LastName = existingUser.LastName,
                Email = existingUser.Email,
                PhoneNumber = existingUser.PhoneNumber,
                Password = existingUser.LoginHint,
                GroupID = group.Id,
                LoginHint = existingUser.LoginHint,
            };
            return View(user);
        }

        public async Task<IActionResult> RefreshRoles(int GroupID, string UserID)
        {
            if (GroupID != null && UserID != null)
            {
                var result = await _context.UpdateUserRoles(GroupID, UserID);
                if (result == 100)
                {
                    TempData["Message"] = "Roles refreshed successfully";
                    TempData["Flag"] = "alert-success";
                    return RedirectToAction(nameof(Details), new { Id = UserID });
                }
            }
            TempData["Message"] = "Error refreshing user roles";
            TempData["Flag"] = "alert-danger";
            return RedirectToAction(nameof(Details), new { Id = UserID });
        }

        [HttpGet]
        public async Task<IActionResult> UnAvUserRoles(string Id)
        {

            var unAvailableUserRoles = await _context.GetUserRolesById(new SqlParameter("@UserID", Id));
            ViewData["UserID"] = Id;
            return PartialView("_UnAvUserRoles", unAvailableUserRoles);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                TempData["Message"] = "Error finding record";
                TempData["Flag"] = "alert-danger";
                return RedirectToAction(nameof(Index));
            }

            await _userManager.DeleteAsync(user);
            TempData["Message"] = "Record deleted successfully";
            TempData["Flag"] = "alert-success";
            return RedirectToAction(nameof(Index));
        }
    }
}
