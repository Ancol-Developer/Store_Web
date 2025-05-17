using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            decimal shippingPrice = 0;

            // Nhận Coupon từ Cookie
            var couponCode = Request.Cookies["CouponTitle"];

            if (shippingPriceCookie is not null)
            {
                var shippingPriceJson = shippingPriceCookie;
                shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
            }

            CartItemViewModel cartItemViewModel = new CartItemViewModel
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Total),
                ShippingPrice = shippingPrice,
                CouponCode = couponCode
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

        [HttpPost]
        [Route("Cart/GetShipping")]
        public async Task<IActionResult> GetShipping(ShippingModel shippingModel, string quan, string tinh, string phuong)
        {

            var existingShipping = await _db.Shippings
                .FirstOrDefaultAsync(x => x.City == tinh && x.District == quan && x.Ward == phuong);

            decimal shippingPrice = 0; // Set mặc định giá tiền

            if (existingShipping != null)
            {
                shippingPrice = existingShipping.Price;
            }
            else
            {
                //Set mặc định giá tiền nếu ko tìm thấy
                shippingPrice = 50000;
            }
            var shippingPriceJson = JsonConvert.SerializeObject(shippingPrice);
            try
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                    Secure = true // using HTTPS
                };

                Response.Cookies.Append("ShippingPrice", shippingPriceJson, cookieOptions);
            }
            catch (Exception ex)
            {
                //
                Console.WriteLine($"Error adding shipping price cookie: {ex.Message}");
            }
            return Json(new { shippingPrice });
        }

        [HttpPost]
        [Route("Cart/GetCoupon")]
        public async Task<IActionResult> GetCoupon(CouponModel couponModel, string coupon_value)
        {
            var validCoupon = await _db.Coupons
                .FirstOrDefaultAsync(x => x.Name == coupon_value && x.Quantity >= 1);


            if (validCoupon != null)
            {
                string couponTitle = validCoupon.Name + " | " + validCoupon?.Description;

                TimeSpan remainingTime = validCoupon.EndDate - DateTime.Now;
                int daysRemaining = remainingTime.Days;

                if (daysRemaining >= 0)
                {
                    try
                    {
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                            Secure = true,
                            SameSite = SameSiteMode.Strict // Kiểm tra tính tương thích trình duyệt
                        };

                        Response.Cookies.Append("CouponTitle", couponTitle, cookieOptions);
                        return Ok(new { success = true, message = "Coupon applied successfully" });
                    }
                    catch (Exception ex)
                    {
                        //trả về lỗi 
                        Console.WriteLine($"Error adding apply coupon cookie: {ex.Message}");
                        return Ok(new { success = false, message = "Coupon applied failed" });
                    }
                }
                else
                {

                    return Ok(new { success = false, message = "Coupon has expired" });
                }

            }
            else
            {
                return Ok(new { success = false, message = "Coupon not existed" });
            }
        }


        [HttpGet]
        public IActionResult RemoveShippingCookie()
        {
            Response.Cookies.Delete("ShippingPrice");
            return RedirectToAction("Index", "Cart");
        }
    }
}
