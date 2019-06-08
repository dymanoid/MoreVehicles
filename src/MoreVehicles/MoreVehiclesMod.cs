﻿// <copyright file="MoreVehiclesMod.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace MoreVehicles
{
    using System.Collections.Generic;
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
        public void OnDisabled()
        {
            if (!isWorkshopMode || patcher == null)
            {
                return;
            }

            VehicleManagerCustomizer.Revert();
            patcher.Revert();
            patcher = null;

            Log.Info("The 'More Vehicles' mod has been disabled.");
        }

        /// <summary>
        /// Performs mod registration when a game level is loaded.
        /// </summary>
        /// <param name="mode">The mode the game level is loaded in.</param>
        public override void OnLevelLoaded(LoadMode mode)
        {
            switch (mode)
            {
                case LoadMode.NewGame:
                case LoadMode.LoadGame:
                case LoadMode.LoadScenario:
                case LoadMode.NewGameFromScenario:
                    break;

                default:
                    return;
            }

            var gameMetadata = SimulationManager.instance.m_metaData;
            if (gameMetadata == null)
            {
                return;
            }

            lock (gameMetadata)
            {
                if (gameMetadata.m_modOverride == null)
                {
                    gameMetadata.m_modOverride = new Dictionary<string, bool>();
                }

                gameMetadata.m_modOverride[Constants.MetadataModName] = true;
            }
        }

        // TODO: activate the workshop check
        private static bool IsWorkshopMode()
            => /*PluginManager.instance.GetPluginsInfo().Any(pi => pi.publishedFileID.AsUInt64 == WorkshopId)*/ true;
    }
}