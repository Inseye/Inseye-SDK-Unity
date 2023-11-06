// Module name: com.inseye.unity.sdk
// File name: AndroidMessageListener.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using Inseye.Android.Internal.JavaInterop;
using Inseye.Internal.Extensions;
using Inseye.Internal.Interfaces;
using UnityEngine;

namespace Inseye.Android.Internal
{
    internal sealed class PooledEventSource<T>
    {
        private T _lastEvent;
        public bool HasEvent { get; private set; }

        public bool TryConsumeEvent(ref T value)
        {
            lock (this)
            {
                if (HasEvent)
                {
                    value = _lastEvent;
                    HasEvent = false;
                    return true;
                }

                return false;
            }
        }

        public void SetEvent(T value)
        {
            lock (this)
            {
                HasEvent = true;
                _lastEvent = value;
            }
        }
    }

    internal sealed class AndroidMessageListener : JavaAndroidCallback, IStateUser
    {
        private readonly PooledEventSource<InseyeEyeTrackerAvailability> _availabilitySource = new();
        private readonly PooledEventSource<InseyeSDKState> _stateSource = new();
        public InseyeSDKState LastSDKState { get; private set; }
        public InseyeEyeTrackerAvailability LastAvailability { get; private set; }
        private bool _disposed;

        private Action<InseyeEyeTrackerAvailability> _listeners;
        public int SubscribersCount { get; private set; }

        public static AndroidMessageListener CreateInstance()
        {
            var gameObject = new GameObject(Guid.NewGuid().ToString())
            {
                hideFlags = HideFlags.HideInHierarchy
            };
            DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<AndroidMessageListener>();
        }

        private void Update()
        {
            InseyeEyeTrackerAvailability availability = default;
            InseyeSDKState state = default;
            if (_availabilitySource.HasEvent && _availabilitySource.TryConsumeEvent(ref availability))
                _listeners.SafeInvoke(availability);
            if (_stateSource.HasEvent && _stateSource.TryConsumeEvent(ref state))
                LastSDKState = state;

        }
        
        private void OnDestroy()
        {
            // TODO: There is possibility that AndroidMessageListener will be destroyed before java disposal
        }

        public InseyeSDKState RequiredInseyeSDKState =>
            InseyeSDKState.Initialized | InseyeSDKState.SubscribedToEyeTrackerEvents;

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true; // to prevent multiple Destroy calls
            Destroy(this);
        }

        public void SetAvailability(InseyeEyeTrackerAvailability availability)
        {
            if (availability == LastAvailability)
                return;
            LastAvailability = availability;
            _listeners?.Invoke(availability);
        }

        public void AddListener(Action<InseyeEyeTrackerAvailability> @delegate)
        {
            _listeners += @delegate;
            SubscribersCount = _listeners.GetInvocationList().Length;
        }

        public Action<InseyeEyeTrackerAvailability> RemoveAllListeners()
        {
            SubscribersCount = 0;
            var listeners = _listeners;
            _listeners = null;
            return listeners;
        }

        public void RemoveListener(Action<InseyeEyeTrackerAvailability> @delegate)
        {

            _listeners -= @delegate;
            SubscribersCount = _listeners.GetInvocationList().Length;
        }

        protected override void OnAvailabilityChanged(InseyeEyeTrackerAvailability availability)
        {
            _availabilitySource.SetEvent(availability);
        }

        protected override void OnSDKStateChanged(InseyeSDKState sdkState)
        {
            _stateSource.SetEvent(sdkState);
        }
    }
}