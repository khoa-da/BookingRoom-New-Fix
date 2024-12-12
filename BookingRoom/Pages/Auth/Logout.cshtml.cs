using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingRoom.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Clear the session data
            HttpContext.Session.Clear();

            // Redirect to the login page
            return RedirectToPage("/Auth/Login");
        }
    }
}
