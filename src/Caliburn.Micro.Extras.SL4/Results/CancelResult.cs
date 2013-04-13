namespace Caliburn.Micro.Extras {
    using System;

    /// <summary>
    /// A result that is always canceled.
    /// </summary>
    public class CancelResult : IResult {
        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(ActionExecutionContext context) {
            Completed(this, new ResultCompletionEventArgs {WasCancelled = true});
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}
