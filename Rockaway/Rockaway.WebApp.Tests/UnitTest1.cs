using Microsoft.AspNetCore.Mvc;
using Rockaway.WebApp.Controllers;

namespace Rockaway.WebApp.Tests;

public class GreetingControllerTests {
	[Fact]
	public void GreetingController_Index_Returns_View() {
		var c = new GreetingController();
		var result = c.Index() as ViewResult;
		Assert.NotNull(result);
	}
}