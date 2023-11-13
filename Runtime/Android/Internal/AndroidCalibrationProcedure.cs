// Module name: com.inseye.unity.sdk
// File name: AndroidCalibrationProcedure.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.
#nullable enable
using System;
using System.Runtime.InteropServices;
using Inseye.Android.Internal.JavaInterop;
using Inseye.Exceptions;
using Inseye.Interfaces;
using Inseye.Internal;
using Inseye.Internal.Interfaces;
using UnityEngine;

namespace Inseye.Android.Internal
{
    internal sealed class AndroidCalibrationProcedure : ICalibrationProcedure, IStateUser
    {
        private readonly JavaCalibrationProcedureProxy _javaCalibrationProcedureProxy;
        private readonly CalibrationDataHandle _calibrationDataHandle;
        private bool _disposed;
        private GCHandle _pinnedDataHandle;
        private bool _readyToDisplayCalled;
        private string? _errorMessage;

        internal AndroidCalibrationProcedure(CalibrationDataHandle calibrationDataHandle, JavaCalibrationProcedureProxy proxy)
        {
            _javaCalibrationProcedureProxy = proxy ?? throw new ArgumentNullException(nameof(proxy));
            _calibrationDataHandle = calibrationDataHandle;
        }


        public int CurrentPointIndex => _calibrationDataHandle.Data.PointCounter;

        public Vector2 CurrentPoint =>
            new(_calibrationDataHandle.Data.CalibrationPointRequest.x, _calibrationDataHandle.Data.CalibrationPointRequest.y);

        public InseyeCalibrationState InseyeCalibrationState => (InseyeCalibrationState) _calibrationDataHandle.Data.CalibrationStatus;

        public void Dispose()
        {
            Dispose(true);
        }

        public string? GetCalibrationResultDescription()
        {
            if (!_disposed)
                _errorMessage = _javaCalibrationProcedureProxy.ReadOptionalErrorMessage();
            return _errorMessage;
        }

        /// <inheritdoc cref="Interfaces.ICalibrationProcedure.ReportReadyToDisplayPoints" />
        public void ReportReadyToDisplayPoints()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AndroidCalibrationProcedure));
            if (_readyToDisplayCalled)
                return;
            _readyToDisplayCalled = true;
            try
            {
                _javaCalibrationProcedureProxy.ReportReadyToDisplayPoints();
            }
            catch (AndroidJavaException internalException)
            {
                throw new SDKCalibrationException($"Failed to call {nameof(ReportReadyToDisplayPoints)}.", internalException);
            }
        }

        /// <inheritdoc cref="Interfaces.ICalibrationProcedure.MarkStartOfPointDisplay" />
        public void MarkStartOfPointDisplay()
        {
            if (!_readyToDisplayCalled)
                throw new InvalidOperationException(
                    $"{nameof(ReportReadyToDisplayPoints)} must be called before {nameof(MarkStartOfPointDisplay)}");
            if (_disposed)
                throw new ObjectDisposedException(nameof(AndroidCalibrationProcedure));
            _calibrationDataHandle.Data.CalibrationPointResponses =
                new CalibrationPointResponse(_calibrationDataHandle.Data.CalibrationPointRequest.x,
                    _calibrationDataHandle.Data.CalibrationPointRequest.y);
        }

        /// <inheritdoc cref="Interfaces.ICalibrationProcedure.AbortCalibration" />
        public void AbortCalibration()
        {
            Dispose(true);
        }

        InseyeSDKState IStateUser.RequiredInseyeSDKState => InseyeSDKState.Initialized | InseyeSDKState.Calibration;

        ~AndroidCalibrationProcedure()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            _disposed = true;
            try
            {
                if (disposing) // can only be called in proper dispose
                {
                    GC.SuppressFinalize(this);
                    if (InseyeCalibrationState is InseyeCalibrationState.NotStarted or InseyeCalibrationState.Ongoing)
                    {
                        _javaCalibrationProcedureProxy.AbortCalibration();
                        _errorMessage = _javaCalibrationProcedureProxy.ReadOptionalErrorMessage();
                    }
                }
            }
            finally
            {
                _calibrationDataHandle.Dispose();
                _javaCalibrationProcedureProxy.Dispose();
                InseyeSDK.CurrentImplementation.SDKStateManager.RemoveListener(this);
            }
        }
    }
}