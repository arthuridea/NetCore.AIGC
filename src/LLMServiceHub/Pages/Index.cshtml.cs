using LLMServiceHub.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
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
        //public IList<AuthenticationScheme> ExternalLogins { get; set; }

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
        public void OnGet(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

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

            //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = ValidateUser(Input.UserName, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    // set authentication cookie
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, Input.UserName),
                        new Claim(ClaimTypes.Role, "User")
                    };
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = Input.RememberMe,//是否持久化

                        //如果用户点“登录“进来，登录成功后跳转到首页，否则跳转到上一个页面
                        RedirectUri = string.IsNullOrWhiteSpace(returnUrl) ? "/Index" : returnUrl,
                    };
                    var identity = new ClaimsIdentity(claims, AppConsts.DefaultAuthScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(AppConsts.DefaultAuthScheme, principal, authProperties);
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

        private (bool Succeeded, string Message) ValidateUser(string username, string password,  bool persist = false)
        {
            var ret = (Succeeded: false, Message: "用户不存在");
            var validusers = _configuration.GetSection("Users")
                                           .Get<List<BuildInUser>>();
            if(validusers.Any(x=> x.UserName== username && x.Password == password))
            {
                ret.Succeeded = true;
                ret.Message = "Ok";
            }
            return ret;
        }

    }
}