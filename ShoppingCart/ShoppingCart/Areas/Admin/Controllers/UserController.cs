using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Repository;

namespace ShoppingCart.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        private readonly DataContext _db;

        public UserController(DataContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var items = await _db.Users.OrderByDescending(x => x.Id).ToListAsync();
            return View();
        }
    }
}
