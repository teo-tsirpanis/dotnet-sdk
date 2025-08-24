// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NETCOREAPP
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;

namespace Microsoft.DotNet.NativeWrapper
{
    internal static partial class Interop
    {
        private static readonly string HostFxrPath;

        static Interop()
        {
            string? hostfxrPath = (string)AppContext.GetData(Constants.RuntimeProperty.HostFxrPath)!;
            if (string.IsNullOrEmpty(hostfxrPath))
            {
                throw new HostFxrRuntimePropertyNotSetException();
            }
            HostFxrPath = hostfxrPath;
            NativeLibrary.SetDllImportResolver(typeof(Interop).Assembly, HostFxrResolver);
        }

        private static IntPtr HostFxrResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (libraryName != Constants.HostFxr)
            {
                return IntPtr.Zero;
            }

            if (!NativeLibrary.TryLoad(HostFxrPath, out var handle))
            {
                throw new HostFxrNotFoundException(HostFxrPath);
            }

            return handle;
        }

        [Flags]
        internal enum hostfxr_resolve_sdk2_flags_t : int
        {
            disallow_prerelease = 0x1,
        }

        internal enum hostfxr_resolve_sdk2_result_key_t : int
        {
            resolved_sdk_dir = 0,
            global_json_path = 1,
            requested_version = 2,
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct hostfxr_dotnet_environment_info
        {
            public nuint size;
            public IntPtr hostfxr_version;
            public IntPtr hostfxr_commit_hash;
            public nuint sdk_count;
            public hostfxr_dotnet_environment_sdk_info* sdks;
            public nuint framework_count;
            public hostfxr_dotnet_environment_framework_info* frameworks;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct hostfxr_dotnet_environment_framework_info
        {
            public nuint size;
            public IntPtr name;
            public IntPtr version;
            public IntPtr path;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct hostfxr_dotnet_environment_sdk_info
        {
            public nuint size;
            public IntPtr version;
            public IntPtr path;
        }

        private static NetRuntimeInfo GetRuntimeInfo(in hostfxr_dotnet_environment_framework_info info)
        {
            string name = Marshal.PtrToStringAuto(info.name) ?? string.Empty;
            string version = Marshal.PtrToStringAuto(info.version) ?? string.Empty;
            string path = Marshal.PtrToStringAuto(info.path) ?? string.Empty;
            return new NetRuntimeInfo(name, version, path);
        }

        private static NetSdkInfo GetSdkInfo(in hostfxr_dotnet_environment_sdk_info info)
        {
            string version = Marshal.PtrToStringAuto(info.version) ?? string.Empty;
            string path = Marshal.PtrToStringAuto(info.path) ?? string.Empty;
            return new NetSdkInfo(version, path);
        }

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
        internal static int hostfxr_get_dotnet_environment_info(
            string dotnet_root,
            out NetEnvironmentInfo result)
        {
            unsafe
            {
                fixed (NetEnvironmentInfo* resultPtr = &result)
                {
                    return hostfxr_get_dotnet_environment_info(dotnet_root, 0, &InvokeHostFxrGetDotnetEnvironmentInfoResult, resultPtr);
                }
            }

            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            static unsafe void InvokeHostFxrGetDotnetEnvironmentInfoResult(hostfxr_dotnet_environment_info* info, void* context)
            {
                NetRuntimeInfo[] runtimes = new NetRuntimeInfo[info->framework_count];
                for (nuint i = 0; i < info->framework_count; i++)
                {
                    runtimes[i] = GetRuntimeInfo(info->frameworks[i]);
                }
                NetSdkInfo[] sdks = new NetSdkInfo[info->sdk_count];
                for (nuint i = 0; i < info->sdk_count; i++)
                {
                    sdks[i] = GetSdkInfo(info->sdks[i]);
                }

                *(NetEnvironmentInfo*)context = new NetEnvironmentInfo(runtimes, sdks);
            }
        }
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

        [LibraryImport(Constants.HostFxr, StringMarshallingCustomType = typeof(AutoStringMarshaller))]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        private static unsafe partial int hostfxr_get_dotnet_environment_info(
            string dotnet_root,
            IntPtr reserved,
            delegate* unmanaged[Cdecl]<hostfxr_dotnet_environment_info*, void*, void> result,
            void* result_context);

        [ThreadStatic]
        private static SdkResolutionResult? t_resolve_sdk_result;

        internal static int hostfxr_resolve_sdk2(
            string? exe_dir,
            string? working_dir,
            hostfxr_resolve_sdk2_flags_t flags,
            out SdkResolutionResult result)
        {
            Debug.Assert(t_resolve_sdk_result == null);
            t_resolve_sdk_result = result = new SdkResolutionResult();
            try
            {
                unsafe
                {
                    return hostfxr_resolve_sdk2(exe_dir, working_dir, flags, &InvokeHostFxrResolveSdk2Result);
                }
            }
            finally
            {
                t_resolve_sdk_result = null;
            }

            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            static void InvokeHostFxrResolveSdk2Result(hostfxr_resolve_sdk2_result_key_t key, IntPtr value)
            {
                var result = t_resolve_sdk_result;
                Debug.Assert(result != null);
                string? valueStr = Marshal.PtrToStringAuto(value);
                result.Initialize(key, valueStr);
            }
        }

        [LibraryImport(Constants.HostFxr, StringMarshallingCustomType = typeof(AutoStringMarshaller))]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        private static unsafe partial int hostfxr_resolve_sdk2(
            string? exe_dir,
            string? working_dir,
            hostfxr_resolve_sdk2_flags_t flags,
            delegate* unmanaged[Cdecl]<hostfxr_resolve_sdk2_result_key_t, IntPtr, void> result);

        [ThreadStatic]
        private static string[]? t_get_available_sdks_result;

        internal static int hostfxr_get_available_sdks(
            string? exe_dir,
            out string[] result)
        {
            Debug.Assert(t_get_available_sdks_result == null);
            try
            {
                unsafe
                {
                    int errorCode = hostfxr_get_available_sdks(exe_dir, &InvokeHostFxrGetAvailableSdksResult);
                    result = t_get_available_sdks_result ?? Array.Empty<string>();
                    return errorCode;
                }
            }
            finally
            {
                t_get_available_sdks_result = null;
            }

            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            static unsafe void InvokeHostFxrGetAvailableSdksResult(int sdk_count, IntPtr* sdk_dirsPtr)
            {
                string[] sdk_dirs = new string[sdk_count];
                for (int i = 0; i < sdk_count; i++)
                {
                    sdk_dirs[i] = Marshal.PtrToStringAuto(sdk_dirsPtr[i]) ?? string.Empty;
                }
                t_get_available_sdks_result = sdk_dirs;
            }
        }

        [LibraryImport(Constants.HostFxr, StringMarshallingCustomType = typeof(AutoStringMarshaller))]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        private static unsafe partial int hostfxr_get_available_sdks(
            string? exe_dir,
            delegate* unmanaged[Cdecl]<int, IntPtr*, void> result);

        [CustomMarshaller(typeof(string), MarshalMode.Default, typeof(AutoStringMarshaller))]
        private static unsafe class AutoStringMarshaller
        {
            public static void* ConvertToUnmanaged(string? managed) => (void*)Marshal.StringToCoTaskMemAuto(managed);

            public static string? ConvertToManaged(void* unmanaged) => Marshal.PtrToStringAuto((IntPtr)unmanaged);

            public static void Free(void* unmanaged) => Marshal.FreeCoTaskMem((IntPtr)unmanaged);
        }
    }
}
#endif
