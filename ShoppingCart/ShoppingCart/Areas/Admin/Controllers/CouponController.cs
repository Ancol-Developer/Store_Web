using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Models;
using ShoppingCart.Repository;

namespace ShoppingCart.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Employee")]
    public class CouponController : Controller
    {
        private readonly DataContext _db;

        public CouponController(DataContext db)
        {
            this._db = db;
        }
        public async Task<IActionResult> Index()
        {
            var coupon_list = await _db.Coupons.ToListAsync();
            ViewBag.Coupons = coupon_list;
            return View();
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponModel coupon)
        {
            if (ModelState.IsValid)
            {

                _db.Coupons.Add(coupon);
                await _db.SaveChangesAsync();
                TempData["success"] = "Thêm coupon thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang lỗi";
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
    }
}
