using Microsoft.AspNetCore.Mvc;
using LLMServiceHub.Models;

namespace LLMServiceHub.Service
{
    /// <summary>
    /// 百度文心api服务
    /// </summary>
    public interface IBaiduWenxinApiService
    {
        /// <summary>
        /// 发起对话
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <seealso cref="ChatRequest" />
        Task Chat(ChatRequest request, CancellationToken cancellationToken = default);
    }
}
