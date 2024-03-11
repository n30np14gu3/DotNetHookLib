using System.Collections.Generic;
using DotNetHookLib.Hooks;
using DotNetHookLib.Tools;

namespace DotNetHookLib
{
    public class Loader
    {
        public static void Instance()
        {
            WinConsole.Initialize();
            List<BaseHookProcessor> processors =  new List<BaseHookProcessor>()
            {
                //Add hooked classes
            };
            foreach (BaseHookProcessor processor in processors)
            {
                processor.SetupHooks();
            }
        }
    }
}