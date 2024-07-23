// Module name: com.inseye.unity.sdk
// File name: InseyeSDK.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

#nullable enable
using System;
using System.Collections.Generic;
using Inseye.Exceptions;
using Inseye.Interfaces;
using Inseye.Internal.Interfaces;

namespace Inseye
{
    /// <summary>
    ///     Provides access to Inseye Explore eye tracker
    /// </summary>
    public static partial class InseyeSDK
    {
        private static WeakReference<ICalibrationProcedure>? currentCalibrationProcedure;

        /// <summary>
        ///     SDK version.
        /// </summary>
        public static readonly InseyeComponentVersion SDKVersion = new(5, 2, 0);

        /// <summary>
        ///     Current SDK implementation.
        /// </summary>
        internal static ISDKImplementation CurrentImplementation { get; private set; } = GetDefaultImplementation();

        /// <summary>
        ///     Current internal SDK state.
        /// </summary>
        public static InseyeSDKState InseyeSDKState => CurrentImplementation.SDKStateManager.InseyeSDKState;

        /// <summary>
        ///     Platform dependent SDK implementation.
        /// </summary>
        /// <returns>Default implementation.</returns>
        private static partial ISDKImplementation GetDefaultImplementation();

        /// <summary>
        ///     Swaps current SDK implementation.
        /// </summary>
        /// <param name="sdkImplementation">
        ///     sdk implementation instance, default implementation for current compile target is
        ///     loaded if argument is null
        /// </param>
        /// <exception cref="System.InvalidOperationException">thrown when current sdk is in any state except Uninitialized.</exception>
        internal static void SwapSDKImplementation(ISDKImplementation? sdkImplementation = default)
        {
            if (sdkImplementation == CurrentImplementation)
                return;
            sdkImplementation ??= GetDefaultImplementation();
            var currentAvailability = InseyeEyeTrackerAvailability.Unavailable;
            try
            {
                currentAvailability = CurrentImplementation.GetEyeTrackerAvailability();
            }
            catch (Exception exception)
            {
#if DEBUG_INSEYE_SDK
                UnityEngine.Debug.LogError("Unable to get current sdk availablity during transfer.");
                UnityEngine.Debug.LogException(exception);
#endif
            }

            CurrentImplementation.EventBroker.TransferListenersTo(sdkImplementation.EventBroker);
            CurrentImplementation.SDKStateManager.TransferAllStateUsersTo(sdkImplementation.SDKStateManager);
            CurrentImplementation.Dispose();
            CurrentImplementation = sdkImplementation;
            var newAvailability = InseyeEyeTrackerAvailability.Unavailable;
            try
            {
                newAvailability = sdkImplementation.GetEyeTrackerAvailability();
            }
            catch (Exception exception)
            {
#if DEBUG_INSEYE_SDK
                UnityEngine.Debug.LogError("Unable to new sdk availablity during transfer.");
                UnityEngine.Debug.LogException(exception);
#endif
            }

            if (newAvailability != currentAvailability)
                sdkImplementation.EventBroker.InvokeEyeTrackerAvailabilityChanged(newAvailability);
        }

        /// <summary>
        ///     Keeps eye tracker initialized as long as returned object is not disposed.
        /// </summary>
        /// <returns>Object that keeps eye tracker initialized until disposed.</returns>
        /// <exception cref="Exceptions.SDKInitializationException">Exception thrown when SDK fails to perform initialization.</exception>
        public static IDisposable KeepEyeTrackerInitialized()
        {
            return CurrentImplementation.KeepEyeTrackerInitialized();
        }

        /// <summary>
        ///     Reads eye tracker availability.
        /// </summary>
        /// <returns>Current availability.</returns>
        /// <exception cref="Exceptions.SDKInitializationException">Exception thrown when SDK fails to perform initialization.</exception>
        public static InseyeEyeTrackerAvailability GetEyetrackerAvailability()
        {
            return CurrentImplementation.GetEyeTrackerAvailability();
        }

