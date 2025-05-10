using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Areas.Admin.Repository;
using ShoppingCart.Models;
using ShoppingCart.Repository;
using System.Security.Claims;

namespace ShoppingCart.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _db;
        private readonly IEmailSender _emailSender;

        public CheckoutController(DataContext db, IEmailSender emailSender)
        {
            this._db = db;
            this._emailSender = emailSender;
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

                    // Update Product Quantity
                    var product = await _db.Products.FirstOrDefaultAsync(x => x.Id == cart.ProductId);
                    product.Quantity -= cart.Quantity;
                    product.Sold += cart.Quantity;

                    _db.OrderDetails.Add(orderDetails);
                    _db.Products.Update(product);
                }
                await _db.SaveChangesAsync();

                HttpContext.Session.Remove("Cart");

                // SendEmail
                string recipient = userEmail;
                var subject = "Đặt hàng thành công";
                var message = $"Đơn hàng của bạn đã được tạo với mã đơn hàng: {orderCode}";
                //await _emailSender.SendEmailAsync(recipient, subject, message);

                TempData["Success"] = "Đơn hàng đã được tạo";

                return RedirectToAction("Index", "Cart");
            }
        }
    }
}
