using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utility.Tools
{
    public static class Reflector
    {
        /*
            Class used to perform reflection calls
        */

        public static IEnumerable<Type> GetTypesWithAttribute<TAttribute>(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TAttribute), true).Length > 0)
                {
                    yield return type;
                }
            }
        }
        public static IEnumerable<object> MakeInstancesByAttribute<T>()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                                          .SelectMany(a => GetTypesWithAttribute<T>(a))
                                          .Select(t => Activator.CreateInstance(t));
        }

        public static IEnumerable<TAttribute> GetAttributesInAssembly<TAttribute>()
        {
            return AppDomain.CurrentDomain
                            .GetAssemblies()
                            .SelectMany(a => a.GetTypes())
                            .SelectMany(t => t.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>());                            
        }

        public static IEnumerable<K> MakeInstancesByAttribute<T, K>()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                                          .SelectMany(a => GetTypesWithAttribute<T>(a))
                                          .Select(t => (K)Activator.CreateInstance(t));
        }

        public static IEnumerable<MethodInfo> GetMethodsWithAttribute<T>(object instance)
        {
            return instance.GetType().GetMethods().Where(m => m.CustomAttributes.Any(at => at.AttributeType == typeof(T)));
        }

        public static T GetAttribute<T>(object obj)
        {
            return obj.GetType().GetCustomAttributes(typeof(T), true).Select(at => (T)at).First();
        }
    }
}
