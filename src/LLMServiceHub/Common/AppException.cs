using LLMServiceHub.Configuration;
using System.Globalization;

namespace LLMServiceHub.Common
{
    /// <summary>
    /// custom exception class for throwing application specific exceptions 
    /// that can be caught and handled within the application
    /// </summary>
    [Serializable]
    public class AppException : Exception
    {
        /// <summary>
        /// Api版本(默认1.0)
        /// </summary>
        public string ApiVersion { get; set; } = "1.0";
        /// <summary>
        /// 
        /// </summary>
        public int ReturnCode { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public List<string> ErrorDetails { get; set; } = new List<string>();
        /// <summary>
        /// 
        /// </summary>
        public AppException() : base() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="returnCode"></param>
        /// <param name="apiVer"></param>
        /// <param name="args"></param>
        public AppException(string message, int returnCode = 0, string apiVer = "1.0", params object[] args) : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
            if (returnCode != 0)
            {
                ReturnCode = returnCode;
            }
            ApiVersion = apiVer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="err"></param>
        public void AddError(string err)
        {
            ErrorDetails.Add(err);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="errs"></param>
        public void AddErrors(params string[] errs)
        {
            ErrorDetails.AddRange(errs);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ApiResponse<T> GenerteApiResponse<T>(T data = default(T))
        {
            var ret = new ApiResponse<T>(ReturnCode, Message);
            ret.Data = data;
            ret.Errors = ErrorDetails;
            ret.Version = ApiVersion;

            return ret;
        }
    }
}
