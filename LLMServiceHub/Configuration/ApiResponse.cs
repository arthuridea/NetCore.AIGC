namespace LLMServiceHub.Configuration
{

    /// <summary>
    /// api通用返回对象包装类
    /// </summary>
    /// <typeparam name="T">实际的实体类型</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Api版本(默认1.0)
        /// </summary>
        public string Version { get; set; } = "1.0";
        /// <summary>
        /// 返回值 0:正常 非0:异常
        /// </summary>
        public int ReturnCode { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message { get; set; } = "";
        /// <summary>
        /// 对象实体
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// 错误列表
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="message"></param>
        /// <param name="version"></param>
        public ApiResponse(int returnCode, string message = "", string version = "1.0")
        {
            ReturnCode = returnCode;
            Message = message;
            Version = version;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="err"></param>
        public void AddErrors(string err)
        {
            Errors.Add(err);
        }
    }
}
