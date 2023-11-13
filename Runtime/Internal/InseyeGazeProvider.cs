// Module name: com.inseye.unity.sdk
// File name: InseyeGazeProvider.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using Inseye.Interfaces;
using Inseye.Internal.Interfaces;

namespace Inseye.Internal
{
    /// <summary>
    ///     Provides access to eye tracker data
    /// </summary>
    internal class InseyeGazeProvider : IStateUser, IGazeProvider
    {
        private bool _disposed;

        /// <inheritdoc cref="IGazeProvider.GetMostRecentGazeData" />
        /// <exception cref="System.ObjectDisposedException">Thrown when GazeProvider was disposed</exception>
        public bool GetMostRecentGazeData(out InseyeGazeData inseyeGazePosition)
        {
            ThrowIfDisposed();
            if (InseyeSDK.CurrentImplementation.GazeDataSource.GetMostRecentGazeData(out inseyeGazePosition))
                return true;
            return false;
        }

        /// <inheritdoc cref="IGazeProvider.GetGazeDataFromLastFrame" />
        /// <exception cref="System.ObjectDisposedException">Thrown when GazeProvider was disposed</exception>
        public ReadOnlySpan<InseyeGazeData> GetGazeDataFromLastFrame()
        {
            ThrowIfDisposed();
            return InseyeSDK.CurrentImplementation.GazeDataSource.GetGazeDataFromLastFrame();
        }

        /// <inheritdoc cref="IGazeProvider.GetEnumerator" />
        /// <exception cref="System.ObjectDisposedException">Thrown when GazeProvider was disposed</exception>
        public InseyeGazeDataEnumerator GetEnumerator()
        {
            ThrowIfDisposed();
            return InseyeSDK.CurrentImplementation.GazeDataSource.GetEnumerator();
        }

        /// <inheritdoc cref="IGazeProvider.GetEnumerator" />
        /// <exception cref="System.ObjectDisposedException">Thrown when GazeProvider was disposed</exception>
        IEnumerator<InseyeGazeData> IEnumerable<InseyeGazeData>.GetEnumerator()
        {
            ThrowIfDisposed();
            return InseyeSDK.CurrentImplementation.GazeDataSource.GetEnumerator();
        }

        /// <inheritdoc cref="IGazeProvider.GetEnumerator" />
        /// <exception cref="System.ObjectDisposedException">Thrown when GazeProvider was disposed</exception>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Disposes gaze provider. Further attempts to call any other method on object will result in
        ///     <see cref="System.ObjectDisposedException" />
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        InseyeSDKState IStateUser.RequiredInseyeSDKState =>
            InseyeSDKState.Initialized | InseyeSDKState.MostRecentGazePointAvailable;

        ~InseyeGazeProvider()
        {
            Dispose(false);
        }


        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException($"{nameof(InseyeGazeProvider)} is disposed");
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            _disposed = true;
            if (disposing)
                GC.SuppressFinalize(this);
            InseyeSDK.CurrentImplementation.SDKStateManager.RemoveListener(this);
        }
    }
}