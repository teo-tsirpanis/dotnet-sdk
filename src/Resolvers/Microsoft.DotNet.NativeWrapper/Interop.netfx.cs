// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if !NETCOREAPP
using System.Runtime.CompilerServices;

namespace Microsoft.DotNet.NativeWrapper
{
    public static unsafe partial class Interop
    {
        public static readonly bool RunningOnWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        static Interop()
        {
            if (RunningOnWindows)
            {
                PreloadWindowsLibrary(Constants.HostFxr);
            }
        }

        // MSBuild SDK resolvers are required to be AnyCPU, but we have a native dependency and .NETFramework does not
        // have a built-in facility for dynamically loading user native dlls for the appropriate platform. We therefore 
        // preload the version with the correct architecture (from a corresponding sub-folder relative to us) on static
        // construction so that subsequent P/Invokes can find it.
        private static void PreloadWindowsLibrary(string dllFileName)
        {
            string? basePath = Path.GetDirectoryName(typeof(Interop).Assembly.Location);
            string architecture = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
            string dllPath = Path.Combine(basePath ?? string.Empty, architecture, $"{dllFileName}.dll");

            // return value is intentionally ignored as we let the subsequent P/Invokes fail naturally.
            LoadLibraryExW(dllPath, IntPtr.Zero, LOAD_WITH_ALTERED_SEARCH_PATH);
        }

        // lpFileName passed to LoadLibraryEx must be a full path.
        private const int LOAD_WITH_ALTERED_SEARCH_PATH = 0x8;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr LoadLibraryExW(string lpFileName, IntPtr hFile, int dwFlags);

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

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct hostfxr_dotnet_environment_info
        {
            public nuint size;
            public string hostfxr_version;
            public string hostfxr_commit_hash;
            public nuint sdk_count;
            public IntPtr sdks;
            public nuint framework_count;
            public IntPtr frameworks;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct hostfxr_dotnet_environment_framework_info
        {
            public nuint size;
            public string name;
            public string version;
            public string path;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct hostfxr_dotnet_environment_sdk_info
        {
            public nuint size;
            public string version;
            public string path;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        internal delegate void hostfxr_get_dotnet_environment_info_result_fn(
            IntPtr info,
            IntPtr result_context);

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
        internal static unsafe int hostfxr_get_dotnet_environment_info(
            string dotnet_root,
            out NetEnvironmentInfo result)
        {
            fixed (NetEnvironmentInfo* resultPtr = &result)
            {
                return hostfxr_get_dotnet_environment_info(dotnet_root, 0, resultCallback, resultPtr);
            }

            static void resultCallback(IntPtr infoPtr, IntPtr result_context)
            {
                var infoStruct = Marshal.PtrToStructure<hostfxr_dotnet_environment_info>(infoPtr);
                var runtimes = new hostfxr_dotnet_environment_framework_info[infoStruct.framework_count];
                for (var i = 0; i < (int)infoStruct.framework_count; i++)
                {
                    var ptr = IntPtr.Add(infoStruct.frameworks, i * Marshal.SizeOf<hostfxr_dotnet_environment_framework_info>());
                    runtimes[i] = Marshal.PtrToStructure<hostfxr_dotnet_environment_framework_info>(ptr);
                }
                var sdks = new hostfxr_dotnet_environment_sdk_info[infoStruct.sdk_count];
                for (var i = 0; i < (int)infoStruct.sdk_count; i++)
                {
                    var ptr = IntPtr.Add(infoStruct.sdks, i * Marshal.SizeOf<hostfxr_dotnet_environment_sdk_info>());
                    sdks[i] = Marshal.PtrToStructure<hostfxr_dotnet_environment_sdk_info>(ptr);
                }
                var runtimeInfos = runtimes.Select(r => new NetRuntimeInfo(r.name, r.version, r.path)).ToList();
                var sdkInfos = sdks.Select(s => new NetSdkInfo(s.version, s.path)).ToList();
                *(NetEnvironmentInfo*)result_context = new NetEnvironmentInfo(runtimeInfos, sdkInfos);
            }
        }
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

        [DllImport(Constants.HostFxr, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int hostfxr_get_dotnet_environment_info(
            string dotnet_root,
            nint reserved,
            hostfxr_get_dotnet_environment_info_result_fn result,
            void* result_context);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private delegate void hostfxr_resolve_sdk2_result_fn(
            hostfxr_resolve_sdk2_result_key_t key,
            string value);

        internal static int hostfxr_resolve_sdk2(
            string? exe_dir,
            string? working_dir,
            hostfxr_resolve_sdk2_flags_t flags,
            out SdkResolutionResult result)
        {
            result = new SdkResolutionResult();
            return hostfxr_resolve_sdk2(exe_dir, working_dir, flags, result.Initialize);
        }

        [DllImport(Constants.HostFxr, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int hostfxr_resolve_sdk2(
            string? exe_dir,
            string? working_dir,
            hostfxr_resolve_sdk2_flags_t flags,
            hostfxr_resolve_sdk2_result_fn result);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        internal delegate void hostfxr_get_available_sdks_result_fn(
            int sdk_count,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]
            string[] sdk_dirs);

        internal static int hostfxr_get_available_sdks(
            string? exe_dir,
            out string[]? result)
        {
            string[]? sdks = null;
            int hr = hostfxr_get_available_sdks(exe_dir, (count, dirs) => sdks = dirs);
            result = sdks;
            return hr;
        }

        [DllImport(Constants.HostFxr, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int hostfxr_get_available_sdks(
            string? exe_dir,
            hostfxr_get_available_sdks_result_fn result);
    }
}
#endif
