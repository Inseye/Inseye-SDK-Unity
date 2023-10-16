// Module name: com.inseye.unity.sdk.samples.calibration
// File name: InseyeCalibrationResult.cs
// Last edit: 2023-10-05 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace Inseye.Samples.Calibration
{
    /// <summary>
    ///     Results
    /// </summary>
    public enum InseyeCalibrationResult
    {
        /// <summary>
        ///     Calibration ended successfully.
        ///     User is properly calibrated.
        /// </summary>
        Successful,

        /// <summary>
        ///     Calibration was aborted.
        ///     User was not calibrated.
        /// </summary>
        CalibrationAborted,

        /// <summary>
        ///     Calibration failed to start.
        ///     User was not calibrated.
        /// </summary>
        FailedToStartCalibration,

        // internal error codes that may be result of miss-configured calibration scene or invalid usage
        /// <summary>
        ///     Calibration had already finished when attempt to calibrate was made.
        /// </summary>
        InvalidState,

        /// <summary>
        ///     Scene was miss-configured.
        /// </summary>
        MissingCalibrationGameObject,

        /// <summary>
        ///     Calibration procedure become null during calibration procedure.
        /// </summary>
        NullCalibrationProcedure
    }
}