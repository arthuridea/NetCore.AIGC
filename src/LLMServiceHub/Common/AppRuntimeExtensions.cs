using Microsoft.AspNetCore.Authentication;

namespace LLMServiceHub.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class AppRuntimeExtensions
    {
        /// <summary>
        /// Gets the external providers asynchronous.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static async Task<AuthenticationScheme[]> GetExternalProvidersAsync(this HttpContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var schemes = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();

            return (from scheme in await schemes.GetAllSchemesAsync()
                    where !string.IsNullOrEmpty(scheme.DisplayName)
                    select scheme).ToArray();
        }
    }
}
