// Module name: com.inseye.unity.sdk
// File name: PinnedGazeDataMetadata.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Runtime.InteropServices;
using Inseye.Internal;

namespace Inseye.Android.Internal
{
    internal static class PinnedGazeDataMetadata
    {
        public static readonly int StructSize = Marshal.SizeOf<GazeData>();
    }
}