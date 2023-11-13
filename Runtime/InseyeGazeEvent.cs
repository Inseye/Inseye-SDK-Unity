// Module name: com.inseye.unity.sdk
// File name: InseyeGazeEvent.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;

namespace Inseye
{
    /// <summary>
    ///     Events that are result of user gaze
    /// </summary>
    [Flags]
    public enum InseyeGazeEvent
    {
        /// <summary>
        ///     Nothing particular happened.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Left eye is closed or blinked.
        /// </summary>
        LeftBlinkOrClosed = 1 << 0,

        /// <summary>
        ///     Right eye is closed or blinked.
        /// </summary>
        RightBlinkOrClosed = 1 << 1,

        /// <summary>
        ///     Both eye are closed or both eye performed blink.
        /// </summary>
        BothBlinkOrClosed = 1 << 2,

        /// <summary>
        ///     Saccade occurred.
        /// </summary>
        Saccade = 1 << 3,

        /// <summary>
        ///     Headset was put on by the user.
        /// </summary>
        HeadsetMounted = 1 << 4,

        /// <summary>
        ///     Headset was put off by the user.
        /// </summary>
        HeadsetDismounted = 1 << 5,

        /// <summary>
        ///     Unknown event that was introduced in later version of service.
        /// </summary>
        Unknown = 1 << 6,

        /// <summary>
        ///     All events.
        /// </summary>
        All = LeftBlinkOrClosed | RightBlinkOrClosed | BothBlinkOrClosed | Saccade | HeadsetMounted |
              HeadsetDismounted | Unknown
    }
}