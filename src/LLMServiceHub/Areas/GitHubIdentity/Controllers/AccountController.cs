using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authorization;
using LLMServiceHub.Common;

#nullable enable
namespace LLMServiceHub.Areas.GitHubIdentity.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    //[NonController]
    [AllowAnonymous]
    [Area("GitHubIdentity")]
    [Route("[area]/[controller]/[action]")]
    public class AccountController : Controller
    {
        /// <summary>
        /// Handles user sign in.
        /// </summary>
        /// <param name="scheme">Authentication scheme.</param>
        /// <param name="redirectUri">Redirect URI.</param>
        /// <returns>Challenge generating a redirect to Azure AD to sign in the user.</returns>
        [HttpGet("{scheme?}")]
        public IActionResult SignIn(
            [FromRoute] string scheme,
            [FromQuery] string redirectUri)
        {
            scheme ??= AppConsts.GitHubAuthScheme;
            string redirect;
            if (!string.IsNullOrEmpty(redirectUri) && Url.IsLocalUrl(redirectUri))
            {
                redirect = redirectUri;
            }
            else
            {
                redirect = Url.Content("~/")!;
            }

            return Challenge(
                new AuthenticationProperties { RedirectUri = redirect },
                scheme);
        }

        /// <summary>
        /// Challenges the user.
        /// </summary>
        /// <param name="redirectUri">Redirect URI.</param>
        /// <param name="scope">Scopes to request.</param>
        /// <param name="loginHint">Login hint.</param>
        /// <param name="domainHint">Domain hint.</param>
        /// <param name="claims">Claims.</param>
        /// <param name="policy">AAD B2C policy.</param>
        /// <param name="scheme">Authentication scheme.</param>
        /// <returns>Challenge generating a redirect to Azure AD to sign in the user.</returns>
        [HttpGet("{scheme?}")]
        public IActionResult Challenge(
            string redirectUri,
            string scope,
            string loginHint,
            string domainHint,
            string claims,
            string policy,
            [FromRoute] string scheme)
        {
            scheme ??= AppConsts.GitHubAuthScheme;
            Dictionary<string, string?> items = new Dictionary<string, string?>
            {
                { Constants.Claims, claims },
                { Constants.Policy, policy },
            };
            Dictionary<string, object?> parameters = new Dictionary<string, object?>
            {
                { Constants.LoginHint, loginHint },
                { Constants.DomainHint, domainHint },
            };

            OAuthChallengeProperties oAuthChallengeProperties = new OAuthChallengeProperties(items, parameters);
            if (scope != null)
            {
                oAuthChallengeProperties.Scope = scope.Split(" ");
            }
            oAuthChallengeProperties.RedirectUri = redirectUri;

            return Challenge(
                oAuthChallengeProperties,
                scheme);
        }

        /// <summary>
        /// Handles the user sign-out.
        /// </summary>
        /// <param name="scheme">Authentication scheme.</param>
        /// <returns>Sign out result.</returns>
        [HttpGet("{scheme?}")]
        public IActionResult SignOut(
            [FromRoute] string scheme)
        {
            if (AppServicesAuthenticationInformation.IsAppServicesAadAuthenticationEnabled)
            {
                if (AppServicesAuthenticationInformation.LogoutUrl != null)
                {
                    return LocalRedirect(AppServicesAuthenticationInformation.LogoutUrl);
                }
                return Ok();
            }
            else
            {
                scheme ??= AppConsts.GitHubAuthScheme;
                var callbackUrl = Url.Page("/Account/SignedOut", pageHandler: null, values: null, protocol: Request.Scheme);
                return SignOut(
                     new AuthenticationProperties
                     {
                         RedirectUri = callbackUrl,
                     },
                     CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
    }
}
#nullable disable