namespace LLMService.DataProvider.Relational.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public static class EntityConsts
    {
        /// <summary>
        /// 
        /// </summary>
        public enum MessageDataType
        {
            /// <summary>
            /// The plain text
            /// </summary>
            PlainText = 0,
            /// <summary>
            /// The HTML
            /// </summary>
            Html = 1,
            /// <summary>
            /// The json
            /// </summary>
            Json = 2,
        }

        /// <summary>
        /// 
        /// </summary>
        public enum MessageContentType
        {
            /// <summary>
            /// The text
            /// </summary>
            Text = 0,
            /// <summary>
            /// The image
            /// </summary>
            Image = 1,
            /// <summary>
            /// The audio
            /// </summary>
            Audio = 2,
            /// <summary>
            /// The video
            /// </summary>
            Video = 3
        }
    }
}
