// Module name: com.inseye.unity.sdk
// File name: InseyeCalibrationState.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace Inseye
{
    /// <summary>
    ///     Calibration state
    /// </summary>
    public enum InseyeCalibrationState
    {
        /// <summary>
        ///     Calibration was not started yet.
        /// </summary>
        NotStarted,

        /// <summary>
        ///     Calibration was started but has not finished yet.
        /// </summary>
        Ongoing,

        /// <summary>
        ///     Calibration was finished and user is correctly calibrated.
        /// </summary>
        FinishedSuccessfully,

        /// <summary>
        ///     Calibration was finished but user is not calibrated.
        /// </summary>
        FinishedFailed
    }
}