// Module name: com.inseye.unity.sdk
// File name: CameraListener.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Inseye.Internal
{
    public abstract class CameraListener
    {
        private readonly WeakEvent _weakEvent;

        public CameraListener()
        {
            _weakEvent = new WeakEvent(this);
            CheckInitialized();
        }

        protected bool Initialized { get; private set; }
        public Camera Camera { get; private set; }

        ~CameraListener()
        {
            _weakEvent.Dispose();
        }

        protected bool CheckInitialized()
        {
            if (!Initialized)
                RefreshCamera();
            return Initialized;
        }

        protected void RefreshCamera()
        {
            Initialized = false;
            Camera = Camera.main;
            if (Camera is null)
            {
                Debug.LogWarning("Failed to refresh camera");
                return;
            }

            if (!Camera.TryGetComponent<CameraMonitor>(out var cameraListener))
                cameraListener = Camera.gameObject.AddComponent<CameraMonitor>();
            cameraListener.AddListener(this);

            OnRefreshCamera();
            Initialized = true;
        }

        protected virtual void OnRefreshCamera()
        {
        }

        private class WeakEvent : IDisposable
        {
            private readonly WeakReference<CameraListener> _ref;

            public WeakEvent(CameraListener cameraListener)
            {
                _ref = new WeakReference<CameraListener>(cameraListener);
                SceneManager.sceneLoaded += SceneLoadedHandler;
            }

            public void Dispose()
            {
                SceneManager.sceneLoaded -= SceneLoadedHandler;
            }

            private void SceneLoadedHandler(Scene scene, LoadSceneMode mode)
            {
                if (_ref.TryGetTarget(out var target))
                    target.RefreshCamera();
                else
                    Dispose(); // automatic dispose when target is lost
            }
        }

        private sealed class CameraMonitor : MonoBehaviour
        {
            private readonly HashSet<WeakReference<CameraListener>> _cameraListeners = new();
            private Camera _camera;
            private bool _disposed;

            private void Awake()
            {
                // hideFlags = HideFlags.HideInInspector;
                _camera = GetComponent<Camera>();
            }

            private void Update()
            {
                if (_camera == null) // camera was destroyed
                    Dispose();
            }

            private void OnDestroy()
            {
                Dispose();
            }

            public void AddListener(CameraListener listener)
            {
                _cameraListeners.Add(new WeakReference<CameraListener>(listener));
            }

            private void Dispose()
            {
                if (_disposed)
                    return;
                foreach (var cameraListener in _cameraListeners)
                    if (cameraListener.TryGetTarget(out var target))
                        target.Initialized = false;
                _cameraListeners.Clear();
                _disposed = true;
                Destroy(this);
            }
        }
    }
}