using System.ComponentModel.DataAnnotations;

namespace LLMService.DataProvider.Relational.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class Conversation : Conversation<Guid, string, string>
    {
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TConversationKey">The type of the conversation key.</typeparam>
    /// <typeparam name="TTenantKey">The type of the tenant key.</typeparam>
    /// <typeparam name="TUserKey">The type of the user key.</typeparam>
    public class Conversation<TConversationKey, TTenantKey, TUserKey>: ICreatedUtcTime
        where TConversationKey : IEquatable<TConversationKey>
        where TTenantKey : IEquatable<TTenantKey>
        where TUserKey : IEquatable<TUserKey>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public virtual TConversationKey Id { get; set; }
        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        public virtual TTenantKey TenantId { get; set; }
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public virtual TUserKey UserId { get; set; }
        /// <summary>
        /// Gets or sets the folder identifier.
        /// </summary>
        /// <value>
        /// The folder identifier.
        /// </value>
        public Guid? FolderId {  get; set; }
        /// <summary>
        /// Gets or sets the topic.
        /// </summary>
        /// <value>
        /// The topic.
        /// </value>
        public string Topic { get; set; } = "";
        /// <summary>
        /// Gets or sets the created UTC time.
        /// </summary>
        /// <value>
        /// The created UTC time.
        /// </value>
        public DateTime CreatedUtcTime { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is deleted; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeleted { get; set; } = false;

    }
}
