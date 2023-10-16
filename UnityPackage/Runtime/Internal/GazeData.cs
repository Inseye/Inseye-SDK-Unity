// Module name: com.inseye.unity.sdk
// File name: GazeData.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Runtime.InteropServices;

namespace Inseye.Internal
{
    /// <summary>
    ///     Layout of this struct must be that same as java serializer in
    ///     com.inseye.shared.communication.GazeData
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct GazeData
    {
        public long timeMilliseconds;
        public float left_x;
        public float left_y;
        public float right_x;
        public float right_y;
        public int event_;

        public override string ToString()
        {
            return
                $"ts: {timeMilliseconds}, lx: {left_x}, ly: {left_y}, rx: {right_x}, ry: {right_y}, {event_}";
        }
    }
}