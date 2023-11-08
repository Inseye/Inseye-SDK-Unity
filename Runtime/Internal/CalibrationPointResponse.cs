// Module name: com.inseye.unity.sdk
// File name: CalibrationPointResponse.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using System.Runtime.InteropServices;

namespace Inseye.Internal
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct CalibrationPointResponse
    {
        public float x;
        public float y;
        public long displayStartMs;

        internal CalibrationPointResponse(float x, float y)
        {
            this.x = x;
            this.y = y;
            displayStartMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}