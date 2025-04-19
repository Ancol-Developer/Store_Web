using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Repository;

namespace ShoppingCart.Controllers
{
    public class BrandsController : Controller
    {
        private readonly DataContext _db;

        public BrandsController(DataContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string? Slug = null)
        {
            var brand = _db.Brands.FirstOrDefault(c => c.Slug == Slug);
            if (brand is not null)
            {
                var producrts = await _db.Products.Where(p => p.BrandId == brand.Id).ToListAsync();
                return View(producrts.OrderByDescending(p => p.Id));
            }
            return RedirectToAction("Index");
        }
    }
}
