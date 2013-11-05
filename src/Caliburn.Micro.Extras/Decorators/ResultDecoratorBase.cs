namespace Caliburn.Micro.Extras {
    using System;

    /// <summary>
    /// Base class for all <see cref="IResult"/> decorators.
    /// </summary>
    public abstract class ResultDecoratorBase : IResult {
        readonly IResult innerResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultDecoratorBase"/> class.
        /// </summary>
        /// <param name="result">The result to decorate.</param>
        protected ResultDecoratorBase(IResult result) {
            if (result == null) 
                throw new ArgumentNullException("result");

            innerResult = result;
        }

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void Execute(ActionExecutionContext context) {
            try {
                innerResult.Completed += InnerResultCompleted;
                IoC.BuildUp(innerResult);
                innerResult.Execute(context);
            }
            catch (Exception ex) {
                InnerResultCompleted(innerResult, new ResultCompletionEventArgs { Error = ex });
            }
        }

        void InnerResultCompleted(object sender, ResultCompletionEventArgs args) {
            innerResult.Completed -= InnerResultCompleted;
            OnInnerResultCompleted(innerResult, args);
        }

        /// <summary>
        /// Called when the execution of the decorated result has completed.
        /// </summary>
        /// <param name="innerResult">The decorated result.</param>
        /// <param name="args">The <see cref="ResultCompletionEventArgs"/> instance containing the event data.</param>
        protected abstract void OnInnerResultCompleted(IResult innerResult, ResultCompletionEventArgs args);

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

        /// <summary>
        /// Raises the <see cref="Completed" /> event.
        /// </summary>
        /// <param name="args">The <see cref="ResultCompletionEventArgs"/> instance containing the event data.</param>
        protected void OnCompleted(ResultCompletionEventArgs args) {
            Completed(this, args);
        }
    }
}
