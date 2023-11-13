// Module name: com.inseye.unity.sdk
// File name: InseyeStubSDK.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using System.Collections.Generic;
using Inseye.Exceptions;
using Inseye.Interfaces;
using Inseye.Internal.Interfaces;

namespace Inseye.Internal
{
    internal sealed class InseyeStubSDK : ISDKImplementation
    {
        private IGazeDataSource _inseyeIGazeDataSource;

        ISDKEventsBroker ISDKImplementation.EventBroker { get; } = new DefaultEventsBroker();

        IGazeDataSource ISDKImplementation.GazeDataSource => throw new NotImplementedException();
        public ISDKStateManager SDKStateManager { get; } = new SDKStateManager();

        public IDisposable KeepEyeTrackerInitialized()
        {
            throw new SDKInitializationException("PC SDK doesnt' change state");
        }

        IGazeProvider ISDKImplementation.GetGazeProvider()
        {
            throw new SDKInternalException("PC SDK doesn't implement gaze provider");
        }

        ICalibrationProcedure ISDKImplementation.StartCalibration()
        {
            throw new SDKCalibrationException("PC SDK doesn't implement calibration");
        }

        public InseyeEyeTrackerAvailability GetEyeTrackerAvailability()
        {
            return InseyeEyeTrackerAvailability.Unknown;
        }

        public IReadOnlyDictionary<InseyeSDKComponent, InseyeComponentVersion> GetVersions()
        {
            return new Dictionary<InseyeSDKComponent, InseyeComponentVersion>
            {
                {InseyeSDKComponent.UnitySDK, InseyeSDK.SDKVersion}
            };
        }

        public Eyes GetMostAccurateEye()
        {
            return Eyes.Both;
        }

        void IDisposable.Dispose()
        {
        }
    }
}