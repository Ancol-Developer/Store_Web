using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Models;
using ShoppingCart.Services.Mono;

namespace ShoppingCart.Controllers
{
	public class PaymentController : Controller
	{
		private readonly IMomoService _momoService;

		public PaymentController(IMomoService momoService)
        {
			this._momoService = momoService;
		}


		[HttpPost]
		public async Task<IActionResult> CreatePaymentMomo(OrderInforModel model)
		{
			var response = await _momoService.CreatePaymentAsync(model);
			return Redirect(response.PayUrl);
		}


		[HttpGet]
		public IActionResult PaymentCallBack()
		{
			var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
			return View(response);
		}
	}
}
