using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Models.Common;
using ShoppingCart.Repository;

namespace ShoppingCart.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Employee")]
    public class OrderController : Controller
    {
        private readonly DataContext _db;

        public OrderController(DataContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int pg = 1)
        {
            var orders = await _db.Orders.OrderByDescending(c => c.Id).ToListAsync();

            int pageSize = 10;

            if (pg < 1)
            {
                pg = 1;
            }

            int totalItems = orders.Count();

            var pager = new Paginate(totalItems, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = orders.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }

        public async Task<IActionResult> ViewOrder(string orderCode)
        {
            var detailsOrder = await _db.OrderDetails.Where(x => x.OrderCode == orderCode).Include(x => x.Product).ToListAsync();
            return View(detailsOrder);
        }
    }
}
