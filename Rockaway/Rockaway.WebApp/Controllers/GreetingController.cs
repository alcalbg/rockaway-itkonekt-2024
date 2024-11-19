using Microsoft.AspNetCore.Mvc;

namespace Rockaway.WebApp.Controllers {
	public class GreetingController : Controller {
		public IActionResult Index() {
			return View();
		}
	}
}
