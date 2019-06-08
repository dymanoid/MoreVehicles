// <copyright file="BuildConfigPatch.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace MoreVehicles
{
    using System;
    using System.Reflection;
    using SkyTools.Patching;

    /// <summary>
    /// A static class that provides the patch objects for the Build Config methods.
    /// </summary>
    internal static class BuildConfigPatch
    {
        /// <summary>Gets the patch for the 'SAVE_DATA_FORMAT_VERSION' property getter of the 'BuildConfig' class.</summary>
        public static IPatch SaveDataFormatVersion { get; } = new BuildConfig_SaveDataFormatVersion();

        private sealed class BuildConfig_SaveDataFormatVersion : PatchBase
        {
            protected override MethodInfo GetMethod()
            {
                return typeof(BuildConfig).GetMethod(
                    "get_" + nameof(BuildConfig.SAVE_DATA_FORMAT_VERSION),
                    BindingFlags.Static | BindingFlags.Public,
                    null,
                    new Type[0],
                    new ParameterModifier[0]);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Redundancy", "RCS1213", Justification = "Harmony patch")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming Rules", "SA1313", Justification = "Harmony patch")]
            private static bool Prefix(ref uint __result)
            {
                __result = 424242u;
                return false;
            }
        }
    }
}
