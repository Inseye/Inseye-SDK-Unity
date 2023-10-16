// Module name: com.inseye.unity.sdk
// File name: InseyeSDKComponent.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace Inseye
{
    public enum InseyeSDKComponent
    {
        /// <summary>
        ///     Package imported to Unity project.
        /// </summary>
        UnitySDK,

        /// <summary>
        ///     Android service responsible for communication with eye tracker board and application sdks.
        /// </summary>
        AndroidService,

        /// <summary>
        ///     Code running on electronic board that operates photo sensors.
        /// </summary>
        BoardFirmware
    }
}