// Module name: com.inseye.unity.sdk
// File name: GazeDataExtesnions.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

namespace Inseye.Internal.Extensions
{
    internal static class GazeDataExtensions
    {
        public static InseyeGazeEvent ConvertEvent(this GazeData gazeData)
        {
            if (gazeData.event_ == 0)
                return InseyeGazeEvent.None;
            return (InseyeGazeEvent) (1 << (gazeData.event_ - 1));
        }
    }
}