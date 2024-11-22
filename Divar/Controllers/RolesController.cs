namespace Divar.Controllers
{
    public class RolesController : Controller
    {
        private static List<Role> roles = new List<Role>();
        private readonly UserManager<CustomUser> _userManager;

        public RolesController(UserManager<CustomUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View(roles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["AccessLevels"] = Enum.GetValues(typeof(AccessLevel)).Cast<AccessLevel>().ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Role role, List<AccessLevel> selectedPermissions)
        {
            if (ModelState.IsValid)
            {
                role.Permissions = selectedPermissions;
                roles.Add(role);
                return RedirectToAction("Index");
            }

            ViewData["AccessLevels"] = Enum.GetValues(typeof(AccessLevel)).Cast<AccessLevel>().ToList();
            return View(role);
        }

        public IActionResult Edit(string name)
        {
            var role = roles.FirstOrDefault(r => r.Name == name);
            if (role == null)
            {
                return NotFound();
            }

            ViewData["AccessLevels"] = Enum.GetValues(typeof(AccessLevel)).Cast<AccessLevel>().ToList();
            return View(role);
        }

        [HttpPost]
        public IActionResult Edit(Role updatedRole, List<AccessLevel> selectedPermissions)
        {
            if (ModelState.IsValid)
            {
                var role = roles.FirstOrDefault(r => r.Name == updatedRole.Name);
                if (role != null)
                {
                    role.Permissions = selectedPermissions;
                }
                return RedirectToAction("Index");
            }

            ViewData["AccessLevels"] = Enum.GetValues(typeof(AccessLevel)).Cast<AccessLevel>().ToList();
            return View(updatedRole);
        }

        public IActionResult Delete(string name)
        {
            var role = roles.FirstOrDefault(r => r.Name == name);
            if (role != null)
            {
                roles.Remove(role);
            }
            return RedirectToAction("Index");
        }



        public IActionResult AssignRole()
        {
            var users = _userManager.Users.ToList(); // دریافت لیست کاربران
            ViewData["Roles"] = roles.Select(r => r.Name).ToList(); // لیست نقش‌ها
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleName))
            {
                ModelState.AddModelError("", "User ID and Role name must be provided.");
                return View(GetUsersWithRoles());
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            // اگر خطایی وجود داشت
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(GetUsersWithRoles());
        }

        private List<CustomUser> GetUsersWithRoles()
        {
            var users = _userManager.Users.ToList(); // دریافت لیست کاربران
            ViewBag.Roles = roles.Select(r => r.Name).ToList(); // لیست نقش‌ها
            return users;
        }

    }

}
