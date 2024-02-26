using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LLMServiceHub.Pages
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.RazorPages.PageModel" />
    [Authorize]
    public class PaintModel : PageModel
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<PaintModel> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaintModel"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public PaintModel(ILogger<PaintModel> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// Called when [get].
        /// </summary>
        public void OnGet()
        {
        }
    }
}
