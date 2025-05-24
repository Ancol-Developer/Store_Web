using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Models;
using ShoppingCart.Models.ViewModel;
using ShoppingCart.Repository;

namespace ShoppingCart.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly DataContext _db;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, DataContext db, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this._db = db;
            this._emailSender = emailSender;
        }
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl,
            });
        }

        [HttpGet]
        public async Task<IActionResult> NewPassword(AppUserModel user, string token)
        {
            var checkuser = await _userManager.Users
                .Where(u => u.Email == user.Email)
                .Where(u => u.Token == user.Token).FirstOrDefaultAsync();

            if (checkuser != null)
            {
                ViewBag.Email = checkuser.Email;
                ViewBag.Token = token;
            }
            else
            {
                TempData["error"] = "Email not found or token is not right";
                return RedirectToAction("ForgotPass", "Account");
            }
            return View();
        }

        public async Task<IActionResult> SendMailForgotPass(AppUserModel user)

        {
            var checkMail = await _db.Users.FirstOrDefaultAsync(x => x.Email == user.Email);

            if (checkMail is null)
            {
                TempData["error"] = "Email is not found";
                return RedirectToAction("ForgotPassword", "Account");
            }
            else
            {
                string token = Guid.NewGuid().ToString();

                checkMail.Token = token;
                _db.Users.Update(user);
                await _db.SaveChangesAsync();

                var receiver = checkMail.Email;
                var subject = "Change password for user " + checkMail.Email;
                var message = "Click on link to change password " +
                    "<a href='" + $"{Request.Scheme}://{Request.Host}/Account/NewPass?email=" + checkMail.Email + "&token=" + token + "'>";

                await _emailSender.SendEmailAsync(receiver, subject, message);
            }

            TempData["success"] = "An email has been sent to your registered email address with password reset instructions.";
            return RedirectToAction("ForgotPass", "Account");

        }

        [HttpPost]
        public async Task<IActionResult> UpdateNewPassword(AppUserModel user, string token)
        {
            var checkuser = await _userManager.Users
                .Where(u => u.Email == user.Email)
                .Where(u => u.Token == user.Token).FirstOrDefaultAsync();

            if (checkuser != null)
            {
                //update user with new password and token
                string newtoken = Guid.NewGuid().ToString();
                // Hash the new password
                var passwordHasher = new PasswordHasher<AppUserModel>();
                var passwordHash = passwordHasher.HashPassword(checkuser, user.PasswordHash);

                checkuser.PasswordHash = passwordHash;
                checkuser.Token = newtoken;

                await _userManager.UpdateAsync(checkuser);
                TempData["success"] = "Password updated successfully.";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                TempData["error"] = "Email not found or token is not right";
                return RedirectToAction("ForgotPass", "Account");
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginViewModel.Username, loginViewModel.Password, false, false);
                if (result.Succeeded)
                {
                    return Redirect(loginViewModel.ReturnUrl ?? "/");
                }

                ModelState.AddModelError("", "Invalid Username and Password");
            }
            return View(loginViewModel);
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

                IdentityResult result = await _userManager.CreateAsync(newUser, userViewModel.Password);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(userViewModel.Username);

                    var addRoleResult = await _userManager.AddToRoleAsync(user, "User");

                    TempData["Success"] = "Tạo User thành công";
                    return Redirect("/account/login");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(userViewModel);
        }

        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();
            return Redirect(returnUrl);
        }

        public async Task<IActionResult> History()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var useId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var orders = await _db.Orders.Where(x => x.Username == userEmail).OrderByDescending(od => od.Id).ToListAsync();

            ViewBag.UserEmail = userEmail;

            return View(orders);
        }

        public async Task<IActionResult> CancelOrder(string ordercode)
        {
            if ((bool)!User.Identity?.IsAuthenticated)
            {
                // User is not logged in, redirect to login
                return RedirectToAction("Login", "Account");
            }
            try
            {
                var order = await _db.Orders.Where(o => o.OrderCode == ordercode).FirstAsync();
                order.Status = 3;
                _db.Update(order);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred while canceling the order.");
            }

            return RedirectToAction("History", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateAccount()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var useId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

            if (user is null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInforAccount(AppUserModel user)
        {
            var useId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var userByEmail = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

            if (userByEmail is null)
            {
                return NotFound();
            }
            else
            {
                userByEmail.PhoneNumber = user.PhoneNumber;
                userByEmail.UserName = user.UserName;

                var passwordHasher = new PasswordHasher<AppUserModel>();
                var passwordHash = passwordHasher.HashPassword(user, user.PasswordHash);
                userByEmail.PasswordHash = passwordHash;

                _db.Update(userByEmail);
                await _db.SaveChangesAsync();

                TempData["success"] = "Update Account Information Success";

                return RedirectToAction("UpdateAccount", "Account");
            }
        }

        public async Task LoginByGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return RedirectToAction("Login");
            }

            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });

            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            string emailName = email.Split('@')[0];
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                var passwordHasher = new PasswordHasher<AppUserModel>();
                var hashedPassword = passwordHasher.HashPassword(null, "123456789");
                // Create new user
                var newUser = new AppUserModel
                {
                    UserName = emailName,
                    Email = email
                };

                newUser.PasswordHash = hashedPassword;

                var resultCreate = await _userManager.CreateAsync(newUser);
                if (resultCreate.Succeeded)
                {
                    // Add user to role
                    await _userManager.AddToRoleAsync(newUser, "User");
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    TempData["success"] = "Đăng ký thành công bằng tài khoản Google";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["error"] = "Đăng ký không thành công bằng tài khoản Google";
                    return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                // Sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
