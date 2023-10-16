// Module name: com.inseye.unity.sdk
// File name: SDKException.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;

namespace Inseye.Exceptions
{
    /// <summary>
    ///     Base class for all exceptions thrown by Inseye SDK.
    /// </summary>
    public abstract class SDKException : Exception
    {
        internal SDKException()
        {
            SDKImplementationName = InseyeSDK.CurrentImplementation.GetType().FullName;
        }

        internal SDKException(string message) : base(message)
        {
            SDKImplementationName = InseyeSDK.CurrentImplementation.GetType().FullName;
        }

        internal SDKException(string message, Exception nestedException) : base(message, nestedException)
        {
            SDKImplementationName = InseyeSDK.CurrentImplementation.GetType().FullName;
        }

        public InseyeComponentVersion SDKVersion { get; } = InseyeSDK.SDKVersion;
        public string SDKImplementationName { get; }
    }
}