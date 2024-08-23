// Module name: com.inseye.unity.sdk
// File name: EyetrackerKeepaliveHandle.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using Inseye.Internal.Interfaces;

namespace Inseye.Internal
{
    internal sealed class EyeTrackerKeepaliveHandle : IStateUser
    {
        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
        }

        public InseyeSDKState RequiredInseyeSDKState => InseyeSDKState.Initialized;

        ~EyeTrackerKeepaliveHandle()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
                GC.SuppressFinalize(this);
            _disposed = true;
            InseyeSDK.CurrentImplementationLazy.SDKStateManager.RemoveListener(this);
        }
    }
}