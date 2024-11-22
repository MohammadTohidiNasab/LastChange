using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Divar.Controllers
{
    public class RolesController : Controller
    {
        private readonly DivarDbContext _context;
        private readonly UserManager<CustomUser> _userManager;

        public RolesController(DivarDbContext context, UserManager<CustomUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _context.Roles.ToListAsync();
            return View(roles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["AccessLevels"] = Enum.GetValues(typeof(AccessLevel)).Cast<AccessLevel>().ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Role role, List<AccessLevel> selectedPermissions)
        {
            if (ModelState.IsValid)
            {
                role.Permissions = selectedPermissions;
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewData["AccessLevels"] = Enum.GetValues(typeof(AccessLevel)).Cast<AccessLevel>().ToList();
            return View(role);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            ViewData["AccessLevels"] = Enum.GetValues(typeof(AccessLevel)).Cast<AccessLevel>().ToList();
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Role updatedRole, List<AccessLevel> selectedPermissions)
        {
            if (ModelState.IsValid)
            {
                var role = await _context.Roles.FindAsync(updatedRole.Id);
                if (role != null)
                {
                    role.Name = updatedRole.Name;
                    role.Permissions = selectedPermissions;
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }

            ViewData["AccessLevels"] = Enum.GetValues(typeof(AccessLevel)).Cast<AccessLevel>().ToList();
            return View(updatedRole);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }


        //اختصاص نقش به کاربران

        public IActionResult AssignRole()
        {
            var users = _userManager.Users.ToList(); // تمامی کاربران را دریافت کنید
            var roles = _context.Roles.Select(r => r.Name).ToList(); // تمامی نام نقش‌ها را دریافت کنید

            var model = new AssignRoleViewModel
            {
                AvailableRoles = roles,
                Users = users.Select(user => new SelectListItem
                {
                    Value = user.Id,
                    Text = user.FirstName + " " + user.LastName
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(AssignRoleViewModel model)
        {
            if (string.IsNullOrEmpty(model.UserId) || model.SelectedRoles == null || !model.SelectedRoles.Any())
            {
                ModelState.AddModelError(string.Empty, "Invalid user or roles.");
                return View(model); // return with error message
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View(model); // return with error message
            }

            // Remove existing roles if any
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            // Assign new roles
            var result = await _userManager.AddToRolesAsync(user, model.SelectedRoles);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model); // return with error message
            }

            return RedirectToAction("Index"); // Redirect after successful assignment
        }


    }
}
