// Module name: com.inseye.unity.sdk
// File name: SDKInitializationException.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using Inseye.Android.Internal;

namespace Inseye.Exceptions
{
    /// <summary>
    ///     Exception thrown when SDK fails to perform initialization.
    /// </summary>
    [Serializable]
    public sealed class SDKInitializationException : SDKException
    {
        /// <summary>
        ///     A list of failed initialization reasons.
        /// </summary>
        public enum Reason
        {
            /// <summary>
            ///     Unknown.
            /// </summary>
            Unknown,

            /// <summary>
            ///     Unable to connect to the eye tracker service.
            /// </summary>
            UnableToConnectToService = 11,

            /// <summary>
            ///     Unable to connect to the eye tracker service in reasonable time.
            /// </summary>
            ConnectionTimeout = 12,

            /// <summary>
            ///     Invalid service version.
            /// </summary>
            InvalidServiceVersion,

            /// <summary>
            ///     Failed to initialize library used to connect to the eyetracker service.
            /// </summary>
            LibraryException
        }

        internal SDKInitializationException(string message, Reason reason = Reason.Unknown) : base(message)
        {
            FailReason = reason;
        }

        internal SDKInitializationException(string message, Exception nestedException) : this(message,
            Reason.Unknown, nestedException)
        {
        }

        internal SDKInitializationException(string message, Reason reason, Exception nestedException) : base(
            message,
            nestedException)
        {
            FailReason = reason;
        }

        /// <summary>
        ///     Initialization failure reason.
        /// </summary>
        public Reason FailReason { get; }

        internal static Reason ReasonFromErrorCode(ErrorCodes errorCode)
        {
            return errorCode switch
            {
                ErrorCodes.FailedToBindToService => Reason.UnableToConnectToService,
                ErrorCodes.InitializationTimeout => Reason.ConnectionTimeout,
                _ => Reason.Unknown
            };
        }
    }
}