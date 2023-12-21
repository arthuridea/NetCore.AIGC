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
    public interface IChatMessage : IChatMessage<string>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContent">The type of the content.</typeparam>
    public interface IChatMessage<TContent>
    {
        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        string Role { get; set; }
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        TContent Content { get; set; }
    }
}
