// Module name: com.inseye.unity.sdk
// File name: InseyeGazeDataEnumerator.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Collections;
using Inseye.Internal;

namespace Inseye
{
    /// <summary>
    ///     Custom gaze data enumerator for data from current frame.
    /// </summary>
    public struct InseyeGazeDataEnumerator : System.Collections.Generic.IEnumerator<InseyeGazeData>
    {
        private GazeDataEnumerator _enumerator;

        internal InseyeGazeDataEnumerator(GazeDataEnumerator enumerator)
        {
            _enumerator = enumerator;
        }

        /// <summary>
        ///     Advances enumerator.
        /// </summary>
        /// <returns>True if data is available.</returns>
        public bool MoveNext()
        {
            return _enumerator.MoveNext();
        }

        /// <summary>
        ///     Moves enumerator back to beggining
        /// </summary>
        public void Reset()
        {
            _enumerator.Reset();
        }

        /// <summary>
        ///     Current gaze data sample.
        /// </summary>
        public InseyeGazeData Current => new(_enumerator.Current);

        /// <summary>
        ///     Current gaze data sample.
        /// </summary>
        object IEnumerator.Current => Current;

        /// <summary>
        ///     Disposes enumerator.
        /// </summary>
        public void Dispose()
        {
            _enumerator.Dispose();
        }
    }
}