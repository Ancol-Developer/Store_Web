using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Models;
using ShoppingCart.Repository;
using System.Security.Claims;

namespace ShoppingCart.Controllers
{
	public class CheckoutController : Controller
	{
        private readonly DataContext _db;

        public CheckoutController(DataContext db)
        {
            this._db = db;
        }

        public async Task<IActionResult> Checkout()
		{
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var orderCode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel();

                orderItem.OrderCode = orderCode;
                orderItem.Username = userEmail;
                orderItem.Status = 1;
                orderItem.CreatedDate = DateTime.Now;

                _db.Orders.Add(orderItem);

                List<CartItemModel> cartItems = HttpContext.Session.GetObjectFromJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                foreach (var cart in cartItems)
                {
                    var orderDetails = new OrderDetails();
                    orderDetails.Username = userEmail;
                    orderDetails.OrderCode = orderCode;
                    orderDetails.ProductId = cart.ProductId;
                    orderDetails.Price = cart.Price;
                    orderDetails.Quantity = cart.Quantity;

                    _db.OrderDetails.Add(orderDetails);
                }

                await _db.SaveChangesAsync();

                HttpContext.Session.Remove("Cart");
                TempData["Success"] = "Đơn hàng đã được tạo";

                return RedirectToAction("Index", "Cart");
            }
		}
	}
}
