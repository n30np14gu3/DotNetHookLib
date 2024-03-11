using System.Reflection;
using System;

namespace DotNetHookLib.Hooks.Structs
{
    /// <summary>
    /// Struct with data of methods
    /// </summary>
    public struct HookData
    {
        /// <summary>
        /// Search flags for GetMethods method
        /// </summary>
        public BindingFlags SearchFlags;

        /// <summary>
        /// array of arguments types<br></br>
        /// Example for method bar(bool arg1, int arg2):<br></br>
        /// <example>new Type[] {typeof(bool), typeof(int)}</example>
        /// </summary>
        public Type[] ArgumentTypes;
    }
}