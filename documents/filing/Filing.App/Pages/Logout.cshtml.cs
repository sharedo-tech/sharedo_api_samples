using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Filing.App.Pages
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                await HttpContext.SignOutAsync();
            }

            return Redirect("/");
        }
    }
}
