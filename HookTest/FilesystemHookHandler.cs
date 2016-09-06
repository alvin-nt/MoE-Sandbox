using System;
using System.IO;
using System.Runtime.InteropServices;
using EasyHook;

namespace HookTest
{
    public static class FilesystemHookHandler
    {
        public static IntPtr CreateFileA(string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile)
        {
            try
            {
                var callback = (Main) HookRuntimeInfo.Callback;
                var result = NativeApi.CreateFileA(filename, access, share, securityAttributes, creationDisposition,
                    flagsAndAttributes, templateFile);

                callback.Interface.Log($"[CreateFileA] path={filename}, " +
                                       $"access={access}, " +
                                       $"share={share}, " +
                                       $"mode={creationDisposition}, " +
                                       $"attributes={flagsAndAttributes}, " +
                                       $"result={result}");
                return result;
            }
            catch // Cannot communicate with the interface. just run the function as usual.
            {
                return NativeApi.CreateFileA(filename, access, share, securityAttributes, creationDisposition,
                    flagsAndAttributes, templateFile);
            }
        }

        public static IntPtr CreateFileW(string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile)
        {
            try
            {
                var callback = (Main)HookRuntimeInfo.Callback;
                var result = NativeApi.CreateFileW(filename, access, share, securityAttributes, creationDisposition,
                    flagsAndAttributes, templateFile);

                callback.Interface.Log($"[CreateFileW] path={filename}, " +
                                       $"access={access}, " +
                                       $"share={share}, " +
                                       $"mode={creationDisposition}, " +
                                       $"attributes={flagsAndAttributes}, " +
                                       $"result={result}");
                return result;
            }
            catch // Cannot communicate with the interface. just run the function as usual.
            {
                return NativeApi.CreateFileW(filename, access, share, securityAttributes, creationDisposition,
                    flagsAndAttributes, templateFile);
            }
        }

        public static bool CreateDirectoryA(string filename, IntPtr securityAttributes)
        {
            try
            {
                var callback = (Main) HookRuntimeInfo.Callback;
                var result = NativeApi.CreateDirectoryA(filename, securityAttributes);

                callback.Interface.Log($"[CreateDirectoryA] path={filename}, result={result}");
                return result;
            }
            catch (Exception)
            {
                return NativeApi.CreateDirectoryA(filename, securityAttributes);
            }
        }

        public static bool CreateDirectoryW(string filename, IntPtr securityAttributes)
        {
            try
            {
                var callback = (Main) HookRuntimeInfo.Callback;
                var result = NativeApi.CreateDirectoryW(filename, securityAttributes);

                callback.Interface.Log($"[CreateDirectoryW] path={filename}, result={result}");
                return result;
            }
            catch (Exception)
            {
                return NativeApi.CreateDirectoryW(filename, securityAttributes);
            }
        }

        public static bool DeleteFileA(string filename)
        {
            try
            {
                var callback = (Main) HookRuntimeInfo.Callback;
                var result = NativeApi.DeleteFileA(filename);

                callback.Interface.Log($"[DeleteFileA] path={filename}, result={result}");
                return result;
            }
            catch (Exception)
            {
                return NativeApi.DeleteFileA(filename);
            }
        }

        public static bool DeleteFileW(string filename)
        {
            try
            {
                var callback = (Main) HookRuntimeInfo.Callback;
                var result = NativeApi.DeleteFileW(filename);

                callback.Interface.Log($"[DeleteFileW] path={filename}, result={result}");
                return result;
            }
            catch (Exception)
            {
                return NativeApi.DeleteFileW(filename);
            }
        }

        public static IntPtr LoadLibraryExA(string filename, IntPtr reserved, uint flags)
        {
            try
            {
                var callback = (Main)HookRuntimeInfo.Callback;
                var result = NativeApi.LoadLibraryExA(filename, reserved, flags);

                callback.Interface.Log($"[LoadLibraryExA] path={filename}, flags={flags:X8}, result={result}");
                return result;
            }
            catch (Exception)
            {
                return NativeApi.LoadLibraryExA(filename, reserved, flags);
            }
        }

        public static IntPtr LoadLibraryExW(string filename, IntPtr reserved, uint flags)
        {
            try
            {
                var callback = (Main)HookRuntimeInfo.Callback;
                var result = NativeApi.LoadLibraryExW(filename, reserved, flags);

                callback.Interface.Log($"[LoadLibraryExW] path={filename}, flags={flags:X8}, result={result}");
                return result;
            }
            catch (Exception)
            {
                return NativeApi.LoadLibraryExW(filename, reserved, flags);
            }
        }

        public static bool RemoveDirectoryA(string filename)
        {
            try
            {
                var callback = (Main)HookRuntimeInfo.Callback;
                var result = NativeApi.RemoveDirectoryA(filename);

                callback.Interface.Log($"[RemoveDirectoryA] path={filename}, result={result}");
                return result;
            }
            catch (Exception)
            {
                return NativeApi.RemoveDirectoryA(filename);
            }
        }

        public static bool RemoveDirectoryW(string filename)
        {
            try
            {
                var callback = (Main)HookRuntimeInfo.Callback;
                var result = NativeApi.RemoveDirectoryW(filename);

                callback.Interface.Log($"[RemoveDirectoryW] path={filename}, result={result}");
                return result;
            }
            catch (Exception)
            {
                return NativeApi.RemoveDirectoryW(filename);
            }
        }
    }
}