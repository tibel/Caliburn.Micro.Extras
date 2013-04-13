namespace Caliburn.Micro.Extras {
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// A result decorator which rescues errors from the decorated result by executing a rescue coroutine.
    /// </summary>
    /// <typeparam name="TException">The type of the exception we want to perform the rescue on</typeparam>
    public class RescueResultDecorator<TException> : ResultDecoratorBase where TException : Exception {
        static readonly ILog Log = LogManager.GetLog(typeof(RescueResultDecorator<>));

        readonly bool cancelResult;
        readonly Func<TException, IEnumerable<IResult>> rescue;
        ActionExecutionContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RescueResultDecorator&lt;TException&gt;"/> class.
        /// </summary>
        /// <param name="result">The result to decorate.</param>
        /// <param name="rescue">The rescue coroutine.</param>
        /// <param name="cancelResult">Set to true to cancel the result after executing rescue.</param>
        /// <exception cref="System.ArgumentNullException">rescue</exception>
        public RescueResultDecorator(IResult result, Func<TException, IEnumerable<IResult>> rescue, bool cancelResult = true) : base(result) {
            if (rescue == null)
                throw new ArgumentNullException("rescue");

            this.rescue = rescue;
            this.cancelResult = cancelResult;
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
            var error = args.Error as TException;
            if (error == null) {
                OnCompleted(args);
            }
            else {
                Rescue(error);
            }
        }

        void Rescue(TException exception) {
            var sb = new StringBuilder();
            sb.AppendFormat("Rescued {0}", exception.GetType().FullName).AppendLine();
            sb.AppendLine(exception.Message);
            sb.AppendLine(exception.StackTrace);
            Log.Info(sb.ToString());

            IResult rescueResult;
            try {
                rescueResult = new SequentialResult(rescue(exception).GetEnumerator());
            }
            catch (Exception ex) {
                OnCompleted(new ResultCompletionEventArgs {Error = ex});
                return;
            }

            try {
                rescueResult.Completed += RescueCompleted;
                rescueResult.Execute(context);
            }
            catch (Exception ex) {
                RescueCompleted(rescueResult, new ResultCompletionEventArgs {Error = ex});
            }
        }

        void RescueCompleted(object sender, ResultCompletionEventArgs args) {
            ((IResult)sender).Completed -= RescueCompleted;
            OnCompleted(new ResultCompletionEventArgs
                {
                    Error = args.Error,
                    WasCancelled = (args.Error == null && (args.WasCancelled || cancelResult))
                });
        }
    }
}
