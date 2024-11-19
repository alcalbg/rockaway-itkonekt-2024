using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspNetRazor.Pages;

public class PeopleModel : PageModel
{
	public List<string> Names { get;set; } = [
		"John", "Paul", "George", "Ringo" ];

    private readonly ILogger<PrivacyModel> _logger;

    public PeopleModel(ILogger<PrivacyModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}

