using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.Shared.Client
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Net.Http.HttpClient" />
    /// <seealso cref="LLMService.Shared.Client.ILLMServiceClient" />
    public class OpenAIApiClient : HttpClient, ILLMServiceClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAIApiClient"/> class.
        /// </summary>
        public OpenAIApiClient()
        {
            BaseAddress = new Uri(LLMServiceConsts.OpenAIApiAuthority + "/");
        }
    }
}
