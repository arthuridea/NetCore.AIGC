using LLMService.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LLMService.Shared.Models.LLMApiDefaults;

namespace LLMService.Shared.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TMessageContent">The type of the message content.</typeparam>
    public interface IChatRequest<TMessageContent>
    {
        /// <summary>
        /// Gets or sets the conversation identifier.
        /// </summary>
        /// <value>
        /// The conversation identifier.
        /// </value>
        string ConversationId { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        TMessageContent Message { get; set; }

        /// <summary>
        /// Gets or sets the model schema.
        /// </summary>
        /// <value>
        /// The model schema.
        /// </value>
        LLM_ModelType ModelSchema {  get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IChatRequest{TMessageContent}"/> is stream.
        /// </summary>
        /// <value>
        ///   <c>true</c> if stream; otherwise, <c>false</c>.
        /// </value>
        bool Stream { get; set; }

        /// <summary>
        /// Gets the name of the model.
        /// </summary>
        /// <value>
        /// The name of the model.
        /// </value>
        string ModelSchemaName => ModelSchema.GetAttribute<DisplayAttribute>().Name;
    }
}
