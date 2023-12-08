using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LLMService.Shared.Models
{
    /// <summary>
    /// 
    /// </summary>
    public static class BaiduApiDefaults
    {
        #region 文心大模型
        /// <summary>
        /// The LLM models
        /// </summary>
        public static Dictionary<LLM_ModelType, string> LLM_Models = new()
        {
            { LLM_ModelType.ERNIE_BOT , "/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/completions" },
            { LLM_ModelType.ERNIE_BOT_4 , "/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/completions_pro" },
            { LLM_ModelType.ERNIE_BOT_TURBO , "/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/eb-instant" },
        };


        /// <summary>
        /// 大模型版本
        /// </summary>
        public enum LLM_ModelType
        {
            /// <summary>
            /// The ernie bot
            /// </summary>
            [Description("文心大模型3.5(默认)")]
            ERNIE_BOT = 1,
            /// <summary>
            /// 文心大模型turbo版
            /// </summary>
            [Description("文心大模型turbo版")]
            ERNIE_BOT_TURBO = 2,
            /// <summary>
            /// The ernie bot 4
            /// </summary>
            [Description("文心大模型4.0")]
            ERNIE_BOT_4 = 3
        }
        #endregion

        #region 智能作画

        /// <summary>
        /// The ernievilg v2 API endpoint
        /// </summary>
        public const string ErnieVilgV2ApiEndpoint = "/rpc/2.0/ernievilg/v1/txt2imgv2";
        /// <summary>
        /// The ernie vilg v2 result API endpoint
        /// </summary>
        public const string ErnieVilgV2ResultApiEndpoint = "/rpc/2.0/ernievilg/v1/getImgv2";

        #endregion
    }
}
