using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LLMService.Shared.ServiceInterfaces
{
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
