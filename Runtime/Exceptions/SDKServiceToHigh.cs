// Module name: com.inseye.unity.sdk
// File name: SDKServiceToHigh.cs
// Last edit: 2024-07-23 11:33 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;

namespace Inseye.Exceptions
{
    /// <summary>
    ///     Exception thrown during initialization if eye tracker service version is too high to be properly handled by the
    ///     SDK.
    /// </summary>
    [Serializable]
    public class SDKServiceToHigh : SDKInitializationException
    {
        internal SDKServiceToHigh(string message, InseyeComponentVersion currentVersion,
            InseyeComponentVersion minimumVersion, InseyeComponentVersion maximumVersion) : base(message,
            Reason.InvalidServiceVersion)
        {
            CurrentVersion = currentVersion;
            MaximumVersion = maximumVersion;
        }

        /// <summary>
        ///     Maximum version of system service supported by this SDK.
        /// </summary>
        public InseyeComponentVersion MaximumVersion { get; }

        /// <summary>
        ///     Minimum version of system service required by this SDK.
        /// </summary>
        public InseyeComponentVersion MinimumVersion { get; }

        /// <summary>
        ///     Current system service version.
        /// </summary>
        public InseyeComponentVersion CurrentVersion { get; }
    }
}