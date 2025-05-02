using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Repository;

namespace ShoppingCart.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _db;

        public ProductController(DataContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int? id = null)
        {
            var product = await _db.Products.Include(x => x.Category).Include(x => x.Brand).FirstOrDefaultAsync(x => x.Id == id);

            var relatedProducts = await _db.Products.Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id).Take(4).ToListAsync();
            ViewBag.RelateProducts = relatedProducts;

            if (product is not null)
            {
                return View(product);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchTerm)
        {
            var products = await _db.Products.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm)).ToListAsync();

            if (products is null || products.Count == 0)
            {
                products = await _db.Products.ToListAsync();
            }

            ViewBag.KeyWord = searchTerm;
            return View(products);
        }
    }
}
