namespace RestClientApi.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IServerSentEventData
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public interface IServeSentEventHandler
    {
        /// <summary>
        /// Resets this instance.
        /// </summary>
        void Reset();

        /// <summary>
        /// Waits for new item.
        /// </summary>
        /// <param name="mapper">The item notifier.</param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        Task<IServerSentEventData> WaitForNewItem<TSource>(Func<TSource, IServerSentEventData> mapper, TSource dataSource);
    }

    /// <summary>
    /// 
    /// </summary>
    public class ServeSentEventHandler : IServeSentEventHandler
    {
        private TaskCompletionSource<IServerSentEventData> _source = new();
        private long _id = 0;

        internal TaskCompletionSource<IServerSentEventData> Source {get;set;}

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public virtual void Reset()
        {
            _source = new();
        }

        /// <summary>
        /// Waits for new item.
        /// </summary>
        /// <param name="mapper">The item notifier.</param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        public virtual Task<IServerSentEventData> WaitForNewItem<TSource>(Func<TSource, IServerSentEventData> mapper, TSource dataSource)
        {
            // Simulate some delay in Item arrival
            Task.Run(() =>
            {
                mapper.Invoke(dataSource);
            });

            return _source.Task;
        }
    }
}
