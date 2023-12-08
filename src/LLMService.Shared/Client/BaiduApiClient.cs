
namespace LLMService.Shared.Client
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="HttpClient" />
    /// <seealso cref="ILLMServiceClient" />
    public class BaiduApiClient: HttpClient, ILLMServiceClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduApiClient"/> class.
        /// </summary>
        public BaiduApiClient()
        {
            BaseAddress = new Uri(LLMServiceConsts.BaiduWenxinApiAuthority + "/");
        }
    }
}
