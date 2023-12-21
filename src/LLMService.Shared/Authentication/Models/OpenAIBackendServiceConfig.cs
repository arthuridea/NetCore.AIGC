using LLMService.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.Shared.Authentication.Models
{
    /// <summary>
    /// OpenAI API settings
    /// </summary>
    public class OpenAIBackendServiceConfig: IChatServiceOption
    {
        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        /// <value>
        /// The API key.
        /// </value>
        public string APIKey {  get; set; }
        /// <summary>
        /// Gets or sets the organization identifier.
        /// </summary>
        /// <value>
        /// The organization identifier.
        /// </value>
        public string OrganizationId {  get; set; }

        /// <summary>
        /// Gets or sets the proxy URL.
        /// </summary>
        /// <value>
        /// The proxy URL.
        /// </value>
        public string ProxyUrl {  get; set; }

        /// <summary>
        /// Gets the name of the backend HTTP client.
        /// </summary>
        /// <value>
        /// The name of the backend HTTP client.
        /// </value>
        public string BackendHttpClientName { get; set; } = LLMServiceConsts.OpenAIApiClientName;

        /// <summary>
        /// Gets a value indicating whether [support streaming].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [support streaming]; otherwise, <c>false</c>.
        /// </value>
        public bool SupportStreaming { get; set; } = true;

        /// <summary>
        /// Gets the backend stream end token pattern.
        /// </summary>
        /// <value>
        /// The backend stream end token pattern.
        /// </value>
        public string BackendStreamEndTokenPattern { get; set; } = "[DONE";
        /// <summary>
        /// Gets a value indicating whether this instance is backend stream end standalone.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is backend stream end standalone; otherwise, <c>false</c>.
        /// </value>
        public bool IsBackendStreamEndStandalone => true;
        /// <summary>
        /// Gets the backend stream prefix token.
        /// </summary>
        /// <value>
        /// The backend stream prefix token.
        /// </value>
        public string BackendStreamPrefixToken => "data:";

        /// <summary>
        /// Gets the sse push interval.
        /// </summary>
        /// <value>
        /// The sse push interval.
        /// </value>
        public int SSEPushInterval => 500;
    }
}
