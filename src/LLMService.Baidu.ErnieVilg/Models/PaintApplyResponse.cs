using System.Text.Json.Serialization;

namespace LLMService.Baidu.ErnieVilg.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class PaintApplyResponse
    {
        /// <summary>
        /// Gets or sets the log identifier.
        /// </summary>
        /// <value>
        /// The log identifier.
        /// </value>
        [JsonPropertyName("log_id")]
        public long LogId {  get; set; }
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        [JsonPropertyName("data")]
        public PaintApplyResultData Data { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PaintApplyResultData
    {
        /// <summary>
        /// Gets or sets the primary task identifier.
        /// </summary>
        /// <value>
        /// The primary task identifier.
        /// </value>
        [JsonPropertyName("primary_task_id")]
        public long PrimaryTaskId { get; set; }
        /// <summary>
        /// Gets or sets the task identifier.
        /// </summary>
        /// <value>
        /// The task identifier.
        /// </value>
        [JsonPropertyName("task_id")]
        public string TaskId {  get; set; }
    }
}
