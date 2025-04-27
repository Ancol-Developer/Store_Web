using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Common;
using ShoppingCart.Models;
using ShoppingCart.Repository;

namespace ShoppingCart.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Admin, Employee")]
    public class CategoryController : Controller
	{

		private readonly DataContext _db;

		public CategoryController(DataContext db)
		{
			_db = db;
		}
		public async Task<IActionResult> Index()
		{
			var items = await _db.Categories.OrderByDescending(c => c.Id).ToListAsync();
			return View(items);
		}

		[HttpGet]
		public async Task<IActionResult> Create()
		{
			return View();
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = CommonHelper.FilterChar(category.Name);

                var slug = await _db.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);

                if (slug is not null)
                {
                    ModelState.AddModelError("", "Category đã có trong database");
                    return View(category);
                }

                await _db.Categories.AddAsync(category);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Tạo Category thành công";
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
			var category = await _db.Categories.FirstOrDefaultAsync(x => x.Id == Id);
			return View(category);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(CategoryModel category)
		{
			var existed_category = await _db.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);

			if (ModelState.IsValid)
			{
				category.Slug = CommonHelper.FilterChar(category.Name);

				var slug = await _db.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug && p.Id != category.Id);

				if (slug is not null)
				{
					ModelState.AddModelError("", "Danh mục đã có trong database");
					return View(category);
				}

				existed_category.Name = category.Name;
				existed_category.Description = category.Description;
				existed_category.Slug = category.Slug;
				existed_category.Status = category.Status;

				_db.Categories.Update(existed_category);
				await _db.SaveChangesAsync();
				TempData["Success"] = "Cập nhật danh mục thành công";
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
            var category = await _db.Categories.FirstOrDefaultAsync(x => x.Id == Id);

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Xóa danh mục thành công";
            return RedirectToAction("Index");
        }
    }
}
