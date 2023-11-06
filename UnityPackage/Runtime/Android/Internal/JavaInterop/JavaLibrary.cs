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
    internal class JavaLibrary
    {
        /// <summary>
        ///     True when _javaClass is not null and not disposed
        /// </summary>
        private bool _initialized;

        private AndroidJavaClass _javaClass;

        public ErrorCodes Initialize(string callbackObject, long timeout)
        {
            ThrowIfNotOnMainThread();
            try
            {
                if (!_initialized)
                    _javaClass = new AndroidJavaClass("com.inseye.unitysdk.UnitySDK");
                _initialized = true;
                return (ErrorCodes) _javaClass.CallStatic<int>("initialize", callbackObject, timeout);
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

        public ErrorCodes StartCalibrationProcedure(IntPtr calibrationPointRequestPointer,
            IntPtr calibrationPointResponsePointer, IntPtr calibrationStatusPointer, IntPtr pointIndexPointer)
        {
            ThrowIfNotInitialized();
            ThrowIfNotOnMainThread();
            return (ErrorCodes) _javaClass.CallStatic<int>("startCalibrationProcedure",
                (long) calibrationPointRequestPointer, (long) calibrationPointResponsePointer,
                (long) calibrationStatusPointer, (long) pointIndexPointer);
        }

        public ErrorCodes SetReadyToDisplayCalibrationPoint()
        {
            ThrowIfNotInitialized();
            ThrowIfNotOnMainThread();
            return (ErrorCodes) _javaClass.CallStatic<int>("setReadyToDisplayCalibrationPoint");
        }

        public ErrorCodes AbortCalibrationProcedure()
        {
            ThrowIfNotInitialized();
            ThrowIfNotOnMainThread();
            return (ErrorCodes) _javaClass.CallStatic<int>("abortCalibrationProcedure");
        }

        public ErrorCodes SubscribeToEvents()
        {
            ThrowIfNotInitialized();
            ThrowIfNotOnMainThread();
            return (ErrorCodes) _javaClass.CallStatic<int>("subscribeToEvents");
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

        public bool IsEyeTrackerCalibrated()
        {
            ThrowIfNotInitialized();
            ThrowIfNotOnMainThread();
            return _javaClass.CallStatic<bool>("isEyeTrackerCalibrated");
        }

        public ErrorCodes BeginRecordingRawData()
        {
            ThrowIfNotInitialized();
            ThrowIfNotOnMainThread();
            return (ErrorCodes) _javaClass.CallStatic<int>("beginRecordingRawData");
        }

        public string EndRecordingRawData()
        {
            ThrowIfNotInitialized();
            ThrowIfNotOnMainThread();
            return _javaClass.CallStatic<string>("endRecordingRawData");
        }

        public void Dispose()
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