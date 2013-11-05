namespace Caliburn.Micro.Extras {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// A class for managing a weak event.
    /// </summary>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    public class WeakEventSource<TEventHandler> where TEventHandler : class {
        class EventHandlerEntry {
            public readonly MethodInfo TargetMethod;
            public readonly WeakReference TargetReference;
            public int CallCount;
            public Action<object, object, EventArgs> FastCall;

            public EventHandlerEntry(MethodInfo targetMethod, WeakReference targetReference) {
                TargetMethod = targetMethod;
                TargetReference = targetReference;
            }
        }

        readonly int invokationsToCompileDelegate;
        readonly List<EventHandlerEntry> eventHandlerEntries = new List<EventHandlerEntry>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEventSource&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="invokationsToCompileDelegate">The number of invokations on which the delegate will be compiled.</param>
        public WeakEventSource(int invokationsToCompileDelegate = 4) {
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

            if (invokationsToCompileDelegate <= 0)
                throw new ArgumentOutOfRangeException("invokationsToCompileDelegate", "Value must be greater than zero.");
            this.invokationsToCompileDelegate = invokationsToCompileDelegate;
        }

        void RemoveDeadEntries() {
            for (var i = eventHandlerEntries.Count - 1; i >= 0; i--) {
                var entry = eventHandlerEntries[i];
                if (entry.TargetReference != null && !entry.TargetReference.IsAlive)
                    eventHandlerEntries.RemoveAt(i);
            }
        }

        /// <summary>
        /// Adds the specified EventHandler.
        /// </summary>
        /// <param name="eh">The EventHandler.</param>
        public void Add(TEventHandler eh) {
            if (eh == null) return;
            var d = (Delegate) (object) eh;

            var declaringType = d.GetMethodInfo().DeclaringType;
            if (declaringType != null && declaringType.GetCustomAttributes(typeof (CompilerGeneratedAttribute), false).Length != 0)
                throw new ArgumentException("Cannot create weak event to anonymous method with closure.");

            lock (eventHandlerEntries) {
                if (eventHandlerEntries.Count == eventHandlerEntries.Capacity)
                    RemoveDeadEntries();

                var target = d.Target != null ? new WeakReference(d.Target) : null;
                eventHandlerEntries.Add(new EventHandlerEntry(d.GetMethodInfo(), target));
            }
        }

        /// <summary>
        /// Removes the specified EventHandler.
        /// </summary>
        /// <param name="eh">The EventHandler.</param>
        public void Remove(TEventHandler eh) {
            if (eh == null) return;
            var d = (Delegate) (object) eh;

            lock (eventHandlerEntries) {
                for (var i = eventHandlerEntries.Count - 1; i >= 0; i--) {
                    var entry = eventHandlerEntries[i];
                    var target = entry.TargetReference != null ? entry.TargetReference.Target : null;

                    if (target == d.Target && ReferenceEquals(entry.TargetMethod, d.GetMethodInfo())) {
                        eventHandlerEntries.RemoveAt(i);
                        break;
                    }
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

            EventHandlerEntry[] invocationList;
            lock (eventHandlerEntries) {
                invocationList = eventHandlerEntries.ToArray();
            }

            for (var i = 0; i < invocationList.Length; i++) {
                var entry = invocationList[i];
                var target = entry.TargetReference != null ? entry.TargetReference.Target : null;

                if (entry.TargetReference != null && target == null) {
                    needsCleanup = true;
                }
                else {
                    if (entry.FastCall == null) {
                        entry.CallCount++;
                        if (entry.CallCount >= invokationsToCompileDelegate)
                            entry.FastCall = ReflectionHelper.CreateEventHandler(entry.TargetMethod);
                    }

                    if (entry.FastCall != null)
                        entry.FastCall(target, sender, e);
                    else
                        entry.TargetMethod.Invoke(target, parameters);
                }
            }

            if (needsCleanup)
                lock (eventHandlerEntries) {
                    RemoveDeadEntries();
                }
        }
    }
}
