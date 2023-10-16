// Module name: com.inseye.unity.sdk
// File name: InseyeSDK.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

#if !UNITY_EDITOR && UNITY_ANDROID
#nullable enable
using Inseye.Android.Internal;
using Inseye.Interfaces;
using Inseye.Internal.Interfaces;

namespace Inseye
{
    public static partial class InseyeSDK
    {
        private static partial ISDKImplementation GetDefaultImplementation()
        {
            return new InseyeAndroidSKDImplementation();
        }
    }
}

#endif