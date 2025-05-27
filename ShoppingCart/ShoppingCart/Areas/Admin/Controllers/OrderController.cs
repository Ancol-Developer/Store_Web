using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Models;
using ShoppingCart.Models.Common;
using ShoppingCart.Repository;

namespace ShoppingCart.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin, Employee")]
	public class OrderController : Controller
	{
		private readonly DataContext _db;

		public OrderController(DataContext db)
		{
			_db = db;
		}

		public async Task<IActionResult> Index(int pg = 1)
		{
			var orders = await _db.Orders.OrderByDescending(c => c.Id).ToListAsync();

			int pageSize = 10;

			if (pg < 1)
			{
				pg = 1;
			}

			int totalItems = orders.Count();

			var pager = new Paginate(totalItems, pg, pageSize);
			int recSkip = (pg - 1) * pageSize;
			var data = orders.Skip(recSkip).Take(pager.PageSize).ToList();

			ViewBag.Pager = pager;

			return View(data);
		}

		public async Task<IActionResult> ViewOrder(string orderCode)
		{
			var detailsOrder = await _db.OrderDetails.Where(x => x.OrderCode == orderCode).Include(x => x.Product).ToListAsync();
			var order = _db.Orders.Where(o => o.OrderCode == orderCode).FirstOrDefault();
			ViewBag.ShippingCost = order.ShippingCost;
			ViewBag.Status = order.Status;
			return View(detailsOrder);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateOrder(string ordercode, int status)
		{
			var orders = await _db.Orders.FirstOrDefaultAsync(x => x.OrderCode == ordercode);

			if (orders is null)
			{
				return Json(new { success = false, message = "Cập nhật trạng thái đơn hàng thất bại" });
			}

			orders.Status = status;
			_db.Orders.Update(orders);

			

			if (status == 0)
			{
				var detailOrders = await _db.OrderDetails
				.Include(od => od.Product)
				.Where(od => od.OrderCode == orders.OrderCode)
				.Select(x => new
				{
					x.Quantity,
					x.Product.Price,
					x.Product.CapitalPrice
				})
				.ToListAsync();

				var statisticModel = await _db.Statistical.FirstOrDefaultAsync(s => s.DateCreate.Date == orders.CreatedDate.Date);

				if (statisticModel is not null)
				{
					foreach (var detailOrder in detailOrders)
					{
						statisticModel.Quantity += 1;
						statisticModel.Sold += detailOrder.Quantity;
						statisticModel.Revenue += detailOrder.Price * detailOrder.Quantity;
						statisticModel.Profit += detailOrder.Price - detailOrder.CapitalPrice;
					}

					_db.Update(statisticModel);
				}
				else
				{
					int new_quantity = 0;
					int new_sold = 0;
					decimal new_profit = 0;
					decimal new_revenue = 0;

					foreach (var orderDetail in detailOrders)
					{
						new_quantity += 1;
						new_sold += orderDetail.Quantity;
						new_profit += orderDetail.Price - orderDetail.CapitalPrice;
						new_revenue += orderDetail.Quantity * orderDetail.Price;

						statisticModel = new StatisticalModel
						{
							DateCreate = orders.CreatedDate,
							Quantity = new_quantity,
							Sold = new_sold,
							Revenue = new_revenue,
							Profit = new_profit
						};
					}

					_db.Add(statisticModel);
				}
			}
			

			await _db.SaveChangesAsync();

			return Json(new { success = true, message = "Cập nhật trạng thái đơn hàng thành công" });
		}

		public async Task<IActionResult> Delete(int Id)
		{
			var order = _db.Orders.FirstOrDefault(o => o.Id == Id);
			_db.Orders.Remove(order);
			await _db.SaveChangesAsync();
			return RedirectToAction("Index");
		}
	}
}
