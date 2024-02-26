using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LLMServiceHub.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class AppAuthenticateManager : IAppAuthenticateManager
    {
        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration _configuration;
        /// <summary>
        /// The context accessor
        /// </summary>
        private readonly IHttpContextAccessor _contextAccessor;
        /// <summary>
        /// The issuer
        /// </summary>
        private string issuer;
        private string audience;
        private byte[] creadentialSigningKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppAuthenticateManager"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="accessor"></param>
        public AppAuthenticateManager(IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _configuration = configuration;
            _contextAccessor = accessor;


            issuer = _configuration["JWtConfig:Issuer"] ?? "";
            audience = _configuration["JWtConfig:Audience"] ?? "";
            string securityKey = _configuration["JWtConfig:SecurityKey"] ?? "";
            creadentialSigningKey = Encoding.UTF8.GetBytes(securityKey);

        }
        /// <summary>
        /// Creates the JWT token.
        /// </summary>
        /// <param name="identityUserName">Name of the identity user.</param>
        /// <param name="requireIdentityAuthentication">if set to <c>true</c> [require identity authentication].</param>
        /// <param name="enableJwtEnryption">if set to <c>true</c> [enable JWT enryption].</param>
        /// <param name="jwtEncryptionKey">The JWT encryption key.must be 16 character</param>
        /// <param name="tokenExpiresIn">The token expires in.</param>
        /// <returns></returns>
        public async Task<JwtAccessToken> CreateJwtToken(
            string identityUserName,
            bool requireIdentityAuthentication = false,
            bool enableJwtEnryption = false, string jwtEncryptionKey = null,
            long tokenExpiresIn = 7200)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, identityUserName),
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(claims, BearerTokenDefaults.AuthenticationScheme);

            var jwtToken = _createJwtTokenFromClaims(identityUserName, claims, enableJwtEnryption, jwtEncryptionKey, tokenExpiresIn);


            if (requireIdentityAuthentication)
            {
                await _contextAccessor.HttpContext.SignInAsync(
                            scheme: BearerTokenDefaults.AuthenticationScheme,
                            principal: new ClaimsPrincipal(identity),
                            properties: new AuthenticationProperties
                            {
                                IsPersistent = false
                            });
            }

            return jwtToken;
        }

        /// <summary>
        /// Executes the sign in.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <param name="identityUserName">Name of the identity user.</param>
        /// <param name="rememberme">if set to <c>true</c> [rememberme].</param>
        /// <param name="returnUrl">The return URL.</param>
        public async Task ExecSignIn(string scheme, string identityUserName, bool rememberme, string returnUrl)
        {
            // set authentication cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, identityUserName),
                new Claim(ClaimTypes.Role, "User")
            };
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberme,// persistance.
                RedirectUri = string.IsNullOrWhiteSpace(returnUrl) ? "/Index" : returnUrl,
            };
            var identity = new ClaimsIdentity(claims, scheme);
            var principal = new ClaimsPrincipal(identity);
            await _contextAccessor.HttpContext.SignInAsync(scheme, principal, authProperties);
        }

        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public (bool Succeeded, string IdentityName, string Message) ValidateUser(string username, string password)
        {
            var ret = (Succeeded: false, IdentityName: "", Message: "用户不存在");
            var validusers = _configuration.GetSection("Users")
                                           .Get<List<BuildInUser>>();
            var user = validusers.FirstOrDefault(x => x.UserName == username && x.Password == password);
            if (user != null)
            {
                ret.Succeeded = true;
                ret.IdentityName = user.DisplayName;
                ret.Message = "ok";
            }
            return ret;
        }

        /// <summary>
        /// Creates the JWT token from claims.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="initClaims">The initialize claims.</param>
        /// <param name="enableJwtEncryption">if set to <c>true</c> [enable JWT encryption].</param>
        /// <param name="jwtEncryptionKey">The JWT encryption key. must be 16 character</param>
        /// <param name="tokenExpiresInSeconds">The token expires in seconds.</param>
        /// <returns></returns>
        private JwtAccessToken _createJwtTokenFromClaims(
            string name,
            IEnumerable<Claim> initClaims,
            bool enableJwtEncryption = false,
            string jwtEncryptionKey = null,
            long tokenExpiresInSeconds = 7200)
        {
            var securityKey = new SymmetricSecurityKey(creadentialSigningKey);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Now its ime to define the jwt token which will be responsible of creating our tokens
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            //var principal = await _signinManager.CreateUserPrincipalAsync(user);
            var claims = new List<Claim>();
            claims.AddRange(initClaims);
            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, name),
                // the JTI is used for our refresh token which we will be convering in the next video
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            });
            var jwtClaimsIdentity = new ClaimsIdentity(claims);

            var issuedTime = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = jwtClaimsIdentity,
                // the life span of the token needs to be shorter and utilise refresh token to keep the user signedin
                // but since this is a demo app we can extend it to fit our current need
                Expires = issuedTime.AddMinutes(tokenExpiresInSeconds),
                // here we are adding the encryption alogorithim information which will be used to decrypt our token
                SigningCredentials = credentials,
                Issuer = issuer,
                Audience = audience,
            };
            if (enableJwtEncryption && !string.IsNullOrEmpty(jwtEncryptionKey))
            {
                var encryptionkey = Encoding.UTF8.GetBytes(jwtEncryptionKey); //must be 16 character
                var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionkey), JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes128CbcHmacSha256);
                tokenDescriptor.EncryptingCredentials = encryptingCredentials;
            }
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = jwtTokenHandler.WriteToken(token);

            return new JwtAccessToken
            {
                AccessToken = jwtToken,
                IssuedAt = ((DateTimeOffset)issuedTime).ToUnixTimeSeconds(),
                ExpiresInSeconds = tokenExpiresInSeconds,
            };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IAppAuthenticateManager
    {
        /// <summary>
        /// Creates the JWT token.
        /// </summary>
        /// <param name="identityUserName">Name of the identity user.</param>
        /// <param name="requireIdentityAuthentication">if set to <c>true</c> [require identity authentication].</param>
        /// <param name="enableJwtEnryption">if set to <c>true</c> [enable JWT enryption].</param>
        /// <param name="jwtEncryptionKey">The JWT encryption key.must be 16 character</param>
        /// <param name="tokenExpiresIn">The token expires in.</param>
        /// <returns></returns>
        Task<JwtAccessToken> CreateJwtToken(
            string identityUserName,
            bool requireIdentityAuthentication = false,
            bool enableJwtEnryption = false, string jwtEncryptionKey = null,
            long tokenExpiresIn = 7200);
        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        (bool Succeeded, string IdentityName, string Message) ValidateUser(string username, string password);

        /// <summary>
        /// Executes the sign in.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <param name="identityUserName">Name of the identity user.</param>
        /// <param name="rememberme">if set to <c>true</c> [rememberme].</param>
        /// <param name="returnUrl">The return URL.</param>
        Task ExecSignIn(string scheme, string identityUserName, bool rememberme, string returnUrl);
    }
}
