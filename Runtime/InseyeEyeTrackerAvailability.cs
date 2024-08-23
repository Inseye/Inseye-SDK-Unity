// Module name: com.inseye.unity.sdk
// File name: InseyeEyeTrackerAvailability.cs
// Last edit: 2024-07-23 11:18 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace Inseye
{
    /// <summary>
    ///     State of hardware eye tracker
    ///     Int values of this enum are not meant to be part of API and are subjected to change on demand without SDK version major value change.
    /// </summary>
    public enum InseyeEyeTrackerAvailability
    {
        // values from service API
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
        ///     Eye tracker is connected to headset but is not yet available.
        /// </summary>
        Unavailable = 5,

        /// <summary>
        ///     Eye tracker is connected but not calibrated and gaze data is not available.
        ///     Gaze data can be provided after calibration procedure.
        /// </summary>
        NotCalibrated = 6,

        /// <summary>
        ///     The eyetracker is connected but unavailable for unknown reason.
        ///     This flag should only appear if client library is behind service library and new flags were added.
        /// </summary>
        Unknown = 7,

        /// <summary>
        ///     The SDK is unable to connect to eye tracker service.
        ///     Service may not be installed on host machine or current service
        ///     API implementation is not compatible with SDK (breaking changes introduced in API).  
        /// </summary>
        /// 
        UnableToConnectToInseyeService = 8,

        /// <summary>
        ///     Service version is incompatible with SDK version.
        /// </summary>
        InvalidServiceVersion = 9
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
            return availability is InseyeEyeTrackerAvailability.Available or InseyeEyeTrackerAvailability.Unavailable
                or InseyeEyeTrackerAvailability.NotCalibrated or InseyeEyeTrackerAvailability.Calibrating
                or InseyeEyeTrackerAvailability.Unknown;
        }
    }
}