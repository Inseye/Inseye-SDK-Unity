// Module name: com.inseye.unity.sdk
// File name: EmptyInseyeIGazeDataSource.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using Inseye.Interfaces;

namespace Inseye.Internal
{
    internal class EmptyInseyeIGazeDataSource : IGazeDataSource, IVersionedSpan<GazeData>
    {
        public static EmptyInseyeIGazeDataSource Instance = new();

        private EmptyInseyeIGazeDataSource()
        {
        }

        public ReadOnlySpan<InseyeGazeData> GetGazeDataFromLastFrame()
        {
            return ReadOnlySpan<InseyeGazeData>.Empty;
        }

        public InseyeGazeDataEnumerator GetEnumerator()
        {
            return new InseyeGazeDataEnumerator(new GazeDataEnumerator(Version, this));
        }

        public bool GetMostRecentGazeData(out InseyeGazeData gazeData)
        {
            gazeData = default;
            return false;
        }

        public void Dispose()
        {
        }

        public int Version => -1;
        public ReadOnlySpan<GazeData> Array => default;

        public ReadOnlySpan<GazeData> GetRawGazeDataFromLastFrame()
        {
            return ReadOnlySpan<GazeData>.Empty;
        }
    }
}