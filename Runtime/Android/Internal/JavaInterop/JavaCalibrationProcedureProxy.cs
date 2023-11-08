// Module name: com.inseye.unity.sdk
// File name: JavaCalibrationProcedureProxy.cs
// Last edit: 2023-11-08 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using UnityEngine;

namespace Inseye.Android.Internal.JavaInterop
{
    internal class JavaCalibrationProcedureProxy : IDisposable
    {
        private readonly AndroidJavaObject _javaObject;
        private bool _disposed;
        public JavaCalibrationProcedureProxy(AndroidJavaObject javaObject)
        {
            _javaObject = javaObject;
        }


        public void ReportReadyToDisplayPoints()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(JavaCalibrationProcedureProxy));
            _javaObject.Call("markReadyForPointDisplay");

        }

        public void AbortCalibration()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(JavaCalibrationProcedureProxy));
            _javaObject.Call("abortCalibration");
        }

        public string ReadOptionalErrorMessage()
        {
            return _javaObject.Call<string>("readOptionalErrorMessage");
        }
        
        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            _javaObject?.Dispose();
        }
    }
}