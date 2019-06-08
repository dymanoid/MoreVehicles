// <copyright file="MoreVehiclesMod.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace MoreVehicles
{
    using ICities;
    using SkyTools.Patching;
    using SkyTools.Tools;

    /// <summary>The main class of the More Vehicles mod.</summary>
    public sealed class MoreVehiclesMod : LoadingExtensionBase, IUserMod
    {
        private const string HarmonyId = "com.cities_skylines.dymanoid.morevehicles";
        ////private const long WorkshopId = 0;

        private readonly string modVersion = GitVersion.GetAssemblyVersion(typeof(MoreVehiclesMod).Assembly);
        private readonly bool isWorkshopMode = IsWorkshopMode();

        private MethodPatcher patcher;

        /// <summary>Gets the name of this mod.</summary>
        public string Name => "More Vehicles";

        /// <summary>Gets the description string of this mod.</summary>
        public string Description => "Increases the maximum allowed number of spawned vehicles. Version: " + modVersion;

        /// <summary>Called when this mod is enabled.</summary>
        public void OnEnabled()
        {
            if (!isWorkshopMode)
            {
                Log.Info($"The 'More Vehicles' mod version {modVersion} cannot be started because of no Steam Workshop");
                return;
            }

            Log.Info("The 'More Vehicles' mod has been enabled, version: " + modVersion);

            IPatch[] patches =
            {
                BuildConfigPatch.SaveDataFormatVersion,
                CinematicCameraControllerPatch.GetNearestVehicle,
                CinematicCameraControllerPatch.GetRandomVehicle,
                CinematicCameraControllerPatch.GetVehicleWithName,
                OutsideConnectionAIPatch.DummyTrafficProbability,
                PathVisualizerPatch.AddPathsImpl,
                ResidentAIPatch.DoRandomMove,
                TouristAIPatch.DoRandomMove,
                VehicleManagerPatch.DataDeserialize,
                VehicleManagerPatch.SimulationStepImpl,
                VehicleManagerPatch.UpdateData,
                VehiclePatch.GetTargetFrame,
            };

            patcher = new MethodPatcher(HarmonyId, patches);

            var patchedMethods = patcher.Apply();
            if (patchedMethods.Count == patches.Length)
            {
                VehicleManagerCustomizer.Customize();
            }
            else
            {
                Log.Error("The 'More Vehicles' mod failed to perform method redirections");
                patcher.Revert();
                patcher = null;
            }
        }

        /// <summary>Called when this mod is disabled.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Must be instance method due to C:S API")]
        public void OnDisabled()
        {
            if (!isWorkshopMode)
            {
                return;
            }

            VehicleManagerCustomizer.Revert();
            patcher?.Revert();
            patcher = null;

            Log.Info("The 'More Vehicles' mod has been disabled.");
        }

        private static bool IsWorkshopMode()
            => /*PluginManager.instance.GetPluginsInfo().Any(pi => pi.publishedFileID.AsUInt64 == WorkshopId)*/ true;
    }
}