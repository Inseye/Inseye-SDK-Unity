// Module name: com.inseye.unity.sdk
// File name: IGazeProvider.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Collections.Generic;

namespace Inseye.Interfaces
{
    /// <summary>
    ///     Provides access to eye tracker data.
    /// </summary>
    public interface IGazeProvider : System.IDisposable, IEnumerable<InseyeGazeData>
    {
        /// <summary>
        ///     Pools for most recent gaze position. If new gaze position is available writes it to struct reference.
        ///     All gaze events that occured between call to this method in current frame and previous
        /// </summary>
        /// <param name="inseyeGazePosition">Output gaze position.</param>
        /// <returns>True if gazePosition was updated.</returns>
        bool GetMostRecentGazeData(out InseyeGazeData inseyeGazePosition);

        /// <summary>
        ///     Returns a span with all gaze data from last frame.
        /// </summary>
        /// <returns>
        ///     Span of data buffer. Span lifetime is a single frame. Reading from the span after that may lead to undefined
        ///     behaviour.
        /// </returns>
        System.ReadOnlySpan<InseyeGazeData> GetGazeDataFromLastFrame();

        /// <summary>
        ///     Returns gaze data enumerator.
        /// </summary>
        /// <returns>Enumerator that is valid for a single frame.</returns>
        public new InseyeGazeDataEnumerator GetEnumerator();
    }
}