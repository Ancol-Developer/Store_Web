﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Models;
using ShoppingCart.Models.ViewModel;
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
            var product = await _db.Products
                .Include(x => x.Category)
                .Include(x => x.Brand)
                .Include(x => x.Ratings).FirstOrDefaultAsync(x => x.Id == id);

            var relatedProducts = await _db.Products
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
                .Take(4)
                .ToListAsync();

            ViewBag.RelateProducts = relatedProducts;


            if (product is not null)
            {
                var productDetailViewModel = new ProductDetailViewModel
                {
                    Product = product,
                };

                return View(productDetailViewModel);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommentProduct(RatingModel ratingModel)
        {
            if (ModelState.IsValid)
            {
                var ratingEntity = new RatingModel
                {
                    ProductId = ratingModel.ProductId,
                    Comment = ratingModel.Comment,
                    Name = ratingModel.Name,
                    Email = ratingModel.Email,
                    Star = ratingModel.Star
                };

                _db.Ratings.Add(ratingEntity);
                await _db.SaveChangesAsync();

                TempData["success"] = "Cảm ơn bạn đã đánh giá sản phẩm này!";

                return Redirect(Request.Headers["Referer"]);
            }
            else
            {
                TempData["error"] = "Đánh giá không thành công!";
                return RedirectToAction("Details", new { id = ratingModel.ProductId });
            }
        }
    }
}
