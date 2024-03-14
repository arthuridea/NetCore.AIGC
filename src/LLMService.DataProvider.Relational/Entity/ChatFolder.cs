namespace LLMService.DataProvider.Relational.Entity
{
    /// <summary>
    /// Folders in percific 
    /// </summary>
    public class ChatFolder : ChatFolder<string, string>
    {
        /// <summary>
        /// [derrived] Gets or sets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        public override string TenantId { get; set; }
    }

    /// <summary>
    /// Chat folders.
    /// </summary>
    /// <typeparam name="TUserKey">The type of the user key.</typeparam>
    /// <typeparam name="TTenantKey">The type of the tenant key.</typeparam>
    public class ChatFolder<TUserKey, TTenantKey> : ICreatedUtcTime
        where TUserKey : IEquatable<TUserKey>
        where TTenantKey : IEquatable<TTenantKey>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        /// <value>
        /// The parent identifier.
        /// </value>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public virtual TUserKey UserId { get; set; }
        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        public virtual TTenantKey TenantId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
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
