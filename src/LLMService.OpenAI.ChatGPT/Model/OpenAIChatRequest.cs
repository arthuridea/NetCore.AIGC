using LLMService.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace LLMService.OpenAI.ChatGPT.Model
{
    /// <summary>
    /// OpenAI chat request model.
    /// </summary>
    /// <seealso cref="IChatRequest{T}" />
    public class OpenAIChatRequest : IChatRequest<string>
    {
        /// <summary>
        /// conversation id.
        /// <para>Will be cached by IChatDataProvider."/></para>
        /// </summary>
        /// <example></example>
        [JsonPropertyName("conversation_id")]
        public string ConversationId { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// user chat message for the last round.
        /// </summary>
        /// <example></example>
        [JsonPropertyName("message")]
        [Required]
        public string Message { get; set; }

        /// <summary>
        /// llm model enum.
        /// <para>available values：GPT_3_5_TURBO_1106(default)|GPT_3_5_TURBO|GPT_4|GPT_4_32K|GPT_4_TURBO</para>
        /// </summary>
        /// <example>5</example>
        [JsonPropertyName("model")]
        [Required]
        public LLMApiDefaults.LLM_ModelType ModelSchema { get; set; } = LLMApiDefaults.LLM_ModelType.GPT_3_5_TURBO_1106;

        /// <summary>
        /// What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic.
        /// <para>We generally recommend altering this or top_p but not both.</para>
        /// </summary>
        /// <value>
        /// default 1.0f
        /// </value>
        /// <example>1.0</example>
        [JsonPropertyName("temperature")]
        public float Temperature { get; set; } = 1.0F;
        /// <summary>
        /// An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered.
        /// <para>We generally recommend altering this or temperature but not both.</para>
        /// </summary>
        /// <value>
        /// default 1.0f
        /// </value>
        /// <example>1.0</example>
        [JsonPropertyName("top_p")]
        public float TopP { get; set; } = 1.0F;
        /// <summary>
        /// Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text so far, decreasing the model's likelihood to repeat the same line verbatim.
        /// <para>
        /// <see href="https://platform.openai.com/docs/guides/text-generation/parameter-details">See more information about frequency and presence penalties.</see>
        /// </para>
        /// </summary>
        /// <value>
        /// default 0.0f
        /// </value>
        /// <example>0.0</example>
        [JsonPropertyName("frequency_penalty")]
        public float FrequentPenalty { get; set; } = 0.0F;
        /// <summary>
        /// Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics.
        /// <para>
        /// <see href="https://platform.openai.com/docs/guides/text-generation/parameter-details">See more information about frequency and presence penalties.</see>
        /// </para>
        /// </summary>
        /// <value>
        /// default 0.0f
        /// </value>
        /// <example>0.0</example>
        [JsonPropertyName("presence_penalty")]
        public float PresencePenalty { get; set; } = 0.0F;
        /// <summary>
        /// If set, partial message deltas will be sent, like in ChatGPT. Tokens will be sent as data-only server-sent events as they become available, with the stream terminated by a data: [DONE] message. 
        /// </summary>
        /// <value>
        ///   <c>true</c> if stream; otherwise, <c>false</c>.
        /// </value>
        /// <example>false</example>
        [JsonPropertyName("stream")]
        public bool Stream { get; set; } = false;
    }
}
