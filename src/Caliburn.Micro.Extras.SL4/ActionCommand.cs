namespace Caliburn.Micro.Extras {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Input;
#if WinRT
    using System.Reflection;
#endif

    /// <summary>
    /// Wraps a ViewModel method (with guard) in an <see cref="ICommand"/>.
    /// </summary>
    public class ActionCommand : ICommand, IDisposable {
        readonly ActionExecutionContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionCommand"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="methodName">Name of the method.</param>
        public ActionCommand(object target, string methodName) {
            if (target == null)
                throw new ArgumentNullException("target");

#if WinRT
            var method = target.GetType().GetRuntimeMethod(methodName, new Type[0]);
#else
            var method = target.GetType().GetMethod(methodName, Type.EmptyTypes);
#endif
            if (method == null)
                throw new ArgumentException(@"Specified method cannot be found.", "methodName");

            context = new ActionExecutionContext {
                Target = target,
                Method = method,
            };

            SetupCanExecute(context, () => CanExecuteChanged(this, EventArgs.Empty));
        }

        /// <summary>
        /// Freeing held references.
        /// </summary>
        public void Dispose() {
            context.Dispose();
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter) {
            var returnValue = context.Method.Invoke(context.Target, new object[0]);

#if !SILVERLIGHT || SL5 || WP8
            var task = returnValue as System.Threading.Tasks.Task;
            if (task != null) {
                returnValue = task.AsResult();
            }
#endif

            var result = returnValue as IResult;
            if (result != null) {
                returnValue = new[] { result };
            }

            var enumerable = returnValue as IEnumerable<IResult>;
            if (enumerable != null) {
                returnValue = enumerable.GetEnumerator();
            }

            var enumerator = returnValue as IEnumerator<IResult>;
            if (enumerator != null) {
                Coroutine.BeginExecute(enumerator, context);
            }
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter) {
            return context.CanExecute == null || context.CanExecute();
        }

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged = delegate { };

        static void SetupCanExecute(ActionExecutionContext context, System.Action raiseCanExecuteChanged) {
            var inpc = context.Target as INotifyPropertyChanged;
            if (inpc == null) return;
            
            var guardName = "Can" + context.Method.Name;
            var targetType = context.Target.GetType();
#if WinRT
            var guard = targetType.GetRuntimeMethod("get_" + guardName, new Type[0]);
#else
            var guard = targetType.GetMethod("get_" + guardName, Type.EmptyTypes);
#endif
            if (guard == null) return;

            SetupCanExecuteChanged(context, inpc, guardName, raiseCanExecuteChanged);

            context.CanExecute = () => (bool)guard.Invoke(context.Target, new object[0]);
        }

        static void SetupCanExecuteChanged(ActionExecutionContext context, INotifyPropertyChanged inpc,
                                           string guardName, System.Action raiseCanExecuteChanged) {
            PropertyChangedEventHandler propertyChangedHandler = null;
            propertyChangedHandler = (s, e) => {
                    if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == guardName) {
                        raiseCanExecuteChanged.OnUIThread();
                    }
                };
            inpc.PropertyChanged += propertyChangedHandler;
            context.Disposing += delegate { inpc.PropertyChanged -= propertyChangedHandler; };

            var deactivatable = inpc as IDeactivate;
            if (deactivatable == null) return;

            EventHandler<DeactivationEventArgs> deactivatableHandler = null;
            deactivatableHandler = (s, e) => {
                    if (!e.WasClosed) return;
                    deactivatable.Deactivated -= deactivatableHandler;
                    context.Dispose();
                };
            deactivatable.Deactivated += deactivatableHandler;
            context.Disposing += delegate { deactivatable.Deactivated -= deactivatableHandler; };
        }
    }
}
