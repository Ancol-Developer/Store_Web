using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Models;
using ShoppingCart.Repository;

namespace ShoppingCart.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class DashboardController : Controller
	{
		private readonly DataContext _db;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public DashboardController(DataContext db, IWebHostEnvironment webHostEnvironment)
		{
			_db = db;
			_webHostEnvironment = webHostEnvironment;

		}
		public IActionResult Index()
		{
            var count_product = _db.Products.Count();
            var count_order = _db.Orders.Count();
            var count_category = _db.Categories.Count();
            var count_user = _db.Users.Count();
            ViewBag.CountProduct = count_product;
            ViewBag.CountOrder = count_order;
            ViewBag.CountCategory = count_category;
            ViewBag.CountUser = count_user;
            return View();
        }

		[HttpPost]
		[Route("GetChartData")]
		public IActionResult GetChartData()
		{

			var chartData = _db.Statistical.Select(s => new
				{
					date = s.DateCreate.ToString("yyyy-MM-dd"),
					sold = s.Sold,
					quantity = s.Quantity,
					revenue = s.Revenue,
					profit = s.Profit,
				}).ToList();

			return Json(chartData);
		}

        [HttpPost]
        [Route("GetChartDataBySelect")]
        public IActionResult GetChartDataBySelect(DateTime startDate, DateTime endDate)
        {

            var chartData = _db.Statistical.Where(s => s.DateCreate >= startDate && s.DateCreate <= endDate)
                .Select(s => new
                {
                    date = s.DateCreate.ToString("yyyy-MM-dd"),
                    sold = s.Sold,
                    quantity = s.Quantity,
                    revenue = s.Revenue,
                    profit = s.Profit,
                }).ToList();

            return Json(chartData);
        }

    }
}
