using System.Text.Json.Serialization;
using static LLMService.Shared.Models.LLMApiDefaults;

namespace LLMService.Shared.Models
{
    /// <summary>
    /// baidu API response model
    /// </summary>
    public class BaiduWenxinChatResponse /*: IServerSentEventData*/
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonPropertyName("id")]
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the type of the object.
        /// </summary>
        /// <value>
        /// The type of the object.
        /// </value>
        [JsonPropertyName("object")]
        public string ObjectType { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        [JsonPropertyName("created")]
        public long Created { get; set; }
        /// <summary>
        /// Gets or sets the sentence identifier.
        /// </summary>
        /// <value>
        /// The sentence identifier.
        /// </value>
        [JsonPropertyName("sentence_id")]
        public int SentenceId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is end.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is end; otherwise, <c>false</c>.
        /// </value>
        [JsonPropertyName("is_end")]
        public bool IsEnd { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is truncated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is truncated; otherwise, <c>false</c>.
        /// </value>
        [JsonPropertyName("is_truncated")]
        public bool IsTruncated { get; set; }
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        [JsonPropertyName("result")]
        public string Result { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [need clear history].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [need clear history]; otherwise, <c>false</c>.
        /// </value>
        [JsonPropertyName("need_clear_history")]
        public bool NeedClearHistory { get; set; }
        /// <summary>
        /// Gets or sets the ban round.
        /// </summary>
        /// <value>
        /// The ban round.
        /// </value>
        [JsonPropertyName("ban_round")]
        public int BanRound { get; set; }
        /// <summary>
        /// Gets or sets the usage.
        /// </summary>
        /// <value>
        /// The usage.
        /// </value>
        [JsonPropertyName("usage")]
        public ChatApiTokenUsage Usage { get; set; }
    }

    /// <summary>
    /// baidu API response wrapper
    /// </summary>
    /// <seealso cref="BaiduWenxinChatResponse" />
    public class ChatApiResponse: IChatResponse<BaiduWenxinChatResponse>
    {
        /// <summary>
        /// Gets or sets the conversation identifier.
        /// </summary>
        /// <value>
        /// The conversation identifier.
        /// </value>
        [JsonPropertyName("conversation_id")]
        public string ConversationId { get; set; } = "";
        /// <summary>
        /// 大模型
        /// <para>可选项：ERNIE-Bot-turbo(default)|ERNIE-Bot-4|ERNIE-Bot</para>
        /// </summary>
        /// <example>2</example>
        [JsonPropertyName("model")]
        public LLM_ModelType ModelSchema { get; set; } = LLM_ModelType.ERNIE_BOT_TURBO;
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        [JsonPropertyName("result")]
        public BaiduWenxinChatResponse Result { get; set; }
        /// <summary>
        /// Gets a value indicating whether [need clear history].
        /// </summary>
        /// <value>
        /// <c>true</c> if [need clear history]; otherwise, <c>false</c>.
        /// </value>
        [JsonPropertyName("need_clear_history")]
        public bool NeedClearHistory => Result?.NeedClearHistory ?? false;
    }


    ///// <summary>
    ///// baidu API token usage per request.
    ///// </summary>
    //public class BaiduWenxinUsage
    //{
    //    /// <summary>
    //    /// Gets or sets the prompt tokens.
    //    /// </summary>
    //    /// <value>
    //    /// The prompt tokens.
    //    /// </value>
    //    [JsonPropertyName("prompt_tokens")]
    //    public int PromptTokens { get; set; }
    //    /// <summary>
    //    /// Gets or sets the completion tokens.
    //    /// </summary>
    //    /// <value>
    //    /// The completion tokens.
    //    /// </value>
    //    [JsonPropertyName("completion_tokens")]
    //    public int CompletionTokens { get; set; }
    //    /// <summary>
    //    /// Gets or sets the total tokens.
    //    /// </summary>
    //    /// <value>
    //    /// The total tokens.
    //    /// </value>
    //    [JsonPropertyName("total_tokens")]
    //    public int TotalTokens {  get; set; }
    //}

    /// <summary>
    /// 
    /// </summary>
    public class ResponseStream<T>
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        [JsonPropertyName("data")]
        public T Data { get; set; }

        /// <summary>
        /// Unfolds this instance.
        /// </summary>
        /// <returns></returns>
        public T Unfold()
        {
            return Data;
        }
    }
}
