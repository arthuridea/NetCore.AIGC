using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LLMService.Shared.Models
{
    /// <summary>
    /// 
    /// </summary>
    public static class LLMApiDefaults
    {
        #region 文心大模型 and ChatGPT
        /// <summary>
        /// The LLM models
        /// </summary>
        public static Dictionary<LLM_ModelType, string> LLM_Models = new()
        {
            { LLM_ModelType.ERNIE_BOT , "/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/completions" },
            { LLM_ModelType.ERNIE_BOT_4 , "/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/completions_pro" },
            { LLM_ModelType.ERNIE_BOT_TURBO , "/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/eb-instant" },
            { LLM_ModelType.GPT_3_5_TURBO , "/v1/chat/completions" },
            { LLM_ModelType.GPT_3_5_TURBO_1106 , "/v1/chat/completions" },
            { LLM_ModelType.GPT_4 , "/v1/chat/completions" },
            { LLM_ModelType.GPT_4_32K , "/v1/chat/completions" },
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
            [Display(Name ="ernie-bot")]
            ERNIE_BOT = 1,
            /// <summary>
            /// 文心大模型turbo版
            /// </summary>
            [Description("文心大模型turbo版")]
            [Display(Name = "ernie-bot-turbo")]
            ERNIE_BOT_TURBO = 2,
            /// <summary>
            /// The ernie bot 4
            /// </summary>
            [Description("文心大模型4.0")]
            [Display(Name = "ernie-bot-4.0")]
            ERNIE_BOT_4 = 3,
            /// <summary>
            /// gpt-3.5-turbo
            /// </summary>
            [Description("gpt-3.5-turbo")]
            [Display(Name = "gpt-3.5-turbo")]
            GPT_3_5_TURBO = 4,
            /// <summary>
            /// gpt-3.5-turbo-1106
            /// </summary>
            [Description("gpt-3.5-turbo-1106")]
            [Display(Name = "gpt-3.5-turbo-1106")]
            GPT_3_5_TURBO_1106 = 5,
            /// <summary>
            /// gpt-4
            /// </summary>
            [Description("gpt-4")]
            [Display(Name = "gpt-4")]
            GPT_4 = 6,
            /// <summary>
            /// gpt-4-32k
            /// </summary>
            [Description("gpt-4-32k")]
            [Display(Name = "gpt-4-32k")]
            GPT_4_32K = 7,
            /// <summary>
            /// gpt-4-1106-preview(turbo)
            /// </summary>
            [Description("gpt-4-1106-preview")]
            [Display(Name = "gpt-4-1106-preview")]
            GPT_4_TURBO = 8,


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
