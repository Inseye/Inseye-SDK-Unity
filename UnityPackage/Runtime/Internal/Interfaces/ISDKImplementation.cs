// Module name: com.inseye.unity.sdk
// File name: ISDKImplementation.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using System.Collections.Generic;
using Inseye.Interfaces;

namespace Inseye.Internal.Interfaces
{
    /// <summary>
    ///     Common interface for all SDK implementations
    /// </summary>
    internal interface ISDKImplementation : IDisposable
    {
        ISDKEventsBroker EventBroker { get; }

        IGazeDataSource GazeDataSource { get; }

        ISDKStateManager SDKStateManager { get; }

        /// <inheritdoc cref="InseyeSDK.KeepEyeTrackerInitialized" />
        IDisposable KeepEyeTrackerInitialized();

        /// <summary>
        ///     Gaze provider factory
        /// </summary>
        /// <returns>new instance of gaze provider</returns>
        IGazeProvider GetGazeProvider();

        /// <summary>
        ///     Starts calibration procedure
        /// </summary>
        /// <returns>New calibration procedure.</returns>
        ICalibrationProcedure StartCalibration();

        /// <summary>
        ///     Reads eyetracker availability.
        /// </summary>
        /// <returns>Current availability.</returns>
        InseyeEyeTrackerAvailability GetEyeTrackerAvailability();

        /// <summary>
        ///     Returns dictionary with all components versions.
        ///     Contents of dictionary depends on operating system and currently available components.
        /// </summary>
        /// <returns>Dictionary mapping sdk component to its version.</returns>
        IReadOnlyDictionary<InseyeSDKComponent, InseyeComponentVersion> GetVersions();

        /// <summary>
        ///     Returns most accurate eye.
        /// </summary>
        /// <returns>Most accurate eye for a given moment.</returns>
        Eyes GetMostAccurateEye();

        [Obsolete(
            "Will be removed in SDK 5.0.0. Use GetEyeTrackerAvailability to get information if the eye tracker is calibrated.")]
        bool IsCalibrated();
    }
}