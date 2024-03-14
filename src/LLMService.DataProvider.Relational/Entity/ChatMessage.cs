using static LLMService.DataProvider.Relational.Entity.EntityConsts;
using static LLMService.Shared.Models.LLMApiDefaults;

namespace LLMService.DataProvider.Relational.Entity
{

    public class ChatMessage: ChatMessage<Guid, Guid, string, string>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TTenantKey">The type of the tenant key.</typeparam>
    /// <typeparam name="TMessageKey">The type of the message key.</typeparam>
    /// <typeparam name="TConversationKey">The type of the conversation key.</typeparam>
    /// <typeparam name="TUserKey">The type of the user key.</typeparam>
    public class ChatMessage<TMessageKey, TConversationKey, TTenantKey, TUserKey>: ICreatedUtcTime
        where TTenantKey : IEquatable<TTenantKey>
        where TMessageKey : IEquatable<TMessageKey>
        where TConversationKey : IEquatable<TConversationKey>
        where TUserKey : IEquatable<TUserKey>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public TMessageKey Id { get; set; }
        /// <summary>
        /// Gets or sets the conversation identifier.
        /// </summary>
        /// <value>
        /// The conversation identifier.
        /// </value>
        public TConversationKey ConversationId { get; set; }
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
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        public string Role { get; set; } = "user";
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Content { get; set; } = "";
        /// <summary>
        /// Gets or sets the display content.
        /// </summary>
        /// <value>
        /// The display content.
        /// </value>
        public string DisplayContent {  get; set; } = "";
        /// <summary>
        /// Gets or sets a value indicating whether [use display content].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use display content]; otherwise, <c>false</c>.
        /// </value>
        public bool UseDisplayContent { get; set; } = false;
        /// <summary>
        /// Gets or sets the type of the model.
        /// </summary>
        /// <value>
        /// The type of the model.
        /// </value>
        public LLM_ModelType ModelType {  get; set; }
        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public MessageDataType DataType { get; set; } = MessageDataType.PlainText;
        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public MessageContentType ContentType { get; set; } = MessageContentType.Text;
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
        public bool IsDeleted {  get; set; } = false;


    }
}
