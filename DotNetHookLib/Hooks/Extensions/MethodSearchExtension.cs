using System;
using System.Linq;
using System.Reflection;

namespace DotNetHookLib.Hooks.Extensions
{
    public static class MethodSearchExtension
    {

        public static MethodInfo GetMethodByArgs(this Type classType, string methodName, BindingFlags searchFlags, Type[] argTypes)
        {
            var methods = classType.GetMethods(searchFlags).Where(x => x.Name == methodName);
            foreach (var method in methods)
            {
                var args = method.GetParameters();

                bool found = true;
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].ParameterType != argTypes[i])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                    return method;
            }

            return null;

        }

        public static BindingFlags GetBindingFlags(this MethodInfo method)
        {
            BindingFlags flag = BindingFlags.Instance;

            if (method.IsPublic)
                flag |= BindingFlags.Public;

            if (method.IsStatic)
                flag |= BindingFlags.Static;

            if (method.IsPrivate)
                flag |= BindingFlags.NonPublic;

            return flag;
        }

        public static string ToStringWithArgs(this MethodInfo method)
        {
            string name = method.Name;
            foreach (var arg in method.GetParameters())
            {
                name += $"#{arg.ParameterType}";
            }
            return name;
        }
    }
}