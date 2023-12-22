using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LLMService.Shared.Models.LLMApiDefaults;

namespace LLMService.Shared.Models
{
    /// <summary>
    /// 
    /// </summary>
    public interface IChatResponse<TChatApiResponse>
    {
        /// <summary>
        /// Gets or sets the conversation identifier.
        /// </summary>
        /// <value>
        /// The conversation identifier.
        /// </value>
        string ConversationId {  get; set; }
        /// <summary>
        /// Gets or sets the model schema.
        /// </summary>
        /// <value>
        /// The model schema.
        /// </value>
        LLM_ModelType ModelSchema { get; set; }
        /// <summary>
        /// Gets a value indicating whether [need clear history].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [need clear history]; otherwise, <c>false</c>.
        /// </value>
        bool NeedClearHistory { get; }
        /// <summary>
        /// Gets or sets the LLM response data.
        /// </summary>
        /// <value>
        /// The LLM response data.
        /// </value>
        TChatApiResponse LLMResponseData { get; set; }
        /// <summary>
        /// Gets the aigc message.
        /// </summary>
        /// <value>
        /// The aigc message.
        /// </value>
        string AIGCMessage { get; }

    }
}
