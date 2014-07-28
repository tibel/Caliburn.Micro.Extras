namespace Caliburn.Micro.Extras {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows.Input;
    using Weakly;

    /// <summary>
    /// Wraps a ViewModel method (with guard) in an <see cref="ICommand"/>.
    /// </summary>
    public class ActionCommand : ICommand {
        readonly WeakReference targetReference;
        readonly MethodInfo method;
        readonly WeakFunc<bool> canExecute;
        readonly WeakEventSource canExecuteChangedSource = new WeakEventSource();
        readonly string guardName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionCommand"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="methodName">Name of the method.</param>
        public ActionCommand(object target, string methodName) {
            if (target == null)
                throw new ArgumentNullException("target");

            targetReference = new WeakReference(target);
            method = target.GetType().GetMethod(methodName);
            if (method == null)
                throw new ArgumentException(@"Specified method cannot be found.", "methodName");

            guardName = "Can" + method.Name;
            var guard = target.GetType().GetMethod("get_" + guardName);
            var inpc = target as INotifyPropertyChanged;
            if (inpc == null || guard == null) return;

            WeakEventHandler.Register<PropertyChangedEventArgs>(inpc, "PropertyChanged", OnPropertyChanged);
            canExecute = new WeakFunc<bool>(inpc, guard);
        }

        void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == guardName) {
                Micro.Execute.OnUIThread(() => canExecuteChangedSource.Raise(this, EventArgs.Empty));
            }
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter) {
            var target = targetReference.Target;
            if (target == null) return;

            var execute = DynamicDelegate.From(method);
            var returnValue = execute(target, new object[0]);

            var task = returnValue as System.Threading.Tasks.Task;
            if (task != null) {
                returnValue = task.AsResult();
            }

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
                var context = new CoroutineExecutionContext
                {
                    Target = target,
                };

                Coroutine.BeginExecute(enumerator, context);
            }
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter) {
            return canExecute == null || canExecute.Invoke();
        }

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged {
            add { canExecuteChangedSource.Add(value); }
            remove { canExecuteChangedSource.Remove(value); }
        }
    }
}
