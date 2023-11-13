// Module name: com.inseye.unity.sdk
// File name: CalibrationDataHandle.cs
// Last edit: 2023-11-08 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using System.Runtime.InteropServices;
using Inseye.Internal;

namespace Inseye.Android.Internal
{
    internal class CalibrationDataHandle : IDisposable
    {
        private bool _disposed;
        private static readonly IntPtr RequestsOffset =
            Marshal.OffsetOf<CalibrationData>(nameof(CalibrationData.CalibrationPointRequest));

        private static readonly IntPtr ResponseOffset =
            Marshal.OffsetOf<CalibrationData>(nameof(CalibrationData.CalibrationPointResponses));

        private static readonly IntPtr StatusOffset =
            Marshal.OffsetOf<CalibrationData>(nameof(CalibrationData.CalibrationStatus));

        private static readonly IntPtr PointCounterOffset =
            Marshal.OffsetOf<CalibrationData>(nameof(CalibrationData.PointCounter));

        internal IntPtr GetCalibrationPointRequestPointer()
        {
            if (!_disposed)
                return new IntPtr((long) _pinnedDataHandle.AddrOfPinnedObject() +
                                  (long) RequestsOffset);
            throw new ObjectDisposedException(nameof(CalibrationDataHandle));
        }
        
        internal IntPtr GetCalibrationPointResponsePointer()
        {
            if (!_disposed)
                return new IntPtr((long) _pinnedDataHandle.AddrOfPinnedObject() +
                                  (long) ResponseOffset);
            throw new ObjectDisposedException(nameof(CalibrationDataHandle));
        }

        internal IntPtr GetCalibrationStatusPointer()
        {
            if (!_disposed)
                return new IntPtr((long) _pinnedDataHandle.AddrOfPinnedObject() +
                                  (long) StatusOffset);
            throw new ObjectDisposedException(nameof(CalibrationDataHandle));
        }

        internal IntPtr GetPointCounterPointer()
        {
            if (!_disposed)
                return new IntPtr((long) _pinnedDataHandle.AddrOfPinnedObject() +
                                  (long) PointCounterOffset);
            throw new ObjectDisposedException(nameof(CalibrationDataHandle));
        }

        [StructLayout(LayoutKind.Sequential)]
        public class CalibrationData
        {
            public CalibrationPointRequest CalibrationPointRequest;
            public CalibrationPointResponse CalibrationPointResponses;
            public int CalibrationStatus = 0;
            public int PointCounter;
        }
        public readonly CalibrationData Data;
        private GCHandle _pinnedDataHandle;

        public CalibrationDataHandle()
        {
            Data = new CalibrationData();
            _pinnedDataHandle = GCHandle.Alloc(Data, GCHandleType.Pinned);
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            _pinnedDataHandle.Free();
        }
    }
}