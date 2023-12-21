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
    /// <typeparam name="TChatMessage">The type of the message content.</typeparam>
    public interface IBackendChatRequest<TChatMessage>
    {
        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        List<TChatMessage> Messages { get; set; }
    }
}
