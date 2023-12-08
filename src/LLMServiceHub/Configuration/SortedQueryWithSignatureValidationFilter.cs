using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;

namespace LLMServiceHub.Configuration
{

    /// <summary>
    /// return type when authorize fails 
    /// <para>UnAuthorizedResult|BadRequestJsonResult|PlainText|StatusCode|ViewResult</para>
    /// </summary>
    public enum AppAuthorizeFailureResultType
    {
        /// <summary>
        /// an unauthorized action result with httpStatus 401
        /// this may cause login redirection by default
        /// </summary>
        UnAuthorizedResult,
        /// <summary>
        /// an badrequest object action result with httpStatus 400
        /// a Json returned
        /// </summary>
        BadRequestJsonResult,
        /// <summary>
        /// content
        /// </summary>
        PlainText,
        /// <summary>
        /// returns a httpStatusCode
        /// </summary>
        StatusCode,
        /// <summary>
        /// returns a custom view
        /// </summary>
        ViewResult,
    }

    /// <summary>
    /// api signature based authorization filter.
    /// <para>use app,ts,nonce,sig parameters building controller or action level authorization.basically,the signature algorithms is:</para>
    /// <para>1.sort query parameters in alphabet sequence</para>
    /// <para>2.build query string for parameters: ?key1=encode({value1})&amp;key2=encode({value2})....</para>
    /// <para>3.contact api endpoint with parameters generated in step2 as sigbase string</para>
    /// <para>4.calulate HMACSHA256 hash using {sigbase} with appsecret</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class SortedQueryWithSignatureValidationFilter : Attribute, IAsyncActionFilter
    {
        private IHostEnvironment _env;
        private IModelMetadataProvider _modelMetadataProvider;
        private ILogger _logger;
        private const string APP_KEY = "app";
        //private const string SIG_KEY = "sig";
        private const string TS_KEY = "ts";
        private const string NONCE_KEY = "nonce";

        private byte[] signingKeyBytes;
        /// <summary>
        /// app key parameter name
        /// </summary>
        public string AppKeyParameterName { get; set; } = APP_KEY;
        /// <summary>
        /// timestamp parameter name
        /// </summary>
        public string TimestampParameterName { get; set; } = TS_KEY;
        /// <summary>
        /// rand string parameter name
        /// </summary>
        public string NonceParameterName { get; set; } = NONCE_KEY;

        /// <summary>
        /// signature stores location(QUERY|HEADER)
        /// </summary>
        public SignatureParameterType SignatureStorage { get; set; } = SignatureParameterType.QUERY;
        /// <summary>
        /// signature parameter key.default value:sig
        /// </summary>
        public string SignatureParameterKey { get; set; } = "sig";
        /// <summary>
        /// Gets or sets the exclude parameters for signature.
        /// split by '|' 
        /// </summary>
        /// <value>
        /// The exclude parameters for signature.
        /// </value>
        public string ExcludeParametersForSignature { get; set; } = "";
        /// <summary>
        /// signature expires in (seconds).default value:3600
        /// </summary>
        public long SignatureExpiresIn { get; set; } = 3600;
        /// <summary>
        /// require timestamp parameter verified
        /// <para>default: true</para>
        /// </summary>
        public bool RequireTimestampValidation { get; set; } = true;

        /// <summary>
        /// action result type when validation failures(UnAuthorizedResult|BadRequestJsonResult|PlainText).default:BadRequestJsonResult
        /// </summary>
        public AppAuthorizeFailureResultType AuthorizeFailureResultType { get; set; } = AppAuthorizeFailureResultType.BadRequestJsonResult;

        /// <summary>
        /// validation fails page or viewname (AuthorizeFailureResultType is UnauthorizedResult or ViewResult).
        /// <para>default value: ~/Account/Login</para>
        /// </summary>
        public string AuthorizeFailureResultPage { get; set; } = "~/Account/Login";


        /// <summary>
        /// signature validation filter:
        /// 1.sort query parameters in alphabets
        /// 2.contact parameters as sigbase string: ?key1=encode(value1)&amp;key2=encode(value2)....
        /// 3.calulate HMACSHA256 hash with appsecret
        /// </summary>
        public SortedQueryWithSignatureValidationFilter()
        {
            //signingKeyBytes = Encoding.UTF8.GetBytes(SigSignKey);            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var sp = context.HttpContext.RequestServices;
            _env = (IHostEnvironment)sp.GetService(typeof(IHostEnvironment));
            _modelMetadataProvider = (IModelMetadataProvider)sp.GetService(typeof(IModelMetadataProvider));
            _logger = (ILogger<SortedQueryWithSignatureValidationFilter>)sp.GetService(typeof(ILogger<SortedQueryWithSignatureValidationFilter>));

            // verify parameters
            var contextVerifyResult = isContextValid(context);
            if (!contextVerifyResult.IsValid)
            {
                return;
            }
            // validate appkey
            string appKey = extractParameter(APP_KEY, context);

            // get signkey use appsecret to generate sign key.
            string hash = GetAppSecret(appKey);
            signingKeyBytes = Encoding.UTF8.GetBytes(hash);
            // sort query 
            var signatureBaseString = getSignatureBaseString(context);
            // validate signature
            var signatureValidationResult = isSignatureValid(signatureBaseString, context);
            if (!signatureValidationResult.IsValid)
            {
                var r = new ApiSignatureValidationResult();
                //r.AddError(signatureValidationResult.Message);
                flushResult(-401000, context, r, signatureValidationResult.Message);
                return;
            }
            if (_env.IsDevelopment())
            {
                _logger?.LogInformation("[SortedQueryWithSignatureValidationFilter::OnActionExecutionAsync] Action Request Verified.Go next()");
            }
            await next().ConfigureAwait(false);
            return;
        }

        private (bool IsValid, ApiSignatureValidationResult Result, IActionResult ActionResult) isContextValid(ActionExecutingContext context)
        {
            var parameterRequired = new string[] { APP_KEY, TS_KEY, NONCE_KEY };
            var result = new ApiSignatureValidationResult();

            var query = context.HttpContext.Request.Query;

            #region check required parameters
            foreach (var p in parameterRequired)
            {
                if (!RequireTimestampValidation && p == TS_KEY)
                {
                    continue;
                }
                if (!query.ContainsKey(p))
                {
                    //result.AddError($"缺少必要的参数:{p}");
                    flushResult(-401001, context, result, $"缺少必要的参数:{p}");
                    return (IsValid: false, Result: result, ActionResult: context.Result);
                }
            }
            #endregion

            #region check appkey 
            // TODO: validate app key
            #endregion

            #region check timestamp expiration
            if (RequireTimestampValidation)
            {
                if (long.TryParse(extractParameter(TS_KEY, context), out long ts))
                {
                    var requestTime = DateTimeOffset.FromUnixTimeSeconds(ts).ToLocalTime().DateTime;
                    if (DateTime.Now.Subtract(requestTime).TotalSeconds > SignatureExpiresIn)
                    {
                        //result.AddError("请求过期");
                        flushResult(-401002, context, result, $"请求过期:在{SignatureExpiresIn}秒内发起有效");
                        return (IsValid: false, Result: result, ActionResult: context.Result);
                    }
                }
                else
                {
                    //result.AddError("时间戳格式错误");
                    flushResult(-401003, context, result, "时间戳格式错误");
                    return (IsValid: false, Result: result, ActionResult: context.Result);
                }
            }

            #endregion

            #region check if signature exists
            string sig = extractParameter(SignatureParameterKey, context, SignatureStorage);
            if (string.IsNullOrEmpty(sig))
            {
                //result.AddError($"缺少必要的参数:{SignatureKey}");
                flushResult(-401004, context, result, $"缺少必要的参数:{SignatureParameterKey}");
                return (IsValid: false, Result: result, ActionResult: context.Result);
            }
            #endregion

            return (IsValid: true, Result: result, ActionResult: context.Result);
        }

        private string getSignatureBaseString(ActionExecutingContext context)
        {
            string apiName = context.HttpContext.Request.Path;
            var sortedQueryWithoutSig = context.HttpContext
                                               .Request.Query
                                               .Where(x => x.Key != SignatureParameterKey && !ExcludeParametersForSignature.Contains(x.Key))
                                               .OrderBy(s => s.Key);

            string querystr = QueryString.Create(sortedQueryWithoutSig)
                                         .ToString();
            return $"{apiName}{querystr}";
        }

        private (bool IsValid, string Message) isSignatureValid(string sigbase, ActionExecutingContext context)
        {
            var sig = extractParameter(SignatureParameterKey, context, SignatureStorage);
            var receivedSignature = WebEncoders.Base64UrlDecode(sig.ToString());


            using var hmac = new HMACSHA256(signingKeyBytes);
            var computedSignature = hmac.ComputeHash(Encoding.UTF8.GetBytes(sigbase));

            bool signaturesMatch = computedSignature.SequenceEqual(receivedSignature);
            string msg = $"[SIGNATURE_BASE] {sigbase}";

            if (_env.IsDevelopment())
            {
                msg += $"\n [EXPECTED SIG] {WebEncoders.Base64UrlEncode(computedSignature)}";
            }

            return (IsValid: signaturesMatch, Message: (signaturesMatch ? "" : msg));
        }
        private IActionResult flushResult(int returnCode, ActionExecutingContext context, ApiSignatureValidationResult result, string errormsg = "")
        {
            if (returnCode != 0 && _logger != null)
            {
                _logger.LogError($"[SortedQueryWithSignatureValidationFilter::flushResult][{returnCode}]{errormsg}");
            }
            result.AddError(errormsg);
            var ret = new ApiResponse<ApiSignatureValidationResult>(returnCode, $"api参数错误:{result.ErrorMessage}");
            ret.Data = result;

            if (AuthorizeFailureResultType == AppAuthorizeFailureResultType.UnAuthorizedResult)
            {
                var rs = new RedirectResult(AuthorizeFailureResultPage);
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                //context.HttpContext.Response.HttpContext.Items.Add("Exception", new AppException(errormsg, -401000));
                context.Result = rs;
                return rs;
            }
            else if (AuthorizeFailureResultType == AppAuthorizeFailureResultType.ViewResult)
            {
                var rs = new ViewResult { ViewName = AuthorizeFailureResultPage };
                if (returnCode != 0)
                {
                    context.ModelState.AddModelError("sorted-query-validation-error", errormsg);
                }
                rs.ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState);
                rs.ViewData.Add("ApiSignatureValidationResult", result);
                rs.ViewData.Add("Exception", new Exception(errormsg));
                context.Result = rs;
                return rs;
            }
            else if (AuthorizeFailureResultType == AppAuthorizeFailureResultType.PlainText)
            {
                var rs = new ContentResult();
                rs.Content = errormsg;
                context.Result = rs;
                return rs;
            }
            else if (AuthorizeFailureResultType == AppAuthorizeFailureResultType.StatusCode)
            {
                var rs = new StatusCodeResult(returnCode);
                context.Result = rs;
                return rs;
            }
            else
            {
                // AppAuthorizeFailureResultType.BadRequestJsonResult                
                var rs = new BadRequestObjectResult(ret);
                context.Result = rs;
                return rs;
            }
        }
        private string extractParameter(string key, ActionExecutingContext context, SignatureParameterType type = SignatureParameterType.QUERY)
        {
            var request = context.HttpContext.Request;

            if (type == SignatureParameterType.QUERY)
            {
                if (request.Query.TryGetValue(key, out var v))
                {
                    return v;
                }
            }
            else if (type == SignatureParameterType.HEADER)
            {
                if (request.Headers.TryGetValue(key, out var v))
                {
                    return v;
                }
            }

            return string.Empty;
        }


