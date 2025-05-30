﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Common;
using ShoppingCart.Models;
using ShoppingCart.Models.Common;
using ShoppingCart.Repository;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShoppingCart.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Employee")]
    public class ProductController : Controller
    {
        private readonly DataContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(DataContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;

        }
        public async Task<IActionResult> Index(int pg = 1)
        {
            var products = await _db.Products.Include(x => x.Category).Include(x => x.Brand).OrderByDescending(c => c.Id).ToListAsync();

            int pageSize = 10;

            if (pg < 1)
            {
                pg = 1;
            }

            int totalItems = products.Count();

            var pager = new Paginate(totalItems, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = products.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(_db.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_db.Brands, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_db.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_db.Brands, "Id", "Name", product.BrandId);

            if (ModelState.IsValid)
            {
                product.Slug = CommonHelper.FilterChar(product.Name);

                var slug = await _db.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);

                if (slug is not null)
                {
                    ModelState.AddModelError("", "Sản phẩm đã có trong database");
                    return View(product);
                }

                if (product.ImageUpload is not null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, @"media\products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();

                    product.Image = imageName;
                }

                await _db.Products.AddAsync(product);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Tạo sản phẩm thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            var product = await _db.Products.FirstOrDefaultAsync(x => x.Id == Id);

            ViewBag.Categories = new SelectList(_db.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_db.Brands, "Id", "Name", product.BrandId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_db.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_db.Brands, "Id", "Name", product.BrandId);

            var existed_product = await _db.Products.FirstOrDefaultAsync(x => x.Id == product.Id);

            if (ModelState.IsValid)
            {
                product.Slug = CommonHelper.FilterChar(product.Name);

                var slug = await _db.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug && p.Id != product.Id);

                if (slug is not null)
                {
                    ModelState.AddModelError("", "Sản phẩm đã có trong database");
                    return View(product);
                }

                if (product.ImageUpload is not null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, @"media\products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();

                    // Delete old Picture
                    string oldFilePath = Path.Combine(uploadDir, existed_product.Image);

                    try
                    {
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                    }

                    existed_product.Image = imageName;
                }

                existed_product.Name = product.Name;
                existed_product.Description = product.Description;
                existed_product.Price = product.Price;
                existed_product.CategoryId = product.CategoryId;
                existed_product.BrandId = product.BrandId;
                existed_product.Slug = product.Slug;
                existed_product.CapitalPrice = product.CapitalPrice;

                _db.Products.Update(existed_product);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Cập nhật sản phẩm thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var product = await _db.Products.FirstOrDefaultAsync(x => x.Id == Id);

            if (!string.Equals(product.Image, "noimage.jpg"))
            {

                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, @"media\products");
                string oldFilePath = Path.Combine(uploadDir, product.Image);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Xóa sản phẩm thành công";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> AddQuantity(int Id)
        {
            var productQuantity = await _db.ProductQuantities.Where(x => x.ProductId == Id).ToListAsync(); ;
            ViewBag.ProductQuantities = productQuantity;
            ViewBag.ProductId = Id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMoreQuantity(ProductQuantityModel productQuantityModel)
        {
            var product = await _db.Products.FirstOrDefaultAsync(x => x.Id == productQuantityModel.ProductId);
            if (product is null)
            {
                return NotFound();
            }

            // Update Product
            product.Quantity += productQuantityModel.Quantity;
            _db.Products.Update(product);

            productQuantityModel.CreatedDate = DateTime.Now;
            productQuantityModel.ProductId = product.Id;

            // Add Product Quantity
            _db.ProductQuantities.Add(productQuantityModel);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Cập nhật số lượng sản phẩm thành công";
            return RedirectToAction("AddQuantity", "Product", new { Id = productQuantityModel.ProductId });
        }
    }
}
