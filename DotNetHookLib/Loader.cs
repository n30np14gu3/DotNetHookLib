using DotNetHookLib.Tools;

namespace DotNetHookLib
{
    public class Loader
    {
        public static void Instance()
        {
            WinConsole.Initialize();
        }
    }
}