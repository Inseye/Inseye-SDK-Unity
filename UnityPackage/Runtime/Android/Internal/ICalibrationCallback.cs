// Module name: com.inseye.unity.sdk
// File name: ICalibrationCallback.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace Inseye.Android.Internal
{
    /// <summary>
    ///     Android callback interface to communicate intents with service
    /// </summary>
    internal interface ICalibrationCallback
    {
        /// <inheritdoc cref="Interfaces.ICalibrationProcedure.ReportReadyToDisplayPoints" />
        public void ReportReadyToDisplayPoints();

        public void AbortCalibration();
    }
}