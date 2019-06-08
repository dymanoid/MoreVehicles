// <copyright file="Compatibility.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace MoreVehicles
{
    using System.Collections.Generic;
    using System.Linq;
    using ColossalFramework.Plugins;
    using static MoreVehicles.WorkshopMods;

    /// <summary>
    /// An utility class for checking the compatibility with other installed mods.
    /// </summary>
    internal sealed class Compatibility
    {
        private static readonly ulong[] IncompatibleModIds =
        {
            TrafficManagerPresidentEditionStable,
            TrafficManagerPresidentEditionLabs,
        };

        private readonly Dictionary<ulong, PluginManager.PluginInfo> activeMods = new Dictionary<ulong, PluginManager.PluginInfo>();

        private Compatibility()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="Compatibility"/> class.</summary>
        /// <returns>A new and initialized instance of the <see cref="Compatibility"/> class.</returns>
        public static Compatibility Create()
        {
            var result = new Compatibility();
            result.Initialize();
            return result;
        }

        /// <summary>Checks for enabled incompatible mods and prepares a notification message text if any found.</summary>
        /// <returns><c>true</c> if there are any active incompatible mod detected; otherwise, <c>false</c>.</returns>
        public bool AreAnyIncompatibleModsActive() => IncompatibleModIds.Any(id => activeMods.ContainsKey(id));

        private void Initialize()
        {
            activeMods.Clear();
            foreach (var plugin in PluginManager.instance.GetPluginsInfo().Where(m => m.isEnabled))
            {
                activeMods[plugin.publishedFileID.AsUInt64] = plugin;
            }
        }
    }
}
