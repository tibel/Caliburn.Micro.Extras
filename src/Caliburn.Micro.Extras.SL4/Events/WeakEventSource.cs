namespace Caliburn.Micro.Extras {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    //TODO: optimize speed by compiling method call into expression (after e.g. three invokations)
    //TODO: thread safety (locking)

    /// <summary>
    /// A class for managing a weak event.
    /// </summary>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    public sealed class WeakEventSource<TEventHandler> where TEventHandler : class {
        private struct EventHandlerEntry {
            public readonly MethodInfo TargetMethod;
            public readonly WeakReference TargetReference;

            public EventHandlerEntry(MethodInfo targetMethod, WeakReference targetReference) {
                TargetMethod = targetMethod;
                TargetReference = targetReference;
            }
        }

        readonly List<EventHandlerEntry> eventHandlerEntries = new List<EventHandlerEntry>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEventSource&lt;T&gt;"/> class.
        /// </summary>
        public WeakEventSource() {
            if (!typeof (TEventHandler).IsSubclassOf(typeof (Delegate)))
                throw new ArgumentException("T must be a delegate type.");
            var invoke = typeof (TEventHandler).GetMethod("Invoke");
            if (invoke == null || invoke.GetParameters().Length != 2)
                throw new ArgumentException("T must be a delegate type taking 2 parameters.");
            var senderParameter = invoke.GetParameters()[0];
            if (senderParameter.ParameterType != typeof (object))
                throw new ArgumentException("The first delegate parameter must be of type 'object'.");
            var argsParameter = invoke.GetParameters()[1];
            if (!(typeof (EventArgs).IsAssignableFrom(argsParameter.ParameterType)))
                throw new ArgumentException("The second delegate parameter must be derived from type 'EventArgs'.");
            if (invoke.ReturnType != typeof (void))
                throw new ArgumentException("The delegate return type must be void.");
        }

        private void RemoveDeadEntries() {
            for (var i = eventHandlerEntries.Count - 1; i >= 0; i--) {
                var entry = eventHandlerEntries[i];
                if (entry.TargetReference != null && !entry.TargetReference.IsAlive) {
                    eventHandlerEntries.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Adds the specified EventHandler.
        /// </summary>
        /// <param name="eh">The EventHandler.</param>
        public void Add(TEventHandler eh) {
            if (eh == null) return;
            var d = (Delegate) (object) eh;

            if (d.GetMethodInfo().DeclaringType.GetCustomAttributes(typeof (CompilerGeneratedAttribute), false).Length != 0)
                throw new ArgumentException("Cannot create weak event to anonymous method with closure.");

            if (eventHandlerEntries.Count == eventHandlerEntries.Capacity)
                RemoveDeadEntries();

            var target = d.Target != null ? new WeakReference(d.Target) : null;
            eventHandlerEntries.Add(new EventHandlerEntry(d.GetMethodInfo(), target));
        }

        /// <summary>
        /// Removes the specified EventHandler.
        /// </summary>
        /// <param name="eh">The EventHandler.</param>
        public void Remove(TEventHandler eh) {
            if (eh == null) return;
            var d = (Delegate) (object) eh;
            
            for (var i = eventHandlerEntries.Count - 1; i >= 0; i--) {
                var entry = eventHandlerEntries[i];
                var target = entry.TargetReference != null ? entry.TargetReference.Target : null;

                if (target == d.Target && ReferenceEquals(entry.TargetMethod, d.GetMethodInfo())) {
                    eventHandlerEntries.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Raises the event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public void Raise(object sender, EventArgs e) {
            var needsCleanup = false;
            var parameters = new[] {sender, e};
            var invocationList = eventHandlerEntries.ToArray();

            for (var i = 0; i < invocationList.Length; i++) {
                var entry = invocationList[i];
                if (entry.TargetReference != null) {
                    var target = entry.TargetReference.Target;
                    if (target != null) {
                        entry.TargetMethod.Invoke(target, parameters);
                    }
                    else {
                        needsCleanup = true;
                    }
                }
                else {
                    entry.TargetMethod.Invoke(null, parameters);
                }
            }

            if (needsCleanup)
                RemoveDeadEntries();
        }
    }
}
