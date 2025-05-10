using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpPost]
        public async Task<IActionResult> Add(int Id, int quantity)
        {
            ProductModel productModel = await _db.Products.FirstOrDefaultAsync(x => x.Id == Id);

            List<CartItemModel> carts = HttpContext.Session.GetObjectFromJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = carts.FirstOrDefault(x => x.ProductId == Id);

            if (cartItem == null)
            {
                cartItem = new CartItemModel(productModel);
                cartItem.Quantity = quantity;
                carts.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            HttpContext.Session.SetObjectAsJson("Cart", carts);

            TempData["Success"] = "Add item to cart successfully";

            return Json(new { success = true });
        }

        public async Task<IActionResult> Decrease(int Id)
        {
            List<CartItemModel> carts = HttpContext.Session.GetObjectFromJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel? cartItem = carts.FirstOrDefault(x => x.ProductId == Id);

            if (cartItem is not null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                }
                else
                {
                    carts.RemoveAll(x => x.ProductId == Id);
                }
            }

            if (carts.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
                HttpContext.Session.SetObjectAsJson("Cart", carts);

            TempData["Success"] = "Decrease item quantity to cart successfully";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Increase(int Id)
        {
            var product = await _db.Products.FirstOrDefaultAsync(x => x.Id == Id);
            List<CartItemModel> carts = HttpContext.Session.GetObjectFromJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = carts.FirstOrDefault(x => x.ProductId == Id);

            if (cartItem is not null && product.Quantity > cartItem.Quantity)
            {
                if (cartItem.Quantity >= 1)
                {
                    cartItem.Quantity++;
                    TempData["Success"] = "Increase item quantity to cart successfully";
                }
            }
            else
            {
                cartItem.Quantity = product.Quantity;
                TempData["Success"] = "Maximine items quantity to cart successfully";
            }

            if (carts.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
                HttpContext.Session.SetObjectAsJson("Cart", carts);

            
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int Id)
        {
            List<CartItemModel> carts = HttpContext.Session.GetObjectFromJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel? cartItem = carts.FirstOrDefault(x => x.ProductId == Id);

            if (cartItem is not null)
            {
                carts.RemoveAll(x => x.ProductId == Id);
            }

            if (carts.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
                HttpContext.Session.SetObjectAsJson("Cart", carts);

            TempData["Success"] = "Remove item of cart successfully";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Clear()
        {
            HttpContext.Session.Remove("Cart");
            TempData["Success"] = "Remove all item of cart successfully";
            return RedirectToAction("Index");
        }
    }
}
