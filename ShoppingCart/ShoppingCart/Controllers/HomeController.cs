using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Models;
using ShoppingCart.Repository;
using System.Diagnostics;

namespace ShoppingCart.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _db;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUserModel> _userManager;

        public HomeController(DataContext db ,ILogger<HomeController> logger, UserManager<AppUserModel> userManager)
        {
            _db = db;
            _logger = logger;
            this._userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _db.Products.Include(x => x.Category).Include(x => x.Brand).ToListAsync();
            var slider = await _db.Sliders.Where(s => s.Status == 1).ToListAsync();
            ViewBag.Slider = slider;
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> AddWishList(int Id)
        {
            var user = await _userManager.GetUserAsync(User);
            WishListModel wishList = new();
            wishList.ProductId = Id;
            wishList.UserId = user.Id;
            _db.WishLists.Add(wishList);

            try
            {
                await _db.SaveChangesAsync();
                return Ok(new { success= true, message="Add to Wishlist Successfully"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCompare(int Id)
        {
            var user = await _userManager.GetUserAsync(User);
            CompareModel compare = new();
            compare.ProductId = Id;
            compare.UserId = user.Id;
            _db.Compares.Add(compare);

            try
            {
                await _db.SaveChangesAsync();
                return Ok(new { success = true, message = "Add to Wishlist Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public async Task<IActionResult> Contact()
        {
            var contact = await _db.Contacts.FirstOrDefaultAsync();
            return View(contact);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int StatusCode)
        {
            if (StatusCode == 404)
            {
                return View("NotFound");
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
