// Module name: com.inseye.unity.sdk
// File name: ISDKEventsBroker.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;

namespace Inseye.Internal.Interfaces
{
    internal interface ISDKEventsBroker
    {
        /// <summary>
        ///     Event raised when eye tracker availability changes.
        /// </summary>
        event Action<InseyeEyeTrackerAvailability> EyeTrackerAvailabilityChanged;

        /// <summary>
        ///     Event raised when most accurate eye changes.
        /// </summary>
        event Action<Eyes> MostAccurateEyeChanged;

        /// <summary>
        ///     Moves listeners to new event broker.
        /// </summary>
        /// <param name="target">new event broker</param>
        void TransferListenersTo(ISDKEventsBroker target);

        void InvokeEyeTrackerAvailabilityChanged(InseyeEyeTrackerAvailability availability);
    }
}