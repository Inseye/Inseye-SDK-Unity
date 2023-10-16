﻿// Module name: com.inseye.unity.sdk
// File name: InseyeEyeTrackerAvailability.cs
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
    ///     State of hardware eye tracker
    /// </summary>
    public enum InseyeEyeTrackerAvailability
    {
        /// <summary>
        ///     The eye tracker is fully functional - gaze data can be provided, a new calibration can be started.
        /// </summary>
        Available = 0,

        /// <summary>
        ///     The eye tracker is physically disconnected from the headset.
        /// </summary>
        Disconnected = 1,

        /// <summary>
        ///     The eye tracker is connected but cannot provide gaze data because calibration is in progress.
        /// </summary>
        Calibrating = 2,

        /// <summary>
        ///     The eye tracker is connected to headset but cannot provide gaze data because raw data is read.
        /// </summary>
        [Obsolete("Will be removed in sdk version 5.0.0. Use Unavailable instead.")]
        RawData = 3,

        /// <summary>
        ///     The eye tracker is connected to headset but is unavailable because eyetracker board firmware is being
        ///     updated.
        /// </summary>
        [Obsolete("Will be removed in sdk version 5.0.0. Use Unavailable instead.")]
        FirmwareUpdate = 4,

        /// <summary>
        ///     The eye tracker is connected to but not yet available.
        /// </summary>
        [Obsolete("Will be removed in sdk version 5.0.0. Use Unavailable instead.")]
        Connected = 5,

        /// <summary>
        ///     Eye tracker is connected but is not yet available.
        /// </summary>
        Unavailable = 5,

        /// <summary>
        ///     Eye tracker is connected but not calibrated and gaze data is not available.
        ///     Gaze data can be provided after calibration procedure.
        /// </summary>
        NotCalibrated = 6,

        /// <summary>
        ///     The eyetracker is connected but unavailable for unknown reason.
        ///     This flag should should only appear if client library is behind service library and new flags were added.
        /// </summary>
        Unknown = 7
    }

    /// <summary>
    ///     Extension methods for <see cref="InseyeEyeTrackerAvailability" />
    /// </summary>
    public static class InseyeEyeTrackerAvailabilityExtensions
    {
        /// <summary>
        ///     Provides information about eye tracker being physically connected to device.
        /// </summary>
        /// <param name="availability">Eye tracker availability.</param>
        /// <returns>True if eye tracker is connected to vr headset.</returns>
        public static bool IsConnected(this InseyeEyeTrackerAvailability availability)
        {
            return availability != InseyeEyeTrackerAvailability.Disconnected;
        }
    }
}