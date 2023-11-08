// Module name: com.inseye.unity.sdk
// File name: StateUserExtensions.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using Inseye.Internal.Interfaces;

namespace Inseye.Internal.Extensions
{
    internal static class StateUserExtensions
    {
        public static void AssertRequiredStateFlags(this IStateUser user, InseyeSDKState flags)
        {
            if ((InseyeSDK.InseyeSDKState & flags) != flags)
                throw new InvalidOperationException(
                    $"{user.GetType().Name} requires {flags:G} in it's state, which is {user.RequiredInseyeSDKState:G}");
        }
    }
}