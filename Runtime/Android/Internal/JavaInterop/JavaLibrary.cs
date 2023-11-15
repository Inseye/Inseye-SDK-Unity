// Module name: com.inseye.unity.sdk
// File name: JavaLibrary.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using Inseye.Exceptions;
using UnityEngine;

namespace Inseye.Android.Internal.JavaInterop
{
    // TODO: Change AndroidCalibrationProcedure after refactor. 
    internal class JavaLibrary
    {
        /// <summary>
        ///     True when _javaClass is not null and not disposed
        /// </summary>
        private bool _initialized;

        private AndroidJavaClass _javaClass;
        public enum LogLevel
        {
            Debug = 3,
            Error = 6,
            Info = 4,
            Verbose = 2,
            Warn = 5
        }
        
        public ErrorCodes Initialize(IntPtr statePointer, long timeout)
        {
            ThrowIfNotOnMainThread();
            try
            {
                MaybeInitializeClass();
                return (ErrorCodes) _javaClass.CallStatic<int>("initialize", (long) statePointer, timeout);
            }
            catch (AndroidJavaException androidJavaException)
            {
                throw new SDKInitializationException("Failed to initialize SDK java library",
                    SDKInitializationException.Reason.LibraryException,
                    androidJavaException);
            }
            catch (Exception exception)
            {
                throw new SDKInitializationException("Unchecked exception at Android Java class initialization",
                    exception);
            }
        }

        public void SetLogLevel(LogLevel level)
        {
            MaybeInitializeClass();
            _javaClass.CallStatic("setLoggingLevel", (int) level);
        }
        
        /// <summary>
        /// Attempts to start calibration procedure. 
        /// </summary>
        /// <param name="calibrationPointRequestPointer">Pointer to CalibrationPointRequest structure.</param>
        /// <param name="calibrationPointResponsePointer">Pointer to CalibrationPointResponse structure.</param>
        /// <param name="calibrationStatusPointer">Pointer to CalibrationStatus enum.</param>
        /// <param name="pointIndexPointer">Pointer to current calibration point index int.</param>
        /// <returns></returns>
        /// <exception cref="UnityEngine.AndroidJavaException">Exception thrown by java library.</exception>
        public JavaCalibrationProcedureProxy StartCalibrationProcedure(IntPtr calibrationPointRequestPointer,
            IntPtr calibrationPointResponsePointer, IntPtr calibrationStatusPointer, IntPtr pointIndexPointer)
        {
            ThrowIfNotInitialized();
            ThrowIfNotOnMainThread();
            return new JavaCalibrationProcedureProxy(_javaClass.CallStatic<AndroidJavaObject>(
                "startCalibrationProcedure",
                (long) calibrationPointRequestPointer, (long) calibrationPointResponsePointer,
                (long) calibrationStatusPointer, (long) pointIndexPointer));
        }

        public ErrorCodes SubscribeToEvents(string listenerGameObjectName)
        {
            ThrowIfNotInitialized();
            ThrowIfNotOnMainThread();
            return (ErrorCodes) _javaClass.CallStatic<int>("subscribeToEvents", listenerGameObjectName);
        }

        public InseyeEyeTrackerAvailability GetEyeTrackerAvailability()
        {
            ThrowIfNotInitialized();
            ThrowIfNotOnMainThread();
            return (InseyeEyeTrackerAvailability) _javaClass.CallStatic<int>("getEyeTrackerAvailability");
        }

        public ErrorCodes UnsubscribeFromEvents()
        {
            ThrowIfNotInitialized();
            ThrowIfNotOnMainThread();
            return (ErrorCodes) _javaClass.CallStatic<int>("unsubscribeFromEvents");
        }

        public ErrorCodes GetGazeDataStreamUDPPort(out int port)
        {
            ThrowIfNotInitialized();
            ThrowIfNotOnMainThread();
            var tempInt = -1;
            ErrorCodes errorCode;
            unsafe
            {
                errorCode = (ErrorCodes) _javaClass.CallStatic<int>("getEyeTrackingDataStreamPort",
                    (long) &tempInt);
            }

            port = tempInt;
            return errorCode;
        }

        public ErrorCodes StopGazeDataStream()
        {
            ThrowIfNotInitialized();
            ThrowIfNotOnMainThread();
            return (ErrorCodes) _javaClass.CallStatic<int>("stopEyeTrackingDataStream");
        }

        public (InseyeComponentVersion serviceVersion, InseyeComponentVersion firmwareVersion) GetVersions()
        {
            ThrowIfNotInitialized();
            ThrowIfNotOnMainThread();
            var stringSerialized = _javaClass.CallStatic<string>("getVersions");
            var separatorIndex = stringSerialized.IndexOf('\n');
            if (separatorIndex == -1)
                throw new SDKInternalException(
                    "Failed to find version separator index in string returned from Java code.");
            var asSpan = stringSerialized.AsSpan();
            return (InseyeComponentVersion.Parse(asSpan[..separatorIndex]),
                InseyeComponentVersion.Parse(asSpan[(separatorIndex + 1)..]));
        }

        public string GetLastErrorMessage()
        {
            ThrowIfNotInitialized();
            ThrowIfNotOnMainThread();
            return _javaClass.CallStatic<string>("getLastErrorMessage");
        }

        public Eyes GetDominantEyes()
        {
            ThrowIfNotInitialized();
            ThrowIfNotOnMainThread();
            return (Eyes) _javaClass.CallStatic<int>("getDominantEye");
        }

        public void Dispose(IntPtr statePointer)
        {
            if (!_initialized)
                return;
            ThrowIfNotOnMainThread();
            try
            {
                var returnCode = (ErrorCodes) _javaClass.CallStatic<int>("dispose");
                switch (returnCode)
                {
                    case ErrorCodes.Successful:
                        break;
                    case ErrorCodes.UnknownErrorCheckErrorMessage:
                        Debug.LogError($"Error occured while disposing JavaSDK: {GetLastErrorMessage()}");
                        break;
                    default:
                        Debug.LogError($"Unexpected error code while disposing SDK: {returnCode:G}");
                        break;
                }
            }
            catch (AndroidJavaException androidJavaException)
            {
                throw new SDKInitializationException(
                    $"Unchecked android exception during disposing: {androidJavaException.Message}");
            }
            finally
            {
                _javaClass.Dispose();
                _javaClass = null;
                _initialized = false;
            }
        }

        private void MaybeInitializeClass()
        {
            if (!_initialized)
                _javaClass = new AndroidJavaClass("com.inseye.unitysdk.UnitySDK");
            _initialized = true;
        }

        private void ThrowIfNotInitialized()
        {
            if (!_initialized)
                throw new SDKInternalException("JavaLibrary is disposed but something is trying to call it's methods");
        }

        private void ThrowIfNotOnMainThread()
        {
#if DEVELOPMENT_BUILD || DEBUG_INSEYE_SDK
            if (!Inseye.Internal.MainThreadGuard.IsOnMainThread)
                throw SDKThreadSafetyException.NotOnMainThread();
#endif
        }
    }
}