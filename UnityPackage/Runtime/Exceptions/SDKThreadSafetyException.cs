// Module name: com.inseye.unity.sdk
// File name: SDKThreadSafetyException.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using System.Threading;
using Inseye.Internal;

namespace Inseye.Exceptions
{
    /// <summary>
    ///     Thrown when method is invoked on invalid thread.
    /// </summary>
    [Serializable]
    public sealed class SDKThreadSafetyException : SDKException
    {
        private readonly int acctualThreadId;
        private readonly int expectedThreadId;

        internal SDKThreadSafetyException(int expectedThreadId) : this(expectedThreadId, string.Empty, null)
        {
        }

        internal SDKThreadSafetyException(int expectedThreadId, string message) : this(expectedThreadId, message, null)
        {
        }

        internal SDKThreadSafetyException(int expectedThreadId, string message, Exception nestedException) : base(
            message,
            nestedException)
        {
            this.expectedThreadId = expectedThreadId;
            acctualThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public static SDKThreadSafetyException NotOnMainThread()
        {
            return new SDKThreadSafetyException(MainThreadGuard.MainThreadId, "Must be invoked on Unity main thread.");
        }
    }
}