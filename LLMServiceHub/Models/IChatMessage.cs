namespace LLMServiceHub.Models
{
    /// <summary>
    /// 
    /// </summary>
    public interface IChatMessage
    {
        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        string Role {  get; set; }
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        string Content { get; set; }
    }
}
