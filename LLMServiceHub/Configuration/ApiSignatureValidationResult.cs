namespace LLMServiceHub.Configuration
{

    /// <summary>
    /// 
    /// </summary>
    public class ApiSignatureValidationResult
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> Errors { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ApiSignatureValidationResult()
        {
            Errors = new List<string>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void AddError(string message)
        {
            Errors.Add(message);
        }

        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessage => string.Join(";", Errors);
    }
}
