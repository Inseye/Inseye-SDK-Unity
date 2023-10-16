// Module name: com.inseye.unity.sdk
// File name: DefaultEventsBroker.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using Inseye.Internal.Interfaces;

namespace Inseye.Internal
{
    internal class DefaultEventsBroker : ISDKEventsBroker
    {
        public event Action<InseyeEyeTrackerAvailability> EyeTrackerAvailabilityChanged;
        public event Action<Eyes> MostAccurateEyeChanged;

        public void TransferListenersTo(ISDKEventsBroker target)
        {
            target.EyeTrackerAvailabilityChanged += EyeTrackerAvailabilityChanged;
            target.MostAccurateEyeChanged += MostAccurateEyeChanged;
            EyeTrackerAvailabilityChanged = null;
            MostAccurateEyeChanged = null;
        }

        public void InvokeEyeTrackerAvailabilityChanged(InseyeEyeTrackerAvailability availability)
        {
            EyeTrackerAvailabilityChanged?.Invoke(availability);
        }
    }
}