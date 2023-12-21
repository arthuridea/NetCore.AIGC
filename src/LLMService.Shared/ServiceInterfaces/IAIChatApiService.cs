namespace LLMService.Shared.ServiceInterfaces
{
    /// <summary>
    /// AI Chat interface
    /// <para>NOTE: Should ALWAYS initialize <seealso cref="IChatDataProvider{TChatMessage, TChatResponse}"/> first before DI.</para>
    /// </summary>
    /// <typeparam name="TChatRequest">The type of the chat request.</typeparam>
    /// <typeparam name="TChatResponse">The type of the chat response.</typeparam>
    public interface IAIChatApiService<TChatRequest, TChatResponse>
    {
        /// <summary>
        /// Chats the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task Chat(TChatRequest request, CancellationToken cancellationToken = default);
    }
}
