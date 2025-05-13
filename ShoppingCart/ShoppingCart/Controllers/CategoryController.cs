using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Repository;

namespace ShoppingCart.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _db;

        public CategoryController(DataContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(string Slug = null, string sort_by = "", string startprice = "", string endprice = "")
        {
            var category = _db.Categories.FirstOrDefault(c => c.Slug == Slug);

            if (category is not null)
            {
                // Get All Products
                var productsByCategory = _db.Products.Where(p => p.CategoryId == category.Id);
                if (productsByCategory.Count() > 0)
                {
                    // Apply sorting based on sort_by parameter
                    if (sort_by == "price_increase")
                    {
                        productsByCategory = productsByCategory.OrderBy(p => p.Price);
                    }
                    else if (sort_by == "price_decrease")
                    {
                        productsByCategory = productsByCategory.OrderByDescending(p => p.Price);
                    }
                    else if (sort_by == "price_newest")
                    {
                        productsByCategory = productsByCategory.OrderByDescending(p => p.Id);
                    }
                    else if (sort_by == "price_oldest")
                    {
                        productsByCategory = productsByCategory.OrderBy(p => p.Id);
                    }
                    else if (startprice != "" && endprice != "")
                    {
                        decimal startPriceValue;
                        decimal endPriceValue;

                        if (decimal.TryParse(startprice, out startPriceValue) && decimal.TryParse(endprice, out endPriceValue))
                        {
                            productsByCategory = productsByCategory.Where(p => p.Price >= startPriceValue && p.Price <= endPriceValue);
                        }
                        else
                        {
                            productsByCategory = productsByCategory.OrderByDescending(p => p.Id);
                        }
                    }
                    else
                    {
                        productsByCategory = productsByCategory.OrderByDescending(p => p.Id);
                    }

                    if (productsByCategory.Count() > 0)
                    {
                        decimal minPrice = await productsByCategory.MinAsync(p => p.Price);
                        decimal maxPrice = await productsByCategory.MaxAsync(p => p.Price);


                        ViewBag.sort_key = sort_by;

                        ViewBag.count = productsByCategory.Count();

                        ViewBag.minprice = minPrice;
                        ViewBag.maxprice = maxPrice;
                    }
                }

                return View(await productsByCategory.ToListAsync());
            }
            return RedirectToAction("Index");
        }
    }
}
