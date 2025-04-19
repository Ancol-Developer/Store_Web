﻿using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Index(string? Slug = null)
        {
            var category = _db.Categories.FirstOrDefault(c => c.Slug == Slug);

            if (category is not null)
            {
                var products = await _db.Products.Where(p => p.CategoryId == category.Id).ToListAsync();
                return View(products.OrderByDescending(p => p.Id));
            }
            return RedirectToAction("Index");
        }
    }
}
