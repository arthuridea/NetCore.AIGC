using System.Text.Json.Serialization;

namespace LLMService.Shared.Authentication.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class TokenErrorResponse
    {
        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        [JsonPropertyName("error")]
        public string Error { get; set; }
    }
}
