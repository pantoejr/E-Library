using E_Library.Data;
using E_Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Library.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly AppDbContext _context;
        public GroupsController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var groups = _context.Groups.ToList();
            return View(groups);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(Group model)
        {
            if (model.Name == null && model.Description == null)
            {
                TempData["Message"] = "Error creating record";
                TempData["Flag"] = "alert-danger";
                return View(model);
            }
            var newGroup = new Group
            {
                Name = model.Name,
                Description = model.Description,
            };

            await _context.Groups.AddAsync(newGroup);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Record created successfully";
            TempData["Flag"] = "alert-success";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var group = _context.Groups.FirstOrDefault(x => x.Id.Equals(Id));
            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, Group model)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(x => x.Id.Equals(Id));
            if (group == null)
            {
                TempData["Message"] = "Error finding record";
                TempData["Flag"] = "alert-danger";
                return RedirectToAction(nameof(Index));
            }
            group.Name = model.Name;
            group.Description = model.Description;
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Record updated successfully";
            TempData["Flag"] = "alert-success";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Details(int Id)
        {
            var group = _context.Groups.FirstOrDefault(x => x.Id.Equals(Id));
            return View(group);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int Id)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(x => x.Id.Equals(Id));
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Group deleted successfully";
            TempData["Flag"] = "alert-success";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> UnAvGroupRoles(int Id)
        {
            var unAvailableGroupRoles = await _context.Roles.Where(role => _context.GroupRoles
                .Where(gr => gr.GroupID == Id)
                .Select(gr => gr.RoleID)
                .Contains(role.Id))
                .ToListAsync();

            ViewData["GroupID"] = Id;
            return PartialView("_UnAvGroupRoles", unAvailableGroupRoles);
        }

        [HttpGet]
        public async Task<IActionResult> AvGroupRoles(int Id)
        {
            var availableGroupRoles = await _context.Roles.Where(role => !_context.GroupRoles
                .Where(gr => gr.GroupID == Id)
                .Select(gr => gr.RoleID)
                .Contains(role.Id))
                .ToListAsync();

            ViewData["GroupID"] = Id;

            return PartialView("_AvGroupRoles", availableGroupRoles);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGroupRole(int groupID, string roleID)
        {
            try
            {
                var newGroupRole = new GroupRole
                {
                    RoleID = roleID,
                    GroupID = groupID,
                };
                await _context.GroupRoles.AddAsync(newGroupRole);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error: " + ex.Message;
                TempData["Flag"] = "alert-danger";
                return RedirectToAction(nameof(Details), new { Id = groupID });
            }
            TempData["Message"] = "Role added to group successfully";
            TempData["Flag"] = "alert-success";
            return RedirectToAction(nameof(Details), new { Id = groupID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveGroupRole(int groupID, string roleID)
        {
            try
            {
                var existingGroupRole = await _context.GroupRoles.FirstOrDefaultAsync(x => x.GroupID.Equals(groupID) && x.RoleID.Equals(roleID));
                _context.GroupRoles.Remove(existingGroupRole);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error: " + ex.Message;
                TempData["Flag"] = "alert-danger";
                return RedirectToAction(nameof(Details), new { Id = groupID });
            }
            TempData["Message"] = "Role removed from group successfully";
            TempData["Flag"] = "alert-success";
            return RedirectToAction(nameof(Details), new { Id = groupID });
        }
    }
}
