using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Models;
using ShoppingCart.Models.ViewModel;
using ShoppingCart.Repository;

namespace ShoppingCart.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _db;

        public CartController(DataContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetObjectFromJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemViewModel cartItemViewModel = new CartItemViewModel
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Total)
            };
            return View(cartItemViewModel);
        }
    }
}
