﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.DotNet.Cli.Commands.MSBuild;
using Microsoft.DotNet.Cli.Utils;

namespace Microsoft.DotNet.Cli;

public static class NuGetSignatureVerificationEnabler
{
    private static readonly EnvironmentProvider s_environmentProvider = new();

    internal static readonly string DotNetNuGetSignatureVerification = "DOTNET_NUGET_SIGNATURE_VERIFICATION";

    public static void ConditionallyEnable(ForwardingApp forwardingApp, IEnvironmentProvider? environmentProvider = null)
    {
        ArgumentNullException.ThrowIfNull(forwardingApp, nameof(forwardingApp));

        if (!IsLinux())
        {
            return;
        }

        string value = GetSignatureVerificationEnablementValue(environmentProvider);

        forwardingApp.WithEnvironmentVariable(DotNetNuGetSignatureVerification, value);
    }

    public static void ConditionallyEnable(MSBuildForwardingApp forwardingApp, IEnvironmentProvider? environmentProvider = null)
    {
        ArgumentNullException.ThrowIfNull(forwardingApp, nameof(forwardingApp));

        if (!IsLinux())
        {
            return;
        }

        string value = GetSignatureVerificationEnablementValue(environmentProvider);

        forwardingApp.EnvironmentVariable(DotNetNuGetSignatureVerification, value);
    }

    private static string GetSignatureVerificationEnablementValue(IEnvironmentProvider? environmentProvider)
    {
        string? value = (environmentProvider ?? s_environmentProvider).GetEnvironmentVariable(DotNetNuGetSignatureVerification);

        return string.Equals(bool.FalseString, value, StringComparison.OrdinalIgnoreCase)
            ? bool.FalseString : bool.TrueString;
    }

    private static bool IsLinux()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }
}
