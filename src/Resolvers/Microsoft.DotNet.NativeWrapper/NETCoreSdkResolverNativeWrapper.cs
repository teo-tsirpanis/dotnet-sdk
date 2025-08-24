// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;

namespace Microsoft.DotNet.NativeWrapper
{
    public static class NETCoreSdkResolverNativeWrapper
    {
        public static SdkResolutionResult ResolveSdk(
            string? dotnetExeDirectory,
            string? globalJsonStartDirectory,
            bool disallowPrerelease = false)
        {
            var flags = disallowPrerelease ? Interop.hostfxr_resolve_sdk2_flags_t.disallow_prerelease : 0;

            int errorCode = Interop.hostfxr_resolve_sdk2(dotnetExeDirectory, globalJsonStartDirectory, flags, out SdkResolutionResult result);

            Debug.Assert((errorCode == 0) == (result.ResolvedSdkDirectory != null));
            return result;
        }

        public static string[]? GetAvailableSdks(string? dotnetExeDirectory)
        {
            int errorCode = Interop.hostfxr_get_available_sdks(dotnetExeDirectory, out string[]? list);

            return list ?? Array.Empty<string>();
        }
    }
}
