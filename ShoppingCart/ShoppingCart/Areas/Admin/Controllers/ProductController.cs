using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Models;
using ShoppingCart.Repository;

namespace ShoppingCart.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly DataContext _db;

        public ProductController(DataContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var items= await _db.Products.OrderByDescending(x => x.Id).Include(x => x.Category).Include(x => x.Brand).ToListAsync();
            return View(items);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(_db.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_db.Brands, "Id", "Name");
            return View();
        }

        public async Task<IActionResult> Create(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_db.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_db.Brands, "Id", "Name", product.BrandId);
            return View();
        }
    }
}
