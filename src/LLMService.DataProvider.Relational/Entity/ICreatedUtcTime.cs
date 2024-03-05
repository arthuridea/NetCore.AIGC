using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.DataProvider.Relational.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICreatedUtcTime
    {
        /// <summary>
        /// Gets or sets the created UTC time.
        /// </summary>
        /// <value>
        /// The created UTC time.
        /// </value>
        DateTime CreatedUtcTime { get; set; }
    }
}
