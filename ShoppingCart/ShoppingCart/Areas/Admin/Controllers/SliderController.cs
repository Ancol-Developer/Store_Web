using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Common;
using ShoppingCart.Models;
using ShoppingCart.Repository;

namespace ShoppingCart.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController : Controller
    {
        private readonly DataContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SliderController(DataContext db, IWebHostEnvironment webHostEnvironment)
        {
            this._db = db;
            this._webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var sliders = await _db.Sliders.OrderByDescending(x => x.Id).ToListAsync();
            return View(sliders);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SliderModel slider)
        {
            if (ModelState.IsValid)
            {

                if (slider.ImageUpload is not null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, @"media\sliders");
                    string imageName = Guid.NewGuid().ToString() + "_" + slider.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await slider.ImageUpload.CopyToAsync(fs);
                    fs.Close();

                    slider.Image = imageName;
                }

                await _db.Sliders.AddAsync(slider);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Tạo slider thành công";
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
        public IActionResult Edit(int Id)
        {
            var slider = _db.Sliders.FirstOrDefault(x => x.Id == Id);
            return View(slider);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SliderModel slider)
        {
            var existed_slider = await _db.Sliders.FirstOrDefaultAsync(x => x.Id == slider.Id);

            if (ModelState.IsValid)
            {
                if (slider.ImageUpload is not null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, @"media\sliders");
                    string imageName = Guid.NewGuid().ToString() + "_" + slider.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await slider.ImageUpload.CopyToAsync(fs);
                    fs.Close();

                    // Delete old Picture
                    string oldFilePath = Path.Combine(uploadDir, existed_slider.Image);

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

                    existed_slider.Image = imageName;
                }

                existed_slider.Name = slider.Name;
                existed_slider.Description = slider.Description;
                existed_slider.Status = slider.Status;

                _db.Sliders.Update(existed_slider);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Cập nhật slider thành công";
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
            var slider = await _db.Sliders.FirstOrDefaultAsync(x => x.Id == Id);

            if (!string.Equals(slider.Image, "noimage.jpg"))
            {

                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, @"media\sliders");
                string oldFilePath = Path.Combine(uploadDir, slider.Image);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            _db.Sliders.Remove(slider);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Xóa slider thành công";
            return RedirectToAction("Index");
        }
    }
}
