using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.Shared
{
    public static class LLMServiceConsts
    {
        /***** 文心大模型 ****/
        //public const string BaiduApiAuthority = "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop";
        /// <summary>
        /// The baidu API authority
        /// </summary>
        public const string BaiduWenxinApiAuthority = "https://aip.baidubce.com";
        /// <summary>
        /// The baidu wenxin API client name
        /// </summary>
        public const string BaiduWenxinApiClientName = "_Baidu_Wenxin_Workshop_Client";


        /***** 智能绘画 ****/

        /// <summary>
        /// The baidu ernie vilg API authority
        /// </summary>
        public const string BaiduErnieVilgApiAuthority = "https://aip.baidubce.com";
        /// <summary>
        /// The baidu ernie vilg API client name
        /// </summary>
        public const string BaiduErnieVilgApiClientName = "_Baidu_ErnieVilg_Client";
    }
}
