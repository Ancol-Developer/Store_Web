using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Repository;

namespace ShoppingCart.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly DataContext _db;

        public OrderController(DataContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _db.Orders.OrderByDescending(x => x.Id).ToListAsync();
            return View(items);
        }

        public async Task<IActionResult> ViewOrder(string orderCode)
        {
            var detailsOrder = await _db.OrderDetails.Where(x => x.OrderCode == orderCode).Include(x => x.Product).ToListAsync();
            return View(detailsOrder);
        }
    }
}
