using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Security.Cryptography;

namespace LLMServiceHub.Configuration
{
    /// <summary>
    /// sorted parameter with signature api operation filter
    /// !!!NOTE: this filter must be registerd after EnableAnnotations(if any) invoked as global description may be overwrite by specific data annotation settings
    /// </summary>
    public class SortedQueryWithSignatureParameterOperationFilter : IOperationFilter
    {
        const string _descript = @"<h4>Api签名说明:</h4><br />
                                  使用 <code>app</code>,<code>ts</code>,<code>nonce</code>,<code>sig</code>参数构建应用于controller或action级别的授权认证.签名算法如下:<br />
                                  1.将QueryString中,除签名参数<code>sig</code>外的参数按字母表顺序排序<br /> 
                                  2.使用上述已排序的参数构建字符串$orderedparameters: <code>?key1=encode({value1})&amp;key2=encode({value2})....</code>备用.<br /> 
                                  3.连接Api终结点<code>$endpoint</code>构建形如<code>{$endpoint}{$orderedparameters}</code>的签名字符串<code>$sigbase</code>.<br />
                                  4.计算哈希值<b>HMACSHA256($sigbase, appsecret)</b>作为签名参数sig.";
        /// <summary>
        /// additional api summary
        /// </summary>
        protected string Description { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public SortedQueryWithSignatureParameterOperationFilter(string description = _descript)
        {
            Description = description;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                                    .Union(context.MethodInfo.GetCustomAttributes(true))
                                    .OfType<SortedQueryWithSignatureValidationFilter>();
            // add global parameters
            if (attributes.Any())
            {
                var filter = attributes.First();

                if (operation.Parameters == null)
                {
                    operation.Parameters = new List<OpenApiParameter>();
                }

                var signatrueLocation = filter.SignatureStorage switch
                {
                    SignatureParameterType.QUERY => ParameterLocation.Query,
                    SignatureParameterType.HEADER => ParameterLocation.Header,
                    _ => ParameterLocation.Query
                };

                #region add global parameters

                string __defaultKey = "01234567890abcd";
                long __ts = _getUnixTimestamp();
                string __nonce = _randString(8);

                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = filter.AppKeyParameterName,
                    In = ParameterLocation.Query,
                    Description = "Api consumer key",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Example = new OpenApiString(__defaultKey)
                    }
                });

                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = filter.SignatureParameterKey,
                    In = signatrueLocation,
                    Description = "签名",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
                });

                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = filter.NonceParameterName,
                    In = ParameterLocation.Query,
                    Description = "单次值(避免重放攻击)",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Example = new OpenApiString(__nonce)
                    }
                });

                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = filter.TimestampParameterName,
                    In = ParameterLocation.Query,
                    Description = "Unix时间戳(秒)",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "integer",
                        Format = "int64",
                        Example = new OpenApiLong(__ts)
                    }
                });
                #endregion

                #region add additional global description

                // description
                var operationattributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                                                            .Union(context.MethodInfo.GetCustomAttributes(true))
                                                            .OfType<SwaggerOperationAttribute>();
                if (operationattributes.Any())
                {
                    var attr = operationattributes.FirstOrDefault();
                    string description = attr.Description;
                    if (!string.IsNullOrEmpty(description))
                    {
                        operation.Description = $"{Description}<br />{description}";
                    }
                }
                else
                {
                    operation.Description = Description;
                }
                #endregion
            }

        }


        /// <summary>
        /// 获得unix时间戳（秒）
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static long _getUnixTimestamp(DateTime date)
        {
            DateTimeOffset dto = new DateTimeOffset(date);
            return dto.ToUnixTimeSeconds();
        }

        /// <summary>
        /// 获得unix时间戳（秒）
        /// </summary>
        /// <returns></returns>
        private static long _getUnixTimestamp()
        {
            return _getUnixTimestamp(DateTime.Now);
        }


        static readonly char[] AvailableCharacters = {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
          };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        private static string _randString(int length, string chars = "")
        {
            char[] identifier = new char[length];
            byte[] randomData = new byte[length];

            var chararray = AvailableCharacters;

            if (!string.IsNullOrEmpty(chars))
            {
                chararray = chars.ToCharArray();
            }
#if NETCOREAPP3_1 || NET5_0
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomData);
            }
#elif NET6_0_OR_GREATER
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomData);
            }
#endif

            for (int idx = 0; idx < identifier.Length; idx++)
            {
                int pos = randomData[idx] % chararray.Length;
                identifier[idx] = chararray[pos];
            }

            return new string(identifier);
        }

    }
}
