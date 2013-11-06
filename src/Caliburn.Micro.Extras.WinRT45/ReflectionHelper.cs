using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Caliburn.Micro.Extras
{
    internal static class ReflectionHelper
    {
        public static bool IsSubclassOf(this Type t, Type c)
        {
            return t.GetTypeInfo().IsSubclassOf(c);
        }

        public static MethodInfo GetMethod(this Type t, string name)
        {
            return t.GetRuntimeMethods().FirstOrDefault(m => m.Name == name);
        }

        public static Attribute[] GetCustomAttributes(this Type t, Type attributeType, bool inherit)
        {
            return t.GetTypeInfo().GetCustomAttributes(attributeType, inherit).ToArray();
        }

        public static IEnumerable<PropertyInfo> GetProperties(this Type t)
        {
            return t.GetRuntimeProperties();
        }
    }
}
