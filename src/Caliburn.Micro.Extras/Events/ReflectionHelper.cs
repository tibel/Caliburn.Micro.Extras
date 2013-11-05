namespace Caliburn.Micro.Extras {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal static class ReflectionHelper {
        public static Action<object, object, EventArgs> CreateEventHandler(MethodInfo method) {
            var target = Expression.Parameter(typeof (object), "target");
            var sender = Expression.Parameter(typeof (object), "sender");
            var args = Expression.Parameter(typeof (EventArgs), "args");

            var eventArgsType = method.GetParameters()[1].ParameterType;
            var convertedArgs = Expression.Convert(args, eventArgsType);

            MethodCallExpression body;
            if (method.IsStatic) {
                body = Expression.Call(null, method, sender, convertedArgs);
            }
            else {
                var convertedTarget = Expression.Convert(target, method.DeclaringType);
                body = Expression.Call(convertedTarget, method, sender, convertedArgs);
            }

            return Expression.Lambda<Action<object, object, EventArgs>>(body, target, sender, args).Compile();
        }

#if !WinRT
        public static MethodInfo GetMethodInfo(this Delegate d) {
            return d.Method;
        }
#else
        public static bool IsSubclassOf(this Type t, Type c) {
            return t.GetTypeInfo().IsSubclassOf(c);
        }

        public static MethodInfo GetMethod(this Type t, string name) {
            return t.GetRuntimeMethods().FirstOrDefault(m => m.Name == name);
        }

        public static Attribute[] GetCustomAttributes(this Type t, Type attributeType, bool inherit) {
            return t.GetTypeInfo().GetCustomAttributes(attributeType, inherit).ToArray();
        }

        public static IEnumerable<PropertyInfo> GetProperties(this Type t) {
            return t.GetRuntimeProperties();
        }
#endif
    }
}
