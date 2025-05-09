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
    [Authorize(Roles = "Admin")]
    public class ContactController : Controller
    {
        private readonly DataContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ContactController(DataContext db, IWebHostEnvironment webHostEnvironment)
        {
            this._db = db;
            this._webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var contacts = _db.Contacts.ToList();
            return View(contacts);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            ContactModel contact = await _db.Contacts.FirstAsync();
            return View(contact);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ContactModel contact)
        {
            var existed_contact = await _db.Contacts.FirstOrDefaultAsync();

            if (ModelState.IsValid)
            {
                if (contact.ImageUpload is not null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, @"media\logo");
                    string imageName = Guid.NewGuid().ToString() + "_" + contact.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await contact.ImageUpload.CopyToAsync(fs);
                    fs.Close();

                    // Delete old Picture
                    string oldFilePath = Path.Combine(uploadDir, existed_contact.LogoImage);

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

                    existed_contact.LogoImage = imageName;
                }

                existed_contact.Name = contact.Name;
                existed_contact.Description = contact.Description;
                existed_contact.Phone = contact.Phone;
                existed_contact.Map = contact.Map;

                _db.Contacts.Update(existed_contact);
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
    }
}
