// Module name: Inseye.SDK.Tests.Helpers.Android
// File name: AndroidServiceProxy.cs
// Last edit: 2023-11-07 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using UnityEngine;

namespace Tests.Helpers.Android
{
    public class AndroidServiceProxy : IDisposable
    {
        private readonly AndroidJavaObject _javaObject;
        private bool _disposed;
        private static AndroidJavaClass UnitySDKClass => new("com.inseye.unitysdk.UnitySDK"); 
        public AndroidServiceProxy()
        {
            using var sdkClass = UnitySDKClass;
            _javaObject = sdkClass.CallStatic<AndroidJavaObject>("injectServiceProxy");
        }

        public void DisconnectService()
        {
            ThrowIfDisposed();
            _javaObject.Call("proxyServiceDisconnect");
        }

        public void ReconnectService()
        {
            ThrowIfDisposed();
            _javaObject.Call("proxyServiceConnect");
        }

        public void MockGazeSource(int udpPort)
        {
            ThrowIfDisposed();
            _javaObject.Call("enableMockServiceGazeDataSource", udpPort);
        }

        public void DisableGazeSourceMock()
        {
            ThrowIfDisposed();
            _javaObject.Call("disableMockServiceGazeDataSource");
        }
        
        
        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            _javaObject?.Dispose();
            using var sdkClass = UnitySDKClass;
            sdkClass.CallStatic("revokeServiceProxy");
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AndroidServiceProxy));
        }
    }
}