namespace Divar.Controllers
{
    public class RolesController : Controller

    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<CustomUser> _userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<CustomUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }



        //List of roles         /Roles/List
        public IActionResult List()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }



        //create roles

        // Create action (GET)
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var identityRole = new IdentityRole(model.Name);
                var result = await _roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("List");
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View(model);
        }


        // ویرایش نقش (GET)
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var model = new EditRoleViewModel { Id = role.Id, Name = role.Name };
            return View(model);
        }

        // ویرایش نقش (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.Id);
                if (role == null)
                {
                    return NotFound();
                }

                role.Name = model.Name;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("List");
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View(model);
        }

        // حذف نقش
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("List");
            }
            return View(role);
        }


        //add role to users
        public IActionResult SelectUser()
        {
            // نمایش لیست کاربران برای انتخاب
            var users = _userManager.Users.ToList();
            return View(users);
        }

        public async Task<IActionResult> ManageRole(string id)
        {
            if (id == null)
            {
                return RedirectToAction("SelectUser");
            }

            // Find the user by Id
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Get user roles and all roles
            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

            // Create the ManageRoleViewModel including additional fields
            var model = new ManageRoleViewModel
            {
                CustomUserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserRoles = userRoles.ToList(),
                AllRoles = allRoles
            };

            return View(model);
        }


        // مدیریت نقش کاربران (POST)
        [HttpPost]
        public async Task<IActionResult> ManageRole(ManageRoleViewModel model)
        {
            // پیدا کردن کاربر بر اساس Id
            var user = await _userManager.FindByIdAsync(model.CustomUserId);
            if (user == null)
            {
                return NotFound();
            }

            // گرفتن نقش‌های کاربر فعلی
            var userRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = model.Roles.Except(userRoles).ToList();
            var rolesToRemove = userRoles.Except(model.Roles).ToList();

            // اضافه کردن نقش‌ها
            foreach (var role in rolesToAdd)
            {
                await _userManager.AddToRoleAsync(user, role);
            }

            // حذف نقش‌ها
            foreach (var role in rolesToRemove)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            return RedirectToAction("List");
        }
    }

}