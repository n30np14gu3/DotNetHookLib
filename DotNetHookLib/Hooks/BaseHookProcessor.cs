using DotNetHookLib.Hooks.Structs;
using DotNetHookLib.Tools;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using DotNetHookLib.Hooks.Attributes;
using DotNetHookLib.Hooks.Extensions;

namespace DotNetHookLib.Hooks
{
    /// <summary>
    /// Abstract class for hooks
    /// </summary>
    public abstract class BaseHookProcessor
    {
        /// <summary>
        /// Type of class with methods to hook
        /// </summary>
        protected Type ClassHookType = null;

        /// <summary>
        /// Dictionary with methods to hooks
        /// Key: method name<br></br>
        /// Value: Hood data - struct with search flags & args array<br></br>
        /// </summary>
        protected static Dictionary<string, HookData> HookedMethods = new Dictionary<string, HookData>();

        /// <summary>
        /// Dictionary with hook processors<br></br>
        /// Key - method name<br></br>
        /// Value - processor class<br></br>
        /// </summary>
        protected static Dictionary<string, DotNetHook> HookedMethodsProcessors = new Dictionary<string, DotNetHook>();

        public void SetupHooks()
        {
            InitClassType();
            InitHookedMethods();
            InitHookProcessors();
        }

        protected void InitHookedMethods()
        {

            MethodInfo[] hookingMethods = GetType().GetMethods(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Static
                ).Where(x => x.CustomAttributes.Count(y => y.AttributeType == typeof(HookedMethod)) != 0).ToArray();


            foreach (var method in hookingMethods)
            {
                HookedMethods.Add(method.ToStringWithArgs(), new HookData()
                {
                    ArgumentTypes = method.GetParameters().Select(x => x.ParameterType).ToArray(),
                    SearchFlags = method.GetBindingFlags()
                });
            }

            PropertyInfo[] hookingProperties = GetType().GetProperties(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Static
            ).Where(x => x.CustomAttributes.Count(y => y.AttributeType == typeof(HookedProperty)) != 0).ToArray();
            foreach (var p in hookingProperties)
            {
                //TODO: add setters hooks
                MethodInfo getMethod = p.GetGetMethod();
                HookedMethods.Add(getMethod.ToStringWithArgs(), new HookData()
                {
                    ArgumentTypes = getMethod.GetParameters().Select(x => x.ParameterType).ToArray(),
                    SearchFlags = getMethod.GetBindingFlags()
                });
            }
        }

        protected abstract void InitClassType();


        private void InitHookProcessors()
        {
            foreach (var method in HookedMethods)
            {
                //Get method from hooked class by name, binding fields & args
                MethodInfo method2Hook =
                    ClassHookType.GetMethodByArgs(method.Key.Split('#')[0], method.Value.SearchFlags, method.Value.ArgumentTypes);
                if (method2Hook == null)
                    continue;

                //Get method from hooking class with same name & same args & private access
                MethodInfo method2Replace = GetType()
                    .GetMethodByArgs(method2Hook.Name, method.Value.SearchFlags,
                        method.Value.ArgumentTypes);

                if (method2Replace == null)
                    continue;

                HookedMethodsProcessors.Add(method2Hook.ToStringWithArgs(), new DotNetHook(method2Hook, method2Replace));
                HookedMethodsProcessors[method2Hook.ToStringWithArgs()].Hook();
#if DEBUG
                Console.WriteLine($"Hooked {method2Hook.Name}");
#endif
            }
        }
    }
}