using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LLMServiceHub.Areas.MicrosoftIdentity.Pages.Account
{
    /// <summary>
    /// Model for the SignOut page.
    /// </summary>
    [AllowAnonymous]
    public class SignedOutModel : PageModel
    {
        /// <summary>
        /// Method handling the HTTP GET method.
        /// </summary>
        /// <returns>A Sign Out page or Home page.</returns>
        public IActionResult OnGet()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                //return LocalRedirect("~/");
            }

            //return Page();
            var returnUrl = HttpContext.Request.Query["returnUrl"];
            if(string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "~/";
            }
            return LocalRedirect(returnUrl);
        }
    }
}
