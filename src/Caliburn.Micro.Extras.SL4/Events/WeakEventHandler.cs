namespace Caliburn.Micro.Extras {
    using System;
    using System.Diagnostics.CodeAnalysis;

#if WinRT
    using System.Reflection;
#endif

    /// <summary>
    /// Helper class to add weak handlers to events.
    /// </summary>
    /// <remarks>
    /// Access to the event and to the real event handler is done through lambda expressions.
    /// The code holds strong references to these expressions, so they must not capture any
    /// variables!
    /// </remarks>
    /// <example>
    /// <code>
    /// WeakEventHandler.Register(
    /// 	eh => textDocument.Changed += eh,
    /// 	eh => textDocument.Changed -= eh,
    /// 	this,
    /// 	(me, sender, args) => me.OnDocumentChanged(sender, args),
    /// 	h => new TextChangedEventHandler(h)
    /// );
    /// </code>
    /// </example>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public class WeakEventHandler : IDisposable {
        private readonly WeakReference listeningReference;
        private Action deregisterCode;

        private WeakEventHandler(object listeningObject) {
            listeningReference = new WeakReference(listeningObject);
        }

        private TEventHandler MakeDeregisterCodeAndWeakEventHandler
            <TEventHandler, TEventArgs, TEventListener>
            (
            Func<EventHandler<TEventArgs>, TEventHandler> convert,
            Action<TEventHandler> deregisterEvent,
            Action<TEventListener, object, TEventArgs> forwardAction
            )
            where TEventHandler : class
            where TEventArgs : EventArgs
            where TEventListener : class {
            var handler = convert((s, e) => {
                var listeningObject = (TEventListener) listeningReference.Target;
                if (listeningObject != null) {
                    forwardAction(listeningObject, s, e);
                }
                else {
                    Deregister();
                }
            });

            deregisterCode = () => deregisterEvent(handler);

            return handler;
        }

        void IDisposable.Dispose() {
            Deregister();
        }

        /// <summary>
        /// Deregisters this handler.
        /// </summary>
        public void Deregister() {
            if (deregisterCode != null) {
                deregisterCode();
                deregisterCode = null;
            }
        }

        private static void VerifyDelegate(Delegate d, string parameterName) {
            if (d == null)
                throw new ArgumentNullException(parameterName);
            if (!d.GetMethodInfo().IsStatic)
                throw new ArgumentException(
                    "Delegates used for WeakEventHandler must not capture any variables (must point to static methods)",
                    parameterName);
        }

        /// <summary>
        /// Registers a weak event handler on a event.
        /// </summary>
        public static WeakEventHandler Register<TEventHandler, TEventArgs, TEventListener>(
            Action<TEventHandler> registerEvent,
            Action<TEventHandler> deregisterEvent,
            TEventListener listeningObject,
            Action<TEventListener, object, TEventArgs> forwardAction,
            Func<EventHandler<TEventArgs>, TEventHandler> convert
            )
            where TEventHandler : class
            where TEventArgs : EventArgs
            where TEventListener : class {
            if (registerEvent == null)
                throw new ArgumentNullException("registerEvent");
            if (deregisterEvent == null)
                throw new ArgumentNullException("deregisterEvent");
            if (listeningObject == null)
                throw new ArgumentNullException("listeningObject");
            VerifyDelegate(convert, "convert");
            VerifyDelegate(forwardAction, "forwardAction");

            var weh = new WeakEventHandler(listeningObject);
            var eh = weh.MakeDeregisterCodeAndWeakEventHandler(convert, deregisterEvent, forwardAction);
            registerEvent(eh);
            return weh;
        }

        /// <summary>
        /// Registers a weak event handler on a generic event.
        /// </summary>
        public static WeakEventHandler Register<TEventArgs, TEventListener>(
            Action<EventHandler<TEventArgs>> registerEvent,
            Action<EventHandler<TEventArgs>> deregisterEvent,
            TEventListener listeningObject,
            Action<TEventListener, object, TEventArgs> forwardAction
            )
            where TEventArgs : EventArgs
            where TEventListener : class {
            return Register(registerEvent,
                            deregisterEvent, listeningObject, forwardAction, h => h);
        }
    }
}
