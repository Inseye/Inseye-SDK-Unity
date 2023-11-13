// Module name: com.inseye.unity.sdk
// File name: ErrorCodes.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace Inseye.Android.Internal
{
    internal enum ErrorCodes
    {
        // universal codes
        Successful = 0,
        UnknownErrorCheckErrorMessage = 1,
        SDKIsNotConnectedToService = 2,
        UnknownError = 3,

        // initialization error codes
        SDKAlreadyConnected = 10,
        FailedToBindToService = 11,
        InitializationTimeout = 12,

        // calibration error codes
        AnotherCalibrationIsOngoing = 20,
        NoCalibrationIsOngoing = 21,
        CalibrationTimeout = 22,

        // reading gaze data
        NoValidGazeAvailable = 30,

        //events
        AlreadySubscribedToEvents = 40
    }
}