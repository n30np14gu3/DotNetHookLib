using System.Reflection;
using DotNetHookLib.Hooks;
using DotNetHookLib.Hooks.Attributes;
using DotNetHookLib.Hooks.Extensions;
using ExampleLib;

namespace DotNetHookLib.Example
{
    public class Class2Hook : BaseHookProcessor
    {
        protected override void InitClassType()
        {
            ClassHookType = typeof(ExampleClass2Hook);
        }

        [HookedMethod]
        public void UpdateName(string name)
        {
            name = $"Hooked {name}";
            MethodInfo method = (MethodInfo)MethodBase.GetCurrentMethod();
            HookedMethodsProcessors[method.ToStringWithArgs()].Unhook();
            HookedMethodsProcessors[method.ToStringWithArgs()].OriginalMethod.Invoke(this, new object[] { name });
            HookedMethodsProcessors[method.ToStringWithArgs()].Hook();
        }
    }
}