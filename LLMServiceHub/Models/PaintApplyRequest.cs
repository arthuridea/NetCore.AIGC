using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LLMServiceHub.Models
{
#nullable enable
    /// <summary>
    /// 绘画请求实体
    /// </summary>
    public class PaintApplyRequest
    {
        /// <summary>
        /// 会话编号
        /// </summary>
        /// <value>
        /// The conversation identifier.
        /// </value>
        /// <example></example>
        [JsonPropertyName("conversation_id")]
        public string ConversationId { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// 生图的文本描述。仅支持中文、日常标点符号。不支持英文，特殊符号，限制 200 字
        /// </summary>
        /// <value>
        /// The prompt.
        /// </value>
        /// <example></example>
        [JsonPropertyName("prompt")]
        [Required]
        [MaxLength(200)]
        public string Prompt { get; set; } = "";
        /// <summary>
        /// 图片大小预设系列
        /// </summary>
        /// <value>
        /// The size of the image.
        /// </value>
        /// <example>2</example>
        [JsonPropertyName("image_size")]
        //[EnumDataType(typeof(ErnieVilgImageSize))]
        //[JsonConverter(typeof(JsonStringEnumConverter))]
        public ErnieVilgImageSize ImageSize { get; set; } = ErnieVilgImageSize.SZ_360_640;
        /// <summary>
        /// 图片宽度，支持：512x512、640x360、360x640、1024x1024、1280x720、720x1280、2048x2048、2560x1440、1440x2560
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        /// <example>512</example>
        [JsonPropertyName("width")]
        public int Width => _getImageSize(ImageSize).width;
        /// <summary>
        /// 图片高度，支持：512x512、640x360、360x640、1024x1024、1280x720、720x1280、2048x2048、2560x1440、1440x2560
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        /// <example>512</example>
        [JsonPropertyName("height")]
        public int Height => _getImageSize(ImageSize).height;
        /// <summary>
        /// 生成图片数量，默认一张，支持生成 1-8 张
        /// </summary>
        /// <value>
        /// The image number.
        /// </value>
        /// <example>1</example>
        [JsonPropertyName("image_num")]
        [Range(1, 8)]
        public int ImageNumber { get; set; } = 1;
        /// <summary>
        /// 参考图，需 base64 编码，大小不超过 10M，最短边至少 15px，最长边最大 8192px，支持jpg/jpeg/png/bmp 格式。优先级：image &gt; url &gt; pdf_file，当image 字段存在时，url、pdf_file 字段失效
        /// </summary>
        /// <value>
        /// The image.
        /// </value>
        /// <example>null</example>
        [JsonPropertyName("image")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Image { get; set; } = null;
        /// <summary>
        /// 参考图完整 url，url 长度不超过 1024 字节，url 对应的图片需 base64 编码，大小不超过 10M，最短边至少 15px，最长边最大8192px，支持 jpg/jpeg/png/bmp 格式。优先级：image &gt; url &gt; pdf_file，当image 字段存在时，url 字段失效请注意关闭 URL 防盗链
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        /// <example>null</example>
        [JsonPropertyName("url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Url { get; set; } = null;
        /// <summary>
        /// 参考图 PDF 文件，base64 编码，大小不超过10M，最短边至少 15px，最长边最大 8192px 。优先级：image &gt; url &gt; pdf_file，当image 字段存在时，url、pdf_file 字段失效
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        /// <example>null</example>
        [JsonPropertyName("pdf_file")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PdfFile { get; set; } = null;
        /// <summary>
        /// 需要识别的 PDF 文件的对应页码，当pdf_file 参数有效时，识别传入页码的对应页面内容，若不传入，则默认识别第 1 页
        /// </summary>
        /// <value>
        /// The PDF file number.
        /// </value>
        /// <example>null</example>
        [JsonPropertyName("pdf_file_num")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PdfFileNumber { get; set; } = null;
        /// <summary>
        /// 参考图影响因子，支持 1-10 内；数值越大参考图影响越大
        /// </summary>
        /// <value>
        /// The change degree.
        /// </value>
        /// <example>1</example>
        [JsonPropertyName("change_degree")]
        public int ChangeDegree { get; set; } = 1;

        /// <summary>
        /// Gets the size of the image.
        /// </summary>
        /// <param name="preset">The preset.</param>
        /// <returns></returns>
        private static (int width, int height) _getImageSize(ErnieVilgImageSize preset)
        {
            return preset switch
            {
                ErnieVilgImageSize.SZ_512_512 => (width: 512, height: 512),
                ErnieVilgImageSize.SZ_640_360 => (width: 640, height: 360),
                ErnieVilgImageSize.SZ_360_640 => (width: 360, height: 640),
                ErnieVilgImageSize.SZ_1024_1024 => (width: 1024, height: 1024),
                ErnieVilgImageSize.SZ_1280_720 => (width: 1280, height: 720),
                ErnieVilgImageSize.SZ_720_1280 => (width: 720, height: 1280),
                ErnieVilgImageSize.SZ_2048_2048 => (width: 2048, height: 2048),
                ErnieVilgImageSize.SZ_2560_1440 => (width: 2560, height: 1440),
                ErnieVilgImageSize.SZ_1440_2560 => (width: 1440, height: 2560),
                _ => (width: 512, height: 512)
            };
        }
    }
#nullable disable

    /// <summary>
    /// 生成图片预设大小
    /// </summary>
    public enum ErnieVilgImageSize : int
    {
        /// <summary>
        /// The sz 512 512
        /// </summary>
        [Description("512*512")]
        SZ_512_512,
        /// <summary>
        /// The sz 640 360
        /// </summary>
        [Description("640*360")]
        SZ_640_360,
        /// <summary>
        /// The sz 360 640
        /// </summary>
        [Description("360*640")]
        SZ_360_640,
        /// <summary>
        /// The sz 1024 1024
        /// </summary>
        [Description("1024*1024")]
        SZ_1024_1024,
        /// <summary>
        /// The sz 1280 720
        /// </summary>
        [Description("1280*720")]
        SZ_1280_720,
        /// <summary>
        /// The sz 720 1280
        /// </summary>
        [Description("720*1280")]
        SZ_720_1280,
        /// <summary>
        /// The sz 2048 2048
        /// </summary>
        [Description("2048*2048")]
        SZ_2048_2048,
        /// <summary>
        /// The sz 2560 1440
        /// </summary>
        [Description("2560*1440")]
        SZ_2560_1440,
        /// <summary>
        /// The sz 1440 2560
        /// </summary>
        [Description("1440*2560")]
        SZ_1440_2560,
    }
}
