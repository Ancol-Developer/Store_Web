using Microsoft.AspNetCore.Mvc;

namespace ShoppingCart.Controllers
{
	public class CheckoutController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
