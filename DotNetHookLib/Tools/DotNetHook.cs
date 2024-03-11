using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;

namespace DotNetHookLib.Tools
{
    /// <summary>
    /// Class for hooking managed functions
    /// </summary>
    public class DotNetHook
    {
        private const uint HookSizeX64 = 12;
        private const uint HookSizeX86 = 7;

        /// <summary>
        /// Original function bytes
        /// </summary>
        private byte[] _original;

        /// <summary>
        /// Original method info
        /// </summary>
        public MethodInfo OriginalMethod { get; }

        /// <summary>
        /// Hooked method info
        /// </summary>
        public MethodInfo HookMethod { get; }

        /// <summary>
        /// Hook cctor
        /// </summary>
        /// <param name="origin">Origin method</param>
        /// <param name="hook">Method to replace</param>
        /// <exception cref="ArgumentNullException">Throw if one of args is null</exception>
        public DotNetHook(MethodInfo origin, MethodInfo hook)
        {

            if (origin == null)
                throw new ArgumentNullException("origin");

            if (hook == null)
                throw new ArgumentNullException("hook");

            RuntimeHelpers.PrepareMethod(origin.MethodHandle);
            RuntimeHelpers.PrepareMethod(hook.MethodHandle);

            OriginalMethod = origin;
            HookMethod = hook;
            _original = null;
        }

        /// <summary>
        /// Set UP hook
        /// </summary>
        /// <exception cref="ArgumentException">Throw if function is hooked</exception>
        /// <exception cref="Win32Exception">Throw when can't change page protection</exception>
        /// <exception cref="NullReferenceException">Throw when can't access to unmanaged memory while patching</exception>
        public void Hook()
        {
            if (_original != null)
                throw new ArgumentException("Function already hooked");

            IntPtr funcFrom = OriginalMethod.MethodHandle.GetFunctionPointer();
            IntPtr funcTo = HookMethod.MethodHandle.GetFunctionPointer();

            uint oldProtection;
            if (IntPtr.Size == 8) //Is x64 process
            {
                _original = new byte[HookSizeX64];

                if (!VirtualProtect(funcFrom, HookSizeX64, 0x40 /*PAGE_READWRITE*/, out oldProtection))
                    throw new Win32Exception("Can't change page protection");

                unsafe
                {
                    byte* ptr = (byte*)funcFrom;

                    for (int i = 0; i < HookSizeX64; ++i)
                    {
                        if (ptr == null)
                            throw new NullReferenceException("Can't access to memory");
                        _original[i] = ptr[i];
                    }

                    // movabs rax, addy
                    // jmp rax
                    *(ptr) = 0x48;
                    *(ptr + 1) = 0xb8;
                    *(IntPtr*)(ptr + 2) = funcTo;
                    *(ptr + 10) = 0xff;
                    *(ptr + 11) = 0xe0;
                    VirtualProtect(funcFrom, HookSizeX64, oldProtection, out _);
                }
            }
            else
            {
                _original = new byte[HookSizeX86];
                if (!VirtualProtect(funcFrom, HookSizeX86, 0x40 /*PAGE_READWRITE*/, out oldProtection))
                    throw new Win32Exception("Can't change page protection");

                unsafe
                {
                    byte* ptr = (byte*)funcFrom;

                    for (int i = 0; i < HookSizeX86; ++i)
                    {
                        if (ptr == null)
                            throw new NullReferenceException("Can't access to memory");
                        _original[i] = ptr[i];
                    }

                    // mov eax, addy
                    // jmp eax
                    *(ptr) = 0xb8;
                    *(IntPtr*)(ptr + 1) = funcTo;
                    *(ptr + 5) = 0xff;
                    *(ptr + 6) = 0xe0;
                    VirtualProtect(funcFrom, HookSizeX86, oldProtection, out _);
                }


            }

        }

        /// <summary>
        /// Restore origin function bytes (delete hook)
        /// </summary>
        /// <exception cref="ArgumentException">Throw if function not hooked</exception>
        /// <exception cref="NullReferenceException">Throw when can't access to unmanaged memory</exception>
        public void Unhook()
        {
            if (_original == null)
                throw new ArgumentException("Function is not hooked");

            uint oldProt;
            uint codeSize = (uint)_original.Length;
            IntPtr origAddr = OriginalMethod.MethodHandle.GetFunctionPointer();
            VirtualProtect(origAddr, codeSize, 0x40 /*PAGE_READWRITE*/, out oldProt);
            unsafe
            {
                byte* ptr = (byte*)origAddr;
                for (var i = 0; i < codeSize; ++i)
                {
                    if (ptr == null)
                        throw new NullReferenceException("Can't access to memory");
                    ptr[i] = _original[i];
                }
            }
            VirtualProtect(origAddr, codeSize, oldProt, out _);


            _original = null;
        }

        /// <summary>
        /// Check function is hooked
        /// </summary>
        /// <returns>return true if function is hooked</returns>
        public bool IsHooked()
        {
            return _original != null;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualProtect(IntPtr address, uint size, uint newProtect, out uint oldProtect);
    }
}