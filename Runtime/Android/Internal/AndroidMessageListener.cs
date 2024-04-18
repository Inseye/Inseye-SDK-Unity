// Module name: com.inseye.unity.sdk
// File name: AndroidMessageListener.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using Inseye.Internal.Extensions;
using Inseye.Internal.Interfaces;
using UnityEngine;

namespace Inseye.Android.Internal
{
    internal sealed class EyeTrackerAvailabilityEvent
    {
        private InseyeEyeTrackerAvailability _lastEvent;
        public bool HasEvent { get; private set; }

        public bool TryConsumeEvent(ref InseyeEyeTrackerAvailability value)
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

        public void SetEvent(InseyeEyeTrackerAvailability value)
        {
            lock (this)
            {
                HasEvent = true;
                _lastEvent = value;
            }
        }
    }

    internal sealed class AndroidMessageListener : MonoBehaviour, IStateUser
    {
        private static AndroidMessageListener _instance;
        private readonly EyeTrackerAvailabilityEvent _eventContainer = new();
        private bool _disposed;

        private Action<InseyeEyeTrackerAvailability> _listeners;
        public static bool Instantiated { get; private set; }
        public static int SubscribersCount { get; private set; }

        public static AndroidMessageListener Instance
        {
            get
            {
                if (!Instantiated)
                {
                    var gameObject = new GameObject(nameof(AndroidMessageListener))
                    {
                        hideFlags = HideFlags.HideInHierarchy
                    };
                    _instance = gameObject.AddComponent<AndroidMessageListener>();
                    Instantiated = true;
                    DontDestroyOnLoad(gameObject);
                }

                return _instance;
            }
        }

        private void Update()
        {
            InseyeEyeTrackerAvailability availability = default;
            if (!(_eventContainer.HasEvent && _eventContainer.TryConsumeEvent(ref availability)))
                return;
            _listeners.SafeInvoke(availability);
        }

        private void OnDestroy()
        {
            _disposed = true;
            if (_instance == this)
            {
                SubscribersCount = 0;
                _instance = null;
                Instantiated = false;
            }
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

        public static void Invoke(InseyeEyeTrackerAvailability availability)
        {
            Instance._listeners?.Invoke(availability);
        }

        public static void AddListener(Action<InseyeEyeTrackerAvailability> @delegate)
        {
            Instance._listeners += @delegate;
            SubscribersCount = Instance._listeners != null ? Instance._listeners.GetInvocationList().Length : 0;
        }

        public static Action<InseyeEyeTrackerAvailability> RemoveAllListeners()
        {
            if (!Instantiated)
                return default;
            SubscribersCount = 0;
            var listeners = Instance._listeners;
            Instance._listeners = null;
            return listeners;
        }

        public static void RemoveListener(Action<InseyeEyeTrackerAvailability> @delegate)
        {
            if (!Instantiated)
                return;
            Instance._listeners -= @delegate;
            SubscribersCount = Instance._listeners is null ? 0 : Instance._listeners.GetInvocationList().Length;
        }

        /// <summary>
        ///     Called by Android SDK java class
        /// </summary>
        /// <param name="enumINT"></param>
        public void InvokeEyeTrackerAvailabilityChanged(string enumINT)
        {
#if DEBUG_INSEYE_SDK
            Debug.Log($"{nameof(InvokeEyeTrackerAvailabilityChanged)}: {enumINT}");
#endif
            if (int.TryParse(enumINT, out var parsed))
                _eventContainer.SetEvent((InseyeEyeTrackerAvailability) parsed);
            else
                Debug.LogError($"Failed to parse availability event: {enumINT}");
        }
    }
}