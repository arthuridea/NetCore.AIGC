namespace LLMService.Shared.ServiceInterfaces
{
    /// <summary>
    /// AI Painting interface.
    /// <para>NOTE: Should ALWAYS initialize <seealso cref="IImageStorageProvider"/> first before DI.</para>
    /// </summary>
    /// <typeparam name="TPaintApplyRequest">The type of the paint apply request.</typeparam>
    /// <typeparam name="TPaintResultResponse">The type of the paint result response.</typeparam>
    public interface IAIPaintApiService<TPaintApplyRequest, TPaintResultResponse>
        where TPaintApplyRequest : new()
        where TPaintResultResponse : new()
    {
        /// <summary>
        /// Text2s the image.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        Task<TPaintResultResponse> Text2Image(TPaintApplyRequest request);
    }
}
