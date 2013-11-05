namespace Caliburn.Micro.Extras {
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A result decorator which executes a coroutine when the wrapped result was cancelled.
    /// </summary>
    public class ContinueResultDecorator : ResultDecoratorBase {
        static readonly ILog Log = LogManager.GetLog(typeof(ContinueResultDecorator));

        readonly Func<IEnumerable<IResult>> coroutine;
        ActionExecutionContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinueResultDecorator"/> class.
        /// </summary>
        /// <param name="result">The result to decorate.</param>
        /// <param name="coroutine">The coroutine to execute when <paramref name="result"/> was canceled.</param>
        public ContinueResultDecorator(IResult result, Func<IEnumerable<IResult>> coroutine)
            : base(result) {
            if (coroutine == null)
                throw new ArgumentNullException("coroutine");

            this.coroutine = coroutine;
        }

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Execute(ActionExecutionContext context) {
            this.context = context;
            base.Execute(context);
        }

        /// <summary>
        /// Called when the execution of the decorated result has completed.
        /// </summary>
        /// <param name="innerResult">The decorated result.</param>
        /// <param name="args">The <see cref="ResultCompletionEventArgs" /> instance containing the event data.</param>
        protected override void OnInnerResultCompleted(IResult innerResult, ResultCompletionEventArgs args) {
            if (args.Error != null || !args.WasCancelled) {
                OnCompleted(new ResultCompletionEventArgs {Error = args.Error});
            }
            else {
                Log.Info(string.Format("Executing coroutine because {0} was cancelled", innerResult.GetType().Name));

                IResult cancelResult;
                try {
                    cancelResult = new SequentialResult(coroutine().GetEnumerator());
                }
                catch (Exception ex) {
                    OnCompleted(new ResultCompletionEventArgs {Error = ex});
                    return;
                }

                try {
                    cancelResult.Completed += HandleCancelCompleted;
                    cancelResult.Execute(context);
                }
                catch (Exception ex) {
                    HandleCancelCompleted(cancelResult, new ResultCompletionEventArgs {Error = ex});
                }
            }
        }

        void HandleCancelCompleted(object sender, ResultCompletionEventArgs args) {
            ((IResult)sender).Completed -= HandleCancelCompleted;
            OnCompleted(new ResultCompletionEventArgs {Error = args.Error, WasCancelled = (args.Error == null)});
        }
    }
}
