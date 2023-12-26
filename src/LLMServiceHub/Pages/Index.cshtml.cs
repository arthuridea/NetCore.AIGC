using LLMServiceHub.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Polly;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LLMServiceHub.Pages
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.RazorPages.PageModel" />
    public class IndexModel : PageModel
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            public string UserName { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="IndexModel"/> class.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="logger">The logger.</param>
        public IndexModel(
            IConfiguration config, 
            ILogger<IndexModel> logger)
        {
            _configuration = config;
            _logger = logger;
        }

        /// <summary>
        /// Called when [get].
        /// </summary>
        public async Task OnGet(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            if(!User.Identity.IsAuthenticated)
            {
                var result = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);
                if (result.Succeeded)
                {
                    await ExecSignIn(AppConsts.DefaultAuthScheme, result.Principal.Identity.Name, true, returnUrl);
                    Redirect(returnUrl);
                }
            }

            ReturnUrl = returnUrl;
        }

        /// <summary>
        /// Called when [post asynchronous].
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = ValidateUser(Input.UserName, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    await ExecSignIn(AppConsts.DefaultAuthScheme, result.IdentityName, Input.RememberMe, returnUrl);
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task ExecSignIn(string scheme, string identityUserName, bool rememberme, string returnUrl)
        {
            // set authentication cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, identityUserName),
                new Claim(ClaimTypes.Role, "User")
            };
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberme,// persistance.
                RedirectUri = string.IsNullOrWhiteSpace(returnUrl) ? "/Index" : returnUrl,
            };
            var identity = new ClaimsIdentity(claims, scheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(scheme, principal, authProperties);
        }

        private (bool Succeeded, string IdentityName, string Message) ValidateUser(string username, string password)
        {
            var ret = (Succeeded: false, IdentityName: "", Message: "用户不存在");
            var validusers = _configuration.GetSection("Users")
                                           .Get<List<BuildInUser>>();
            var user = validusers.FirstOrDefault(x => x.UserName == username && x.Password == password);
            if (user != null)
            {
                ret.Succeeded = true;
                ret.IdentityName = user.DisplayName;
                ret.Message = "ok";
            }
            return ret;
        }

    }
}