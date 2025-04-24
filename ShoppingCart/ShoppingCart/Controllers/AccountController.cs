using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Models;
using ShoppingCart.Models.ViewModel;

namespace ShoppingCart.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        public AccountController(UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;

        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                AppUserModel newUser = new AppUserModel
                {
                    UserName = userViewModel.Username,
                    Email = userViewModel.Email,
                };

                IdentityResult result = await _userManager.CreateAsync(newUser);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Tạo User thành công";
                    return Redirect("/Home");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(userViewModel);
        }
    }
}
