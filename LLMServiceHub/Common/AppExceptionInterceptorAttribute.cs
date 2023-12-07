using LLMServiceHub.Configuration;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;

namespace LLMServiceHub.Common
{
    /// <summary>
    /// 异常拦截器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AppExceptionInterceptorAttribute : ExceptionFilterAttribute, IAsyncExceptionFilter
    {
        private IHostEnvironment _hostEnvironment;
        private IModelMetadataProvider _modelMetadataProvider;
        private ILogger _logger;

        /// <summary>
        /// set json return code.default is exception.HRESULT
        /// </summary>
        public int ReturnCode { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessageTemplate { get; set; } = "发生错误:{0}";
        /// <summary>
        /// 
        /// </summary>
        public bool FlushStacktrace { get; set; } = false;
        /// <summary>
        /// 触发异常的Api版本，会被自定义的AppException中的api版本定义覆盖
        /// </summary>
        public string ApiVersion { get; set; } = "1.0";
        /// <summary>
        /// 输出类型 默认为JSON
        /// </summary>
        public AppExceptionResponseType ResponseType { get; set; } = AppExceptionResponseType.Json;
        /// <summary>
        /// 
        /// </summary>
        public string View { get; set; } = "Error";
        /// <summary>
        /// 
        /// </summary>
        public string ViewModelDataKey { get; set; } = "ex-view-model";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnExceptionAsync(ExceptionContext context)
        {
            var services = context.HttpContext.RequestServices;
            _hostEnvironment = (IHostEnvironment)services.GetService(typeof(IHostEnvironment));
            _modelMetadataProvider = (IModelMetadataProvider)services.GetService(typeof(IModelMetadataProvider));
            _logger = (ILogger<AppExceptionInterceptorAttribute>)context.HttpContext.RequestServices.GetService(typeof(ILogger<AppExceptionInterceptorAttribute>));

            _logger?.LogError($"[AppExceptionInterceptor::OnExceptionAsync]{context?.Exception?.Message}");

            flushResponse(context);

            return Task.CompletedTask;
        }

        private void flushResponse(ExceptionContext context)
        {
            if (ResponseType == AppExceptionResponseType.Json)
            {
                //Business exception-More generics for external world
                int retCode = ReturnCode == 0 ? context.Exception.HResult : ReturnCode;
                string errMsg = context.Exception.Message;
                List<string> errdetails = new List<string>();
                var ret = new ApiResponse<RouteData>(retCode, errMsg);

                ret.Version = ApiVersion;

                if (context.Exception is AppException)
                {
                    var ex = context.Exception as AppException;
                    ret = ex.GenerteApiResponse(context.RouteData);
                }

                if (FlushStacktrace || _hostEnvironment.IsDevelopment())
                {
                    ret.AddErrors(context.Exception.StackTrace);
                }
                //Logs your technical exception with stack trace below

                context.HttpContext.Response.ContentType = "application/json";
                context.Result = new OkObjectResult(ret);
            }
            else if (ResponseType == AppExceptionResponseType.Html)
            {
                context.HttpContext.Response.ContentType = "text/html";
                var result = new ViewResult { ViewName = View };
                result.ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState);
                result.ViewData.Add("Exception", context.Exception);
                // TODO: Pass additional detailed data via ViewData
                result.ViewData.Model = context.HttpContext.Items[ViewModelDataKey];
                context.Result = result;
            }
            else
            {
                context.HttpContext.Response.ContentType = "text/plain";
                var result = new ContentResult();
                result.Content = context.Exception.Message;
                context.Result = result;
            }
        }

    }
    /// <summary>
    /// 
    /// </summary>
    public enum AppExceptionResponseType
    {
        /// <summary>
        /// 
        /// </summary>
        Json,
        /// <summary>
        /// 
        /// </summary>
        Plain,
        /// <summary>
        /// 
        /// </summary>
        Html
    }
}
