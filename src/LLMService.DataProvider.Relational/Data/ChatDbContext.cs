using LLMService.DataProvider.Relational.Entity;
using LLMService.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static LLMService.DataProvider.Relational.Entity.EntityConsts;
using static LLMService.Shared.Models.LLMApiDefaults;

namespace LLMService.DataProvider.Relational.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class ChatDbContext : ChatDbContext<ChatFolder, Conversation, ChatMessage, string, string, Guid, Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatDbContext"/> class.
        /// </summary>
        /// <remarks>
        /// See <see href="https://aka.ms/efcore-docs-dbcontext">DbContext lifetime, configuration, and initialization</see>
        /// for more information.
        /// </remarks>
        public ChatDbContext()
        {            
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatDbContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        /// <remarks>
        /// See <see href="https://aka.ms/efcore-docs-dbcontext">DbContext lifetime, configuration, and initialization</see> and
        /// <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see> for more information.
        /// </remarks>
        public ChatDbContext(DbContextOptions<ChatDbContext> options)
            : base(options)
        {            
        }

        internal override void OnModelCreatingFinishing(ModelBuilder modelBuilder)
        {
            base.OnModelCreatingFinishing(modelBuilder);

            modelBuilder.Entity<ChatFolder>()
                        .Property(x => x.TenantId)
                        .HasMaxLength(128);
            modelBuilder.Entity<ChatFolder>()
                        .Property(x => x.UserId)
                        .HasMaxLength(128);
            modelBuilder.Entity<Conversation>()
                        .Property(x => x.TenantId)
                        .HasMaxLength(128);
            modelBuilder.Entity<Conversation>()
                        .Property(x => x.UserId)
                        .HasMaxLength(128);
            modelBuilder.Entity<ChatMessage>()
                        .Property(x => x.TenantId)
                        .HasMaxLength(128);
            modelBuilder.Entity<ChatMessage>()
                        .Property(x => x.UserId)
                        .HasMaxLength(128);

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TChatFolder">The type of the chat folder.</typeparam>
    /// <typeparam name="TConversation">The type of the conversation.</typeparam>
    /// <typeparam name="TMessage">The type of the chat message.</typeparam>
    /// <typeparam name="TTenantKey">The type of the tenant key.</typeparam>
    /// <typeparam name="TUserKey">The type of the user key.</typeparam>
    /// <typeparam name="TConversationKey">The type of the conversation key.</typeparam>
    /// <typeparam name="TMessageKey">The type of the message key.</typeparam>
    public class ChatDbContext<TChatFolder, TConversation, TMessage, TTenantKey, TUserKey, TConversationKey, TMessageKey>: DbContext
        where TUserKey : IEquatable<TUserKey>
        where TTenantKey : IEquatable<TTenantKey>
        where TMessageKey : IEquatable<TMessageKey>
        where TConversationKey : IEquatable<TConversationKey>
        where TChatFolder: ChatFolder<TUserKey, TTenantKey>
        where TConversation: Conversation<TConversationKey, TTenantKey, TUserKey>
        where TMessage: ChatMessage<TMessageKey, TConversationKey, TTenantKey, TUserKey>
    {
        #region Entities 
        /// <summary>
        /// Gets or sets the chat folders.
        /// </summary>
        /// <value>
        /// The chat folders.
        /// </value>
        public virtual DbSet<TChatFolder> ChatFolders { get; set; }
        /// <summary>
        /// Gets or sets the conversations.
        /// </summary>
        /// <value>
        /// The conversations.
        /// </value>
        public virtual DbSet<TConversation> Conversations { get; set; }
        /// <summary>
        /// Gets or sets the chat messages.
        /// </summary>
        /// <value>
        /// The chat messages.
        /// </value>
        public virtual DbSet<TMessage> ChatMessages { get; set; }
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatDbContext{TChatFolder, TConversation, TChatMessage, TTenantKey, TUserKey, TConversationKey, TMessageKey}"/> class.
        /// </summary>
        /// <remarks>
        /// See <see href="https://aka.ms/efcore-docs-dbcontext">DbContext lifetime, configuration, and initialization</see>
        /// for more information.
        /// </remarks>
        public ChatDbContext()
        {
            
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatDbContext{TChatFolder, TConversation, TChatMessage, TTenantKey, TUserKey, TConversationKey, TMessageKey}"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        /// <remarks>
        /// See <see href="https://aka.ms/efcore-docs-dbcontext">DbContext lifetime, configuration, and initialization</see> and
        /// <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see> for more information.
        /// </remarks>
        public ChatDbContext(DbContextOptions options)
            :base(options)
        {
            
        }
        #endregion

        #region FluentAPI config       
        
        /// <summary>
        /// Override this method to further configure the model that was discovered by convention from the entity types
        /// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
        /// and re-used for subsequent instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
        /// define extension methods on this object that allow you to configure aspects of the model that are specific
        /// to a given database.</param>
        /// <remarks>
        /// <para>
        /// If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
        /// then this method will not be run.
        /// </para>
        /// <para>
        /// See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see> for more information.
        /// </para>
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // overriding configurations
            modelBuilder.Entity<TChatFolder>(_configureChatFolder);
            modelBuilder.Entity<TConversation>(_configureConversation);
            modelBuilder.Entity<TMessage>(_configureChatMessage);

            modelBuilder.ConfigEntitiesThatImplementedFromIDomainEntity(this);

            OnModelCreatingFinishing(modelBuilder);
        }

        /// <summary>
        /// Called when [model creating internal].
        /// This method is called after all default model initialization.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        internal virtual void OnModelCreatingFinishing(ModelBuilder modelBuilder)
        {
            // override in derrived class.
        }

        /// <summary>
        /// Configures the chat folder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void _configureChatFolder(EntityTypeBuilder<TChatFolder> builder)
        {
            builder.ToTable("ChatFolder");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .HasMaxLength(512);

            builder.HasMany<TChatFolder>()
                   .WithOne()
                   .HasForeignKey(x => x.ParentId)
                   .IsRequired(false);

        }
        /// <summary>
        /// Configures the conversation.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void _configureConversation(EntityTypeBuilder<TConversation> builder)
        {
            builder.ToTable("Conversation");
            builder.HasKey(x => x.Id);

            // one to many
            builder.HasMany<TMessage>()
                   .WithOne()
                   .HasForeignKey(x => x.ConversationId)
                   .IsRequired(false);

        }
        /// <summary>
        /// Configures the chat message.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void _configureChatMessage(EntityTypeBuilder<TMessage> builder)
        {
            builder.ToTable("ChatMessage");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ConversationId)
                   .IsRequired(true);

            builder.Property(x => x.Role)
                   .HasMaxLength(256);

            // model type comments.

            builder.Property(x => x.ModelType)
                   .HasComment(_getCommentFromEnum<LLM_ModelType>());

            // MessageDataType comments.
            builder.Property(x => x.DataType)
                   .HasComment(_getCommentFromEnum<MessageDataType>());

            // MessageContentType comments.
            builder.Property(x => x.ContentType)
                   .HasComment(_getCommentFromEnum<MessageContentType>());
        }

        private static string _getCommentFromEnum<TEnum>()
            where TEnum : Enum
        {
            StringBuilder comments = new();
            var lists = Enum.GetValues(typeof(TEnum))
                            .Cast<TEnum>()
                            .Select(x => new
                            {
                                Value = (int)(object)x,
                                Name = x.ToString(),
                                DisplayName = x.GetAttribute<DisplayAttribute>()?.Name ?? "",
                                Description = x.GetAttribute<DescriptionAttribute>()?.Description ?? "",
                            });
            foreach (var item in lists)
            {
                comments.Append($"{item.Name} = {item.Value}{(string.IsNullOrEmpty(item.Description) ? "" : $"({item.Description})")} | ");
            }
            return comments.ToString();
        }

        #endregion
    }
}
