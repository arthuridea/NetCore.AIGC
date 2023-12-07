namespace LLMServiceHub.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class AuthenticationHandlerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationHandlerException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AuthenticationHandlerException(string message) : base(message)
        {
        }
    }
}
