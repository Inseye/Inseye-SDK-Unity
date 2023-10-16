// Module name: com.inseye.unity.sdk
// File name: AndroidCalibrationProcedure.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using System.Runtime.InteropServices;
using Inseye.Exceptions;
using Inseye.Interfaces;
using Inseye.Internal;
using Inseye.Internal.Interfaces;
using UnityEngine;

namespace Inseye.Android.Internal
{
    internal sealed class AndroidCalibrationProcedure : ICalibrationProcedure, IStateUser
    {
        private static readonly IntPtr RequestsOffset =
            Marshal.OffsetOf<PinnedData>(nameof(PinnedData.CalibrationPointRequest));

        private static readonly IntPtr ResponseOffset =
            Marshal.OffsetOf<PinnedData>(nameof(PinnedData.CalibrationPointResponses));

        private static readonly IntPtr StatusOffset =
            Marshal.OffsetOf<PinnedData>(nameof(PinnedData.CalibrationStatus));

        private static readonly IntPtr PointCounterOffset =
            Marshal.OffsetOf<PinnedData>(nameof(PinnedData.PointCounter));

        private readonly ICalibrationCallback _callback;
        private readonly PinnedData _pinnedData;

        internal readonly IntPtr CalibrationPointRequestPointer,
            CalibrationPointResponsePointer,
            CalibrationStatusPointer,
            PointCounterPointer;

        private bool _disposed;
        private GCHandle _pinnedDataHandle;
        private bool _readyToDisplayCalled;

        internal AndroidCalibrationProcedure(ICalibrationCallback callback)
        {
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
            _pinnedData = new PinnedData();
            _pinnedDataHandle = GCHandle.Alloc(_pinnedData, GCHandleType.Pinned);
            CalibrationPointRequestPointer = new IntPtr((long) _pinnedDataHandle.AddrOfPinnedObject() +
                                                        (long) RequestsOffset);
            CalibrationPointResponsePointer = new IntPtr((long) _pinnedDataHandle.AddrOfPinnedObject() +
                                                         (long) ResponseOffset);
            CalibrationStatusPointer = new IntPtr((long) _pinnedDataHandle.AddrOfPinnedObject() +
                                                  (long) StatusOffset);
            PointCounterPointer = new IntPtr((long) _pinnedDataHandle.AddrOfPinnedObject() + (long) PointCounterOffset);
        }


        public int CurrentPointIndex => _pinnedData.PointCounter;

        public Vector2 CurrentPoint =>
            new(_pinnedData.CalibrationPointRequest.x, _pinnedData.CalibrationPointRequest.y);

        public InseyeCalibrationState InseyeCalibrationState => (InseyeCalibrationState) _pinnedData.CalibrationStatus;

        public void Dispose()
        {
            Dispose(true);
        }

        /// <inheritdoc cref="Interfaces.ICalibrationProcedure.ReportReadyToDisplayPoints" />
        public void ReportReadyToDisplayPoints()
        {
            if (_readyToDisplayCalled)
                return;
            _readyToDisplayCalled = true;
            try
            {
                _callback.ReportReadyToDisplayPoints();
            }
            catch (SDKInternalException internalException)
            {
                throw new SDKCalibrationException("Internal exception", internalException);
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
            _pinnedData.CalibrationPointResponses =
                new CalibrationPointResponse(_pinnedData.CalibrationPointRequest.x,
                    _pinnedData.CalibrationPointRequest.y);
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
            _pinnedDataHandle.Free();
            if (!disposing)
                return;
            GC.SuppressFinalize(this);
            try
            {
                if (InseyeCalibrationState is InseyeCalibrationState.NotStarted or InseyeCalibrationState.Ongoing)
                    _callback.AbortCalibration();
            }
            finally
            {
                InseyeSDK.CurrentImplementation.SDKStateManager.RemoveListener(this);
            }
        }
    }
}