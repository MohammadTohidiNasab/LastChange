using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        // ریزرواشطر برای AssignRole به همان صورت
    }
}
