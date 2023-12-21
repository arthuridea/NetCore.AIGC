using LLMService.Shared.Models;
using LLMService.Shared.ServiceInterfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LLMService.Shared.DataProvider
{
    /// <summary>
    /// 用MemoryCache持久化会话内容
    /// <para>TODO:使用数据库结合分布式cache/nodb等做持久化</para>
    /// </summary>
    /// <typeparam name="TChatMessage">The type of the chat message.</typeparam>
    /// <typeparam name="TMessageContent"></typeparam>
    public class ChatDataProvider<TChatMessage, TMessageContent> : IChatDataProvider<TChatMessage, TMessageContent>
        where TChatMessage : IChatMessage<TMessageContent>, new()
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatDataProvider{TChatMessage, TMessageContent}"/> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="logger">The logger.</param>
        public ChatDataProvider(
            IMemoryCache cache,
            ILogger<ChatDataProvider<TChatMessage, TMessageContent>> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Gets the conversation.
        /// </summary>
        /// <param name="conversationId">The conversation identifier.</param>
        /// <returns></returns>
        public async Task<List<TChatMessage>> GetConversationHistory(string conversationId)
        {
            _cache.TryGetValue<List<TChatMessage>>(conversationId, out var conversation);
            if (conversation == null)
            {
                conversation = await GetConversationInternal(conversationId);
            }
            return conversation;
        }

        /// <summary>
        /// Adds the chat message.
        /// not write to cache.
        /// </summary>
        /// <param name="conversation">The conversation.</param>
        /// <param name="message">The message.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        public async Task<List<TChatMessage>> AddChatMessage(List<TChatMessage> conversation, TMessageContent message, string role = "user")
        {
            AddChatRound(conversation, message, role);
            return await Task.FromResult(conversation);
        }

        /// <summary>
        /// Saves the chat.
        /// </summary>
        /// <param name="conversationId">The conversation identifier.</param>
        /// <param name="conversation"></param>
        public async Task SaveChat(string conversationId, IEnumerable<TChatMessage> conversation)
        {
            _cache.Set(conversationId, conversation, TimeSpan.FromMinutes(30));

#if DEBUG
            _logger.LogDebug(@$"【save chat】{JsonSerializer.Serialize(conversation, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            })}");
#endif

            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the session.
        /// </summary>
        /// <param name="conversationId">The conversation identifier.</param>
        /// <returns></returns>
        public bool ResetSession(string conversationId)
        {
            try
            {
                _cache.Remove(conversationId);
                return true;
            }
            catch(Exception ex) {
                _logger.LogError(ex.ToString());
                return false;
            }
        }

        internal List<TChatMessage> AddChatRound(List<TChatMessage> conversation, TMessageContent message, string role = "user")
        {
            conversation.Add(new TChatMessage
            {
                Content = message,
                Role = role
            });
            return conversation;
        }

        internal async Task<List<TChatMessage>> GetConversationInternal(string conversationId)
        {
            return await Task.FromResult(new List<TChatMessage>());
        }

        internal async Task<int> SaveConversationInternal(IEnumerable<TChatMessage> round)
        {
            return await Task.FromResult(0);
        }
    }
}
