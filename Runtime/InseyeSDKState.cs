// Module name: com.inseye.unity.sdk
// File name: InseyeSDKState.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;

namespace Inseye
{
    /// <summary>
    ///     Internal SDK state description.
    /// </summary>
    [Flags]
    public enum InseyeSDKState
    {
        /// <summary>
        ///     Initialized SDK, it's safe to swap SDK implementation in this state.
        /// </summary>
        Uninitialized = 0,

        /// <summary>
        ///     SDK is initialized - e.g. connected to eye tracker server.
        /// </summary>
        Initialized = 1 << 0,

        /// <summary>
        ///     SDK is calibrating eye tracker.
        /// </summary>
        Calibration = 1 << 1,

        /// <summary>
        ///     SDK is able to obtain gaze data from eye tracker.
        /// </summary>
        MostRecentGazePointAvailable = 1 << 2,

        /// <summary>
        ///     SDK is subscribed to eye tracker server events.
        /// </summary>
        SubscribedToEyeTrackerEvents = 1 << 3
    }
}