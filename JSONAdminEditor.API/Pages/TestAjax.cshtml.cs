using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JSONAdminEditor.Pages;

public class TestAjaxModel : PageModel
{
    public void OnGet()
    {
    }

    public IActionResult OnGetTestEndpoint(string term)
    {
        return new JsonResult(new { message = $"Hello from AJAX endpoint with term: {term}" });
    }
}
