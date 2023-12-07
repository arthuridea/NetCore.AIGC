using LLMServiceHub.Authentication;

namespace LLMServiceHub.Client
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Net.Http.HttpClient" />
    /// <seealso cref="LLMServiceHub.Client.IBaiduApiClient" />
    public class BaiduApiClient: HttpClient, IBaiduApiClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduApiClient"/> class.
        /// </summary>
        public BaiduApiClient()
        {
            BaseAddress = new Uri(BaiduApiConsts.BaiduWenxinApiAuthority + "/");
        }
    }
}
