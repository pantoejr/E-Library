using E_Library.Data;
using E_Library.Helpers;
using E_Library.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Library.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        public AccountController(AppDbContext context, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetTempDataMessage("Invalid input. Please try again.", "alert-danger");
                return View(model);
            }
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Username);
                if (user == null || !user.IsActive)
                {
                    SetTempDataMessage("Incorrect Username and Password", "alert-danger");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    SetTempDataMessage("Login successfull", "alert-success");
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                SetTempDataMessage("An error occurred. Please try again later.", "alert-danger");
            }

            SetTempDataMessage("Incorrect Username and Password", "alert-danger");
            return View(model);
        }

        private void SetTempDataMessage(string message, string flag)
        {
            TempData["Message"] = message;
            TempData["Flag"] = flag;
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        user.LoginHint = model.NewPassword;
                        _context.Users.Update(user);
                        await _context.SaveChangesAsync();
                        await _signInManager.SignOutAsync();
                        TempData["Message"] = "Password was changed successfully";
                        return RedirectToAction("Login", "Account");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                    TempData["Flag"] = "alert-danger";
                    return RedirectToAction("ChangePassword", "Account");
                }
            }
            TempData["Message"] = "Error changing password";
            TempData["Flag"] = "alert-danger";
            return RedirectToAction("ChangePassword", "Account");
        }
    }
}
