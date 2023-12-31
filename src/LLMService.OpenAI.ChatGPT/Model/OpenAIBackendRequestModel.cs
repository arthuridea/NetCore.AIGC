﻿using LLMService.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LLMService.OpenAI.ChatGPT.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class OpenAIBackendRequestModel: IBackendChatRequest<OpenAIChatMessage>
    {
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        [JsonPropertyName("messages")]
        public List<OpenAIChatMessage> Messages { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OpenAIBackendRequestModel"/> is stream.
        /// </summary>
        /// <value>
        ///   <c>true</c> if stream; otherwise, <c>false</c>.
        /// </value>
        [JsonPropertyName("stream")]
        public bool Stream { get; set; } = false;

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
    }
}