        /// <summary>
        ///     Reads eye tracker availability.
        ///     This method never throws.
        /// </summary>
        /// <param name="availability">Current availability if operation ended successfully.</param>
        /// <returns>True if availability was read successfully.</returns>
        public static bool TryGetEyeTrackerAvailability(out InseyeEyeTrackerAvailability availability)
        {
            // TODO: Add TryGetEyeTrackerAvailablility to ISDKImplementation interface
            availability = InseyeEyeTrackerAvailability.Unavailable;
            try
            {
                availability = CurrentImplementation.GetEyeTrackerAvailability();
                return true;
            }
            catch (SDKInitializationException)
            {
            }
            catch (Exception exception)
            {
#if DEBUG_INSEYE_SDK
                UnityEngine.Debug.LogException(exception);
#endif
            }

            return false;
        }

        /// <summary>
        ///     Returns object that can be pooled for gaze data.
        /// </summary>
        /// <returns>New instance of gaze provider.</returns>
        /// <exception cref="Exceptions.SDKInitializationException">Exception thrown when SDK fails to perform initialization.</exception>
        public static IGazeProvider GetGazeProvider()
        {
            return CurrentImplementation.GetGazeProvider();
        }

        /// <summary>
        ///     Starts calibration procedure.
        /// </summary>
        /// <returns>New calibration procedure.</returns>
        /// <exception cref="Exceptions.SDKInitializationException">Exception thrown when SDK fails to perform initialization.</exception>
        /// <exception cref="Exceptions.SDKCalibrationException">
        ///     Exception thrown when SDK failed to start new calibration
        ///     procedure.
        /// </exception>
        public static ICalibrationProcedure StartCalibration()
        {
            var calibration = CurrentImplementation.StartCalibration();
            currentCalibrationProcedure = new WeakReference<ICalibrationProcedure>(calibration);
            return calibration;
        }

        /// <summary>
        ///     Aborts currently running calibration, if there isn't any calibration in progress then nothing happens.
        /// </summary>
        public static void AbortCalibration()
        {
            if (currentCalibrationProcedure is not null && currentCalibrationProcedure.TryGetTarget(out var target))
                target.AbortCalibration();
        }

        /// <summary>
        ///     Returns dictionary with all components versions.
        ///     Contents of dictionary depends on operating system and currently available components.
        /// </summary>
        /// <returns>Dictionary mapping sdk component to its version.</returns>
        public static IReadOnlyDictionary<InseyeSDKComponent, InseyeComponentVersion> GetVersions()
        {
            try
            {
                return CurrentImplementation.GetVersions();
            }
            catch (SDKInitializationException)
            {
                return new Dictionary<InseyeSDKComponent, InseyeComponentVersion>
                {
                    {InseyeSDKComponent.UnitySDK, SDKVersion}
                };
            }
        }

        /// <summary>
        ///     Returns eye that provides most accurate gaze data based on
        /// </summary>
        /// <returns>Most accurate eye or both.</returns>
        public static Eyes GetMostAccurateEye()
        {
            return CurrentImplementation.GetMostAccurateEye();
        }


        /// <summary>
        ///     Event invoked when availability of eye tracking device changes.
        /// </summary>
        public static event Action<InseyeEyeTrackerAvailability> EyeTrackerAvailabilityChanged
        {
            add => CurrentImplementation.EventBroker.EyeTrackerAvailabilityChanged += value;
            remove => CurrentImplementation.EventBroker.EyeTrackerAvailabilityChanged -= value;
        }

        /// <summary>
        ///     Event invoked when most accurate eye changes.
        /// </summary>
        public static event Action<Eyes> MostAccurateEyeChanged
        {
            add => CurrentImplementation.EventBroker.MostAccurateEyeChanged += value;
            remove => CurrentImplementation.EventBroker.MostAccurateEyeChanged -= value;
        }
    }
}