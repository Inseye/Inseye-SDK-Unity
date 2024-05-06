// Module name: com.inseye.unity.sdk
// File name: AndroidSettings.cs
// Last edit: 2024-5-6 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace Inseye.Android
{
    /// <summary>
    /// Additional settings that are used in android builds 
    /// </summary>
    public static class InseyeAndroidSettings
    {
        /// <summary>
        /// Maximum time the android library waits during service initialization.
        /// It should be only changed if sdk is used on non-standard (non-vr headset) device.
        /// </summary>
        public static int ServiceInitializationTimeoutMs { get; set; } = 2000;
    }
}