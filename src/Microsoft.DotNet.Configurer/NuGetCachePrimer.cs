﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.EnvironmentAbstractions;

namespace Microsoft.DotNet.Configurer
{
    public class NuGetCachePrimer : INuGetCachePrimer
    {
        private readonly IFile _file;

        private readonly INuGetPackagesArchiver _nugetPackagesArchiver;

        private readonly INuGetCacheSentinel _nuGetCacheSentinel;

        private readonly INuGetConfig _nuGetConfig;

        public NuGetCachePrimer(
            INuGetPackagesArchiver nugetPackagesArchiver,
            INuGetCacheSentinel nuGetCacheSentinel,
            INuGetConfig nuGetConfig)
            : this(nugetPackagesArchiver,
                nuGetCacheSentinel,
                nuGetConfig,
                FileSystemWrapper.Default.File)
        {
        }

        internal NuGetCachePrimer(
            INuGetPackagesArchiver nugetPackagesArchiver,
            INuGetCacheSentinel nuGetCacheSentinel,
            INuGetConfig nuGetConfig,
            IFile file)
        {
            _nugetPackagesArchiver = nugetPackagesArchiver;

            _nuGetCacheSentinel = nuGetCacheSentinel;

            _nuGetConfig = nuGetConfig;

            _file = file;
        }

        public void PrimeCache()
        {
            if (SkipPrimingTheCache())
            {
                return;
            }

            var cliFallbackFolderPathCalculator = new CLIFallbackFolderPathCalculator();
            var nuGetFallbackFolder = cliFallbackFolderPathCalculator.CLIFallbackFolderPath;

            _nuGetConfig.AddCLIFallbackFolder(nuGetFallbackFolder);

            _nugetPackagesArchiver.ExtractArchive(nuGetFallbackFolder);

            _nuGetCacheSentinel.CreateIfNotExists();
        }

        private bool SkipPrimingTheCache()
        {
            return !_file.Exists(_nugetPackagesArchiver.NuGetPackagesArchive);
        }
    }
}
