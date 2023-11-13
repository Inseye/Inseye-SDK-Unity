// Module name: com.inseye.unity.sdk
// File name: PinnedInt.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using System.Runtime.InteropServices;

namespace Inseye.Android.Internal
{
    [StructLayout(LayoutKind.Sequential)]
    internal class PinnedInt
    {
        private byte _disposedField;
        private GCHandle _gcHandle;
        public int Value;

        public PinnedInt()
        {
            _gcHandle = GCHandle.Alloc(this, GCHandleType.Pinned);
        }

        private bool IsDisposed
        {
            get => _disposedField == 1;
            set => _disposedField = (byte) (value ? 1 : 0);
        }

        public IntPtr GetValuePointer()
        {
            return new IntPtr((long) _gcHandle.AddrOfPinnedObject() +
                              (long) Marshal.OffsetOf<PinnedInt>(nameof(Value)));
        }

        ~PinnedInt()
        {
            Dispose(false);
        }

        private void Dispose(bool isDisposing)
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            if (isDisposing)
                GC.SuppressFinalize(this);
            _gcHandle.Free();
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}