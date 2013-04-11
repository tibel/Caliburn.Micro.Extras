namespace Caliburn.Micro.Extras {
#if !SILVERLIGHT || SL5 || WP8
    using System.Threading.Tasks;
#endif

    /// <summary>
    /// Extensions for <see cref="IEventAggregator"/>.
    /// </summary>
    public static class EventAggregatorExtensions {
        /// <summary>
        ///   Publishes a message on the current thread (synchrone).
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name = "message">The message instance.</param>
        public static void PublishOnCurrentThread(this IEventAggregator eventAggregator, object message) {
            eventAggregator.Publish(message, action => action());
        }

        /// <summary>
        ///   Publishes a message on a background thread (async).
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name = "message">The message instance.</param>
        public static void PublishOnBackgroundThread(this IEventAggregator eventAggregator, object message) {
#if !SILVERLIGHT || SL5 || WP8
            eventAggregator.Publish(message, action => System.Threading.Tasks.Task.Factory.StartNew(action));
#else
            eventAggregator.Publish(message, action => System.Threading.ThreadPool.QueueUserWorkItem(state => action()));
#endif
        }

        /// <summary>
        ///   Publishes a message on the UI thread.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name = "message">The message instance.</param>
        public static void PublishOnUIThread(this IEventAggregator eventAggregator, object message) {
            eventAggregator.Publish(message, action => action.OnUIThread());
        }

        /// <summary>
        ///   Publishes a message on the UI thread asynchrone.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name = "message">The message instance.</param>
        public static void BeginPublishOnUIThread(this IEventAggregator eventAggregator, object message) {
            eventAggregator.Publish(message, action => action.BeginOnUIThread());
        }

#if !SILVERLIGHT || SL5 || WP8
        /// <summary>
        ///   Publishes a message on the UI thread asynchrone.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="message">The message instance.</param>
        public static Task PublishOnUIThreadAsync(this IEventAggregator eventAggregator, object message) {
            Task task = null;
            eventAggregator.Publish(message, action => task = action.OnUIThreadAsync());
            return task;
        }
#endif
    }
}
