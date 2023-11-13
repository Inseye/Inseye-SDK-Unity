// Module name: com.inseye.unity.sdk
// File name: IGazeDataSource.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;

namespace Inseye.Interfaces
{
    /// <summary>
    ///     Provides versioned gaze data
    /// </summary>
    public interface IGazeDataSource : IDisposable
    {
        /// <summary>
        ///     Returns span with all gaze data from last frame
        /// </summary>
        /// <returns></returns>
        ReadOnlySpan<InseyeGazeData> GetGazeDataFromLastFrame();

        /// <summary>
        ///     Returns enumerator that enumerates data from last frame
        /// </summary>
        /// <returns></returns>
        InseyeGazeDataEnumerator GetEnumerator();

        /// <summary>
        ///     Returns most recent gaze data.
        ///     Events are sum of all events from the last frame.
        /// </summary>
        /// <param name="gazeData">A struct where data will be written on successful read.</param>
        /// <returns>True if read was successful, otherwise false.</returns>
        bool GetMostRecentGazeData(out InseyeGazeData gazeData);
    }
}