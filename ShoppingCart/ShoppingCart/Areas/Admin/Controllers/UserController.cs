using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Models;
using ShoppingCart.Repository;

namespace ShoppingCart.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly DataContext _db;
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<AppRoleModel> _roleManager;

        public UserController(DataContext db,UserManager<AppUserModel> userManager, RoleManager<AppRoleModel> roleManager)
        {
            this._db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var userWithRoles = await (from u in _db.Users
                                       join ur in _db.UserRoles on u.Id equals ur.UserId
                                       join r in _db.Roles on ur.UserId equals r.Id
                                       select new { User = u, RoleName = r.Name})
                                       .ToListAsync();
            return View(userWithRoles);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppUserModel appUserModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(appUserModel, appUserModel.PasswordHash);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(appUserModel.UserName);

                    var role = await _roleManager.FindByIdAsync(appUserModel.RoleId);

                    var addRoleResult = await _userManager.AddToRoleAsync(user, role.Name);
                    if (!addRoleResult.Succeeded)
                    {
                        AddIdentityErrors(addRoleResult);
                    }

                    return RedirectToAction("Index", "User");
                }
                else
                {
                    var roles = await _roleManager.Roles.ToListAsync();
                    ViewBag.Roles = new SelectList(roles, "Id", "Name");
                    AddIdentityErrors(result);
                    return View(appUserModel);
                }
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, AppUserModel user)
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser is null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.RoleId = user.RoleId;
                var result = await _userManager.UpdateAsync(existingUser);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    AddIdentityErrors(result);
                    return View(user);
                }
            }

            TempData["error"] = "Model có một vài thứ đang bị lỗi";
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            string errorMessage = string.Join("\n", errors);
            return View(existingUser);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            var deleteResult = await _userManager.DeleteAsync(user);

            if (!deleteResult.Succeeded)
            {
                return View("Error");
            }

            TempData["Success"] = "Đã xóa user thành công";
            return RedirectToAction("Index");
        }
        private void AddIdentityErrors(IdentityResult identityResult)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
