using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.Shared.Models
{
    /// <summary>
    /// 
    /// </summary>
    public interface IChatServiceOption
    {
        /// <summary>
        /// Gets the name of the backend HTTP client.
        /// </summary>
        /// <value>
        /// The name of the backend HTTP client.
        /// </value>
        string BackendHttpClientName { get; }
        /// <summary>
        /// Gets a value indicating whether [support streaming].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [support streaming]; otherwise, <c>false</c>.
        /// </value>
        bool SupportStreaming { get; }

        /// <summary>
        /// Gets the backend stream end token pattern.
        /// </summary>
        /// <value>
        /// The backend stream end token pattern.
        /// </value>
        string BackendStreamEndTokenPattern {  get; }
        /// <summary>
        /// Gets a value indicating whether this instance is backend stream end standalone.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is backend stream end standalone; otherwise, <c>false</c>.
        /// </value>
        bool IsBackendStreamEndStandalone { get; }
        /// <summary>
        /// Gets the backend stream prefix token.
        /// </summary>
        /// <value>
        /// The backend stream prefix token.
        /// </value>
        string BackendStreamPrefixToken {  get; }
        /// <summary>
        /// Gets the sse push interval.
        /// </summary>
        /// <value>
        /// The sse push interval.
        /// </value>
        int SSEPushInterval { get; }
    }
}
