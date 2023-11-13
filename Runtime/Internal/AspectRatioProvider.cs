// Module name: com.inseye.unity.sdk
// File name: AspectRatioProvider.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using UnityEngine;

namespace Inseye.Internal
{
    internal static class AspectRatioProvider
    {
        public static float GetAspectRatio()
        {
            return SystemInfo.deviceName switch
            {
                "G2 4K" => 1.0f,
                "Pico Neo 3 Pro" => 1832f / 1920f,
                "PICO 4" => 1.0f,
                _ => Screen.width / (float) Screen.height
            };
        }
    }
}