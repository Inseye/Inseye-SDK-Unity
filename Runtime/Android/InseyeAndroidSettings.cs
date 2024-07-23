// Module name: com.inseye.unity.sdk
// File name: InseyeAndroidSettings.cs
// Last edit: 2024-07-23 11:18 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace Inseye.Android
{
    /// <summary>
    ///     Additional settings that are used in android builds
    /// </summary>
    public static class InseyeAndroidSettings
    {
        /// <summary>
        ///     Minimum Android service version required to work with this SDK.
        /// </summary>
        public static readonly InseyeComponentVersion MinimumServiceVersion = new(0, 13, 0);

        /// <summary>
        ///     Maximum Android service version that this Unity SDK supports.
        /// </summary>
        public static readonly InseyeComponentVersion MaximumServiceVersion = new(1, 0, 0);

        /// <summary>
        ///     Maximum time the android library waits during service initialization.
        ///     It should be only changed if sdk is used on non-standard (non-vr headset) device.
        /// </summary>
        public static int ServiceInitializationTimeoutMs { get; set; } = 2000;
    }
}