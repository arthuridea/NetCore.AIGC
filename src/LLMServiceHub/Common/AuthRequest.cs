using System.ComponentModel.DataAnnotations;

namespace LLMServiceHub.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        /// <example></example>
        [Required]
        public string Username {  get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        /// <example></example>
        [Required]
        public string Password {  get; set; }
    }
}
