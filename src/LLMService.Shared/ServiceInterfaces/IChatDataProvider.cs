
using LLMService.Shared.Models;

namespace LLMService.Shared.ServiceInterfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TChatMessage">The type of the chat message.</typeparam>
    /// <typeparam name="TMessageContent"></typeparam>
    public interface IChatDataProvider<TChatMessage, TMessageContent>
        where TChatMessage : IChatMessage<TMessageContent>, new()
    {
        /// <summary>
        /// Gets the conversation.
        /// </summary>
        /// <param name="conversationId">The conversation identifier.</param>
        /// <returns></returns>
        Task<List<TChatMessage>> GetConversationHistory(string conversationId);

        /// <summary>
        /// Adds the chat message.
        /// not write to cache.
        /// </summary>
        /// <param name="conversation">The conversation.</param>
        /// <param name="message">The message.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        Task<List<TChatMessage>> AddChatMessage(List<TChatMessage> conversation, TMessageContent message, string role = "user");

        /// <summary>
        /// Saves the chat.
        /// </summary>
        /// <param name="conversationId">The conversation identifier.</param>
        /// <param name="conversation"></param>
        Task SaveChat(string conversationId, IEnumerable<TChatMessage> conversation);

        /// <summary>
        /// Resets the session.
        /// </summary>
        /// <param name="conversationId">The conversation identifier.</param>
        /// <returns></returns>
        bool ResetSession(string conversationId);
    }
}
