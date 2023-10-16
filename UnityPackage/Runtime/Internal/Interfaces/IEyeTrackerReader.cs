// Module name: com.inseye.unity.sdk
// File name: IEyeTrackerReader.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;

namespace Inseye.Internal.Interfaces
{
    internal interface IEyeTrackerReader : IDisposable
    {
        /// <summary>
        ///     Returns gaze data with oldest timestamp. Doesn't drain underlying queue/buffer/
        /// </summary>
        /// <param name="gazeData">gaze data struct to write</param>
        /// <returns>true fi gazeData was updated</returns>
        bool GetGazeData(ref GazeData gazeData);
    }
}