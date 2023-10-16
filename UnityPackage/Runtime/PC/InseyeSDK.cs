// Module name: com.inseye.unity.sdk
// File name: InseyeSDK.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE
using Inseye.Internal;
using Inseye.Internal.Interfaces;

namespace Inseye
{
    /// <summary>
    ///     Eye tracker SDK for PC.
    ///     Currently has no implementation
    /// </summary>
    public static partial class InseyeSDK
    {
        private static partial ISDKImplementation GetDefaultImplementation()
        {
            return new InseyeStubSDK();
        }
    }
}
#endif