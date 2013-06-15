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
    /// 	textDocument,
    /// 	(d, eh) => d.Changed += eh,
    /// 	(d, eh) => d.Changed -= eh,
    /// 	this,
    /// 	(me, sender, args) => me.OnDocumentChanged(sender, args)
    /// );
    /// </code>
    /// </example>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public class WeakEventHandler : IDisposable {
        readonly WeakReference listeningReference;
        Action deregisterCode;

        private WeakEventHandler(object listeningObject) {
            listeningReference = new WeakReference(listeningObject);
        }

        private TEventHandler MakeDeregisterCodeAndWeakEventHandler
            <TEventSource, TEventHandler, TEventArgs, TEventListener>
            (
            Func<EventHandler<TEventArgs>, TEventHandler> convert,
            TEventSource senderObject,
            Action<TEventSource, TEventHandler> deregisterEvent,
            Action<TEventListener, object, TEventArgs> forwardAction
            )
            where TEventSource : class
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

            deregisterCode = () => deregisterEvent(senderObject, handler);

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
        /// Registers an event handler on an instance event.
        /// </summary>
        public static WeakEventHandler Register<TEventSource, TEventHandler, TEventArgs, TEventListener>(
            Func<EventHandler<TEventArgs>, TEventHandler> convert,
            TEventSource senderObject,
            Action<TEventSource, TEventHandler> registerEvent,
            Action<TEventSource, TEventHandler> deregisterEvent,
            TEventListener listeningObject,
            Action<TEventListener, object, TEventArgs> forwardAction
            )
            where TEventSource : class
            where TEventHandler : class
            where TEventArgs : EventArgs
            where TEventListener : class {
            if (senderObject == null)
                throw new ArgumentNullException("senderObject");
            if (listeningObject == null)
                throw new ArgumentNullException("listeningObject");
            VerifyDelegate(registerEvent, "registerEvent");
            VerifyDelegate(deregisterEvent, "deregisterEvent");
            VerifyDelegate(forwardAction, "forwardAction");

            var weh = new WeakEventHandler(listeningObject);
            var eh = weh.MakeDeregisterCodeAndWeakEventHandler(convert, senderObject, deregisterEvent, forwardAction);
            registerEvent(senderObject, eh);
            return weh;
        }

        /// <summary>
        /// Registers an event handler on a generic instance event.
        /// </summary>
        public static WeakEventHandler Register<TEventSource, TEventArgs, TEventListener>(
            TEventSource senderObject,
            Action<TEventSource, EventHandler<TEventArgs>> registerEvent,
            Action<TEventSource, EventHandler<TEventArgs>> deregisterEvent,
            TEventListener listeningObject,
            Action<TEventListener, object, TEventArgs> forwardAction
            )
            where TEventSource : class
            where TEventArgs : EventArgs
            where TEventListener : class {
            return Register(h => h, senderObject, registerEvent, deregisterEvent, listeningObject, forwardAction);
        }

        /// <summary>
        /// Registers an event handler on a static event.
        /// </summary>
        public static WeakEventHandler Register<TEventHandler, TEventArgs, TEventListener>(
            Func<EventHandler<TEventArgs>, TEventHandler> convert,
            Action<TEventHandler> registerEvent,
            Action<TEventHandler> deregisterEvent,
            TEventListener listeningObject,
            Action<TEventListener, object, TEventArgs> forwardAction
            )
            where TEventHandler : class
            where TEventArgs : EventArgs
            where TEventListener : class
        {
            if (listeningObject == null)
                throw new ArgumentNullException("listeningObject");
            VerifyDelegate(registerEvent, "registerEvent");
            VerifyDelegate(deregisterEvent, "deregisterEvent");
            VerifyDelegate(forwardAction, "forwardAction");

            var weh = new WeakEventHandler(listeningObject);
            var eh = weh.MakeDeregisterCodeAndWeakEventHandler(convert, (object)null, (s, h) => deregisterEvent(h), forwardAction);
            registerEvent(eh);
            return weh;
        }

        /// <summary>
        /// Registers an event handler on a generic static event.
        /// </summary>
        public static WeakEventHandler Register<TEventArgs, TEventListener>(
            Action<EventHandler<TEventArgs>> registerEvent,
            Action<EventHandler<TEventArgs>> deregisterEvent,
            TEventListener listeningObject,
            Action<TEventListener, object, TEventArgs> forwardAction
            )
            where TEventArgs : EventArgs
            where TEventListener : class {
            return Register(h => h, registerEvent,
                            deregisterEvent, listeningObject, forwardAction);
        }
    }
}
