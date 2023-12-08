using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.Shared.ServiceInterfaces
{
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
