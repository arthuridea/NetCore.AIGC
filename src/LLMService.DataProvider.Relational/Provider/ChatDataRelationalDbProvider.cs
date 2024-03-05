using LLMService.Shared.Models;
using LLMService.Shared.ServiceInterfaces;

namespace LLMService.DataProvider.Relational.Provider
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TChatMessage">The type of the chat message.</typeparam>
    /// <typeparam name="TMessageContent">The type of the message content.</typeparam>
    /// <seealso cref="IChatDataProvider{TChatMessage, TMessageContent}" />
    public class ChatDataRelationalDbProvider<TChatMessage, TMessageContent> : IChatDataProvider<TChatMessage, TMessageContent>
        where TChatMessage : IChatMessage<TMessageContent>, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatDataRelationalDbProvider{TChatMessage, TMessageContent}"/> class.
        /// </summary>
        public ChatDataRelationalDbProvider()
        {
            
        }

        /// <summary>
        /// Adds the chat message.
        /// not write to cache.
        /// </summary>
        /// <param name="conversation">The conversation.</param>
        /// <param name="message">The message.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<List<TChatMessage>> AddChatMessage(List<TChatMessage> conversation, TMessageContent message, string role = "user")
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the conversation.
        /// </summary>
        /// <param name="conversationId">The conversation identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<List<TChatMessage>> GetConversationHistory(string conversationId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resets the session.
        /// </summary>
        /// <param name="conversationId">The conversation identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool ResetSession(string conversationId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the chat.
        /// </summary>
        /// <param name="conversationId">The conversation identifier.</param>
        /// <param name="conversation"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task SaveChat(string conversationId, IEnumerable<TChatMessage> conversation)
        {
            throw new NotImplementedException();
        }
    }
}
