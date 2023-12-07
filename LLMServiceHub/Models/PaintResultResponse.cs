using System.ComponentModel;
using System.Text.Json.Serialization;

namespace LLMServiceHub.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class PaintResultResponse
    {
        /// <summary>
        /// Gets or sets the log identifier.
        /// </summary>
        /// <value>
        /// The log identifier.
        /// </value>
        [JsonPropertyName("log_id")]
        public long LogId { get; set; }
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        [JsonPropertyName("data")]
        public PaintResultData Data { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PaintResultData
    {
        /// <summary>
        /// 任务 Id
        /// </summary>
        /// <value>
        /// The task identifier.
        /// </value>
        [JsonPropertyName("task_id")]
        public long TaskId {  get; set; }
        /// <summary>
        /// 计算总状态.有 INIT（初始化），WAIT（排队中）, RUNNING（生成中）, FAILED（失败）, SUCCESS（成功）四种状态，只有 SUCCESS 为成功状态
        /// </summary>
        /// <value>
        /// The task status.
        /// </value>
        /// <example>INIT</example>
        [JsonPropertyName("task_status")]
        public string StatusText {  get; set; }
        /// <summary>
        /// 计算总状态码.有 INIT（初始化），WAIT（排队中）, RUNNING（生成中）, FAILED（失败）, SUCCESS（成功）四种状态，只有 SUCCESS 为成功状态
        /// </summary>
        /// <value>
        /// The task status.
        /// </value>
        /// <example>INIT</example>
        [JsonPropertyName("task_status_code")]
        public PaintResultTaskStatus Status => (PaintResultTaskStatus)Enum.Parse(typeof(PaintResultTaskStatus), StatusText);
        /// <summary>
        /// 图片生成总进度.进度包含2种，0为未处理完，1为处理完成
        /// </summary>
        /// <value>
        /// The task process status.
        /// </value>
        /// <example>0</example>
        [JsonPropertyName("task_progress")]
        public PaintTaskProcessStatus ProcessStatus { get; set; } = PaintTaskProcessStatus.OnProgress;
        /// <summary>
        /// 子任务生成结果列表
        /// </summary>
        /// <value>
        /// The sub task results.
        /// </value>
        [JsonPropertyName("sub_task_result_list")]
        public List<PaintSubTaskResult> SubTaskResults { get; set; } = new();

    }
    /// <summary>
    /// 子任务结果
    /// </summary>
    public class PaintSubTaskResult
    {
        /// <summary>
        /// 单风格图片状态。有 INIT（初始化），WAIT（排队中）, RUNNING（生成中）, FAILED（失败）, SUCCESS（成功）四种状态，只有 SUCCESS 为成功状态
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [JsonPropertyName("sub_task_status")]
        public string StatusText { get; set; }
        /// <summary>
        /// 单风格图片状态码
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [JsonPropertyName("sub_task_status_code")]
        public PaintResultTaskStatus Status => (PaintResultTaskStatus)Enum.Parse(typeof(PaintResultTaskStatus), StatusText);
        /// <summary>
        /// 单任务图片生成进度，进度包含2种，0为未处理完，1为处理完成
        /// </summary>
        /// <value>
        /// The process status.
        /// </value>
        [JsonPropertyName("sub_task_progress")]
        public PaintTaskProcessStatus ProcessStatus { get; set; }= PaintTaskProcessStatus.OnProgress;
        /// <summary>
        /// 单风格任务错误码。0:正常；501:文本黄反拦截；201:模型生图失败
        /// </summary>
        /// <value>
        /// The sub task error code.
        /// </value>
        [JsonPropertyName("sub_task_error_code")]
        public long SubTaskErrorCode {  get; set; }
        /// <summary>
        /// 单风格任务错误码信息。0:正常；501:文本黄反拦截；201:模型生图失败  由SubTaskErrorCode生成
        /// </summary>
        /// <value>
        /// The sub task error information.
        /// </value>
        [JsonPropertyName("sub_task_error_info")]
        public PaintTaskErrorCode SubTaskErrorInfo => (PaintTaskErrorCode)SubTaskErrorCode;
        /// <summary>
        /// Gets or sets the images.
        /// </summary>
        /// <value>
        /// The images.
        /// </value>
        [JsonPropertyName("final_image_list")]
        public List<ImageResultItem> Images { get; set; } = new();
    }
    /// <summary>
    /// 单张图片结果
    /// </summary>
    public class ImageResultItem
    {
        /// <summary>
        /// 图片像素信息-宽度
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        [JsonPropertyName("width")]
        public int Width {  get; set; }
        /// <summary>
        /// 图片像素信息-高度
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        [JsonPropertyName("height")]
        public int Height { get; set; }
        /// <summary>
        /// 图片所在 BOS http 地址，默认 1 小时失效
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        [JsonPropertyName("img_url")]
        public string Url {  get; set; }
        /// <summary>
        /// 图片机审结果，"block"：输出图片违规；"review": 输出图片疑似违规；"pass": 输出图片未发现问题；
        /// </summary>
        /// <value>
        /// The approve result.
        /// </value>
        [JsonPropertyName("img_approve_conclusion")]
        public string ApproveResult {  get; set; }
        /// <summary>
        /// 是否通过审核（根据ApproveResult生成）
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is approved; otherwise, <c>false</c>.
        /// </value>
        [JsonPropertyName("is_approved")]
        public bool IsApproved => ApproveResult == "pass";
    }


    /// <summary>
    /// 
    /// </summary>
    public enum PaintResultTaskStatus
    {
        /// <summary>
        /// 图片任务计算总状态
        /// </summary>
        [Description("初始化")]
        INIT = 0,
        /// <summary>
        /// 排队中
        /// </summary>
        [Description("排队中")]
        WAIT = 1,
        /// <summary>
        /// 生成中
        /// </summary>
        [Description("生成中")]
        RUNNING = 2,
        /// <summary>
        /// 失败
        /// </summary>
        [Description("失败")]
        FAILED = 3,
        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        SUCCESS = 4,
    }
    /// <summary>
    /// 图片生成总进度状态
    /// </summary>
    public enum PaintTaskProcessStatus : int
    {
        /// <summary>
        /// 处理中
        /// </summary>
        OnProgress = 0,
        /// <summary>
        /// 已完成
        /// </summary>
        Finished = 1
    }

    /// <summary>
    /// 任务错误码
    /// </summary>
    public enum PaintTaskErrorCode : long
    {
        /// <summary>
        /// 成功
        /// </summary>
        SUCCESS = 0,
        /// <summary>
        /// 服务器内部错误，请再次请求， 如果持续出现此类错误，请在控制台提交工单联系技术支持团队
        /// </summary>
        INTERNAL_ERROR = 282000,
        /// <summary>
        /// 请求中包含敏感词、非法参数、字数超限，或上传违规参考图，请检查后重新尝试
        /// </summary>
        CENSORSHIP = 282004,
        /// <summary>
        /// 缺少必要参数
        /// </summary>
        PARAMETER_MISSING = 282003,
        /// <summary>
        /// 日配额流量超限
        /// </summary>
        USAGE_EXCEEDED = 17,
        /// <summary>
        /// QPS 超限额
        /// </summary>
        OPEN_API_QPS_EXCEEDED = 18,
        /// <summary>
        /// 服务器内部错误，请再次请求，如果持续出现此类错误，请通过工单联系技术支持
        /// </summary>
        RECOGNIZE_ERROR = 282003,
        /// <summary>
        /// 文本黄反拦截
        /// </summary>
        VIOLATION = 501,
        /// <summary>
        /// 模型生图失败
        /// </summary>
        GENERATION_ERROR = 201,
        /// <summary>
        /// 参数不满足格式要求
        /// </summary>
        INVALID_PARAMETER = 216100,
        /// <summary>
        /// 参考图不满足格式要求
        /// </summary>
        REF_IMAGE_FORMAT_ERROR = 216201,

        //USER

    }
}
