using Asp.Versioning;
using LLMServiceHub.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LLMServiceHub.Controllers.v1_0
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiExplorerSettings(GroupName = "Auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        /// <summary>
        /// The application authenticate manager
        /// </summary>
        private readonly IAppAuthenticateManager _appAuthenticateManager;
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthApiController"/> class.
        /// </summary>
        /// <param name="appAuthenticateManager">The application authenticate manager.</param>
        public AuthApiController(IAppAuthenticateManager appAuthenticateManager)
        {
            _appAuthenticateManager = appAuthenticateManager;
        }

        /// <summary>
        /// 获取jwt token
        /// </summary>
        /// <param name="request">jwt请求对象，包含用户名密码</param>
        /// <returns></returns>
        [HttpPost("token")]
        [ProducesResponseType(typeof(string), 200)]
        [AppExceptionInterceptor(ReturnCode = -300001, ApiVersion = "1.0")]
        public async Task<IActionResult> Token([FromBody]AuthRequest request)
        {
            var result = _appAuthenticateManager.ValidateUser(request.Username, request.Password);
            if (result.Succeeded)
            {
                var token = await _appAuthenticateManager.CreateJwtToken(request.Username);
                return Ok(token);
            }
            else
            {
                return Unauthorized();
            }
        }
        /// <summary>
        /// Claimses this instance.
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{AppConsts.DefaultAuthScheme}")]
        [HttpGet("claims")]
        public IActionResult Claims()
        {
            var claims = User.Claims.ToList();
            return Ok(claims.Select(x => new
            {
                x.Type,
                x.Value
            }));
        }
    }
}
