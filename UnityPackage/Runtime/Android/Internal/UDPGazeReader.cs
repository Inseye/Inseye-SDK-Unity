// Module name: com.inseye.unity.sdk
// File name: UDPGazeReader.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

#nullable enable
using Inseye.Internal;
using Inseye.Internal.Interfaces;

namespace Inseye.Android.Internal
{
    /// <summary>
    ///     Non-blocking UDP port data reader.
    /// </summary>
    internal sealed class UDPGazeReader : UdpReader<GazeData>, IEyeTrackerReader
    {
        public UDPGazeReader(int udpPort) : base(udpPort) { }

        public bool GetGazeData(ref GazeData gazeData)
        {
            return TryReadData(ref gazeData);
        }
    }
}