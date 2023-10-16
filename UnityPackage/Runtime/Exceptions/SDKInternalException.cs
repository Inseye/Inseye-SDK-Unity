// Module name: com.inseye.unity.sdk
// File name: SDKInternalException.cs
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
    ///     Exception that should never occur. If something throws that it means that some developer made implementation
    ///     mistakes/not all corner cases are covered properly.
    /// </summary>
    [Serializable]
    public sealed class SDKInternalException : SDKException
    {
        internal SDKInternalException(string message) : base(message)
        {
        }

        internal SDKInternalException(string message, Exception baseException) : base(message, baseException)
        {
        }
    }
}