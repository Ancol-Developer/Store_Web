using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Models;
using ShoppingCart.Repository;

namespace ShoppingCart.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Employee")]
    public class ShippingController : Controller
    {
        private readonly DataContext _db;

        public ShippingController(DataContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var shippingList = await _db.Shippings.ToListAsync();
            ViewBag.Shippings = shippingList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StoreShipping(ShippingModel shippingModel, string phuong, string quan, string tinh, decimal price)
        {

            shippingModel.City = tinh;
            shippingModel.District = quan;
            shippingModel.Ward = phuong;
            shippingModel.Price = price;

            try
            {

                var existingShipping = await _db.Shippings
                    .AnyAsync(x => x.City == tinh && x.District == quan && x.Ward == phuong);

                if (existingShipping)
                {
                    return Ok(new { duplicate = true, message = "Dữ liệu trùng lặp." });
                }
                _db.Shippings.Add(shippingModel);
                await _db.SaveChangesAsync();
                return Ok(new { success = true, message = "Thêm shipping thành công" });
            }
            catch (Exception)
            {

                return StatusCode(500, "An error occurred while adding shipping.");
            }
        }
        public async Task<IActionResult> Delete(int Id)
        {
            ShippingModel shipping = await _db.Shippings.FindAsync(Id);

            _db.Shippings.Remove(shipping);
            await _db.SaveChangesAsync();
            TempData["success"] = "Shipping đã được xóa thành công";
            return RedirectToAction("Index");
        }
    }
}