        private const string _key = "https://www.2049baby.com";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="hashkey"></param>
        /// <returns></returns>
        private static string GetAppSecret(string appid, string hashkey = _key)
        {
            //string salt = EncryptProvider.Md5(appid).Substring(0, 6).ToLower();

            //string rs = EncryptProvider.HMACSHA256(appid, $"{hashkey}&{salt}").ToLower();

            string salt = md5(appid).Substring(0, 6).ToLower();

            string rs = HmacSha256(appid, $"{hashkey}&{salt}").ToLower();

            return $"{salt}{rs}";
            //return default(string);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        private static string HmacSha256(string message, string secret)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] keyBytes = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            System.Security.Cryptography.HMACSHA256 cryptographer = new HMACSHA256(keyBytes);

            byte[] bytes = cryptographer.ComputeHash(messageBytes);

            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }


        /// <summary>
        /// 计算MD5
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private static string md5(string text)
        {
            var result = default(string);
            using (var algo = MD5.Create())
            {
                result = GenerateHashString(algo, text);
            }
            return result;
        }



        private static string GenerateHashString(HashAlgorithm algo, string text)
        {
            // Compute hash from text parameter
            algo.ComputeHash(Encoding.UTF8.GetBytes(text));

            // Get has value in array of bytes
            var result = algo.Hash;

            // Return as hexadecimal string
            return string.Join(
                string.Empty,
                result.Select(x => x.ToString("x2")));
        }
    }
}
