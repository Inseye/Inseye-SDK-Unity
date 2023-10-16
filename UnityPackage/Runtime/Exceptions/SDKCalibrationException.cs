// Module name: com.inseye.unity.sdk
// File name: SDKCalibrationException.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;

namespace Inseye.Exceptions
{
    /// <summary>
    ///     Exception thrown when SDK failed to start new calibration procedure
    /// </summary>
    [Serializable]
    public sealed class SDKCalibrationException : SDKException
    {
        internal SDKCalibrationException(string message) : base(message)
        {
        }

        internal SDKCalibrationException(string message, Exception nestedException) : base(message, nestedException)
        {
        }
    }
}