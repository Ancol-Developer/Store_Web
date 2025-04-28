using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Common;
using ShoppingCart.Models;
using ShoppingCart.Models.Common;
using ShoppingCart.Repository;

namespace ShoppingCart.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Employee")]
    public class BrandController : Controller
    {
        private readonly DataContext _db;

        public BrandController(DataContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(int pg = 1)
        {
            var brands = await _db.Brands.OrderByDescending(c => c.Id).ToListAsync();

            int pageSize = 10;

            if (pg < 1)
            {
                pg = 1;
            }

            int totalItems = brands.Count();

            var pager = new Paginate(totalItems, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = brands.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                brand.Slug = CommonHelper.FilterChar(brand.Name);

                var slug = await _db.Categories.FirstOrDefaultAsync(p => p.Slug == brand.Slug);

                if (slug is not null)
                {
                    ModelState.AddModelError("", "Brand đã có trong database");
                    return View(brand);
                }

                await _db.Brands.AddAsync(brand);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Tạo Brand thành công";
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
            var brand = await _db.Brands.FirstOrDefaultAsync(x => x.Id == Id);
            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BrandModel brand)
        {
            var existed_brand = await _db.Brands.FirstOrDefaultAsync(x => x.Id == brand.Id);

            if (ModelState.IsValid)
            {
                brand.Slug = CommonHelper.FilterChar(brand.Name);

                var slug = await _db.Brands.FirstOrDefaultAsync(p => p.Slug == brand.Slug && p.Id != brand.Id);

                if (slug is not null)
                {
                    ModelState.AddModelError("", "Thương hiệu đã có trong database");
                    return View(brand);
                }

                existed_brand.Name = brand.Name;
                existed_brand.Description = brand.Description;
                existed_brand.Slug = brand.Slug;
                existed_brand.Status = brand.Status;

                _db.Brands.Update(existed_brand);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Cập nhật thương hiệu thành công";
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
            var brand = await _db.Brands.FirstOrDefaultAsync(x => x.Id == Id);

            _db.Brands.Remove(brand);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Xóa thương hiệu thành công";
            return RedirectToAction("Index");
        }
    }
}
