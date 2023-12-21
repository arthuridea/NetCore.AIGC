using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.Shared
{
    /// <summary>
    /// consts of LLM service
    /// </summary>
    public static class LLMServiceConsts
    {
        /***** 文心大模型 ****/

        #region 
        //public const string BaiduApiAuthority = "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop";
        /// <summary>
        /// The baidu API authority
        /// </summary>
        public const string BaiduWenxinApiAuthority = "https://aip.baidubce.com";
        /// <summary>
        /// The baidu wenxin API client name
        /// </summary>
        public const string BaiduWenxinApiClientName = "_Baidu_Wenxin_Workshop_Client";
        #endregion


        /****** OpenAI LLM ******/

        #region 
        /// <summary>
        /// The open ai API authority
        /// </summary>
        public const string OpenAIApiAuthority = "https://api.openai.com";

        /// <summary>
        /// The open ai API client name
        /// </summary>
        public const string OpenAIApiClientName = "_OpenAI_Client";
        #endregion


        /***** 智能绘画 ****/

        #region
        /// <summary>
        /// The baidu ernie vilg API authority
        /// </summary>
        public const string BaiduErnieVilgApiAuthority = "https://aip.baidubce.com";
        /// <summary>
        /// The baidu ernie vilg API client name
        /// </summary>
        public const string BaiduErnieVilgApiClientName = "_Baidu_ErnieVilg_Client";
        #endregion
    }
}
