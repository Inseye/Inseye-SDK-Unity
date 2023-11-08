// Module name: com.inseye.unity.sdk.samples.adapter
// File name: InseyeEyeGazeInputAdapter.cs
// Last edit: 2023-10-05 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using Inseye.Extensions;
using Inseye.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using InputDevice = UnityEngine.InputSystem.InputDevice;
#if !OPENXR || USE_INPUT_SYSTEM_POSE_CONTROL
using PoseState = UnityEngine.InputSystem.XR.PoseState;
using PoseControl = UnityEngine.InputSystem.XR.PoseControl;
#else
using PoseState = UnityEngine.XR.OpenXR.Input.Pose;
using PoseControl = UnityEngine.XR.OpenXR.Input.PoseControl;
#endif
#if OPENXR_1_6_OR_NEWER
// Opt-in Scripting Define Symbol to use Input System PoseControl with com.unity.xr.openxr@1.6
using EyeGazeDevice = UnityEngine.XR.OpenXR.Features.Interactions.EyeGazeInteraction.EyeGazeDevice;

#else
namespace Inseye.Adapter
{
    [Preserve]
    [InputControlLayout(displayName = "Inseye Gaze Device", isGenericTypeOfDevice = false)]
    internal class EyeGazeDevice : InputDevice
    {
        [Preserve]
        [InputControl(offset = 0, usages = new[] {"Device", "gaze"})]
        public PoseControl pose { get; private set; }

        protected override void FinishSetup()
        {
            base.FinishSetup();
            pose = GetChildControl<PoseControl>("pose");
        }
    }
}
#endif
namespace Inseye.Adapter
{
    /// <summary>
    ///     Injects Inseye eye tracker gaze data into Unity Input system in the form of OpenXR compatible eye tracking device
    ///     when eye tracker is available, <see cref="Inseye.InseyeEyeTrackerAvailability" />.
    ///     Works both with and without com.unity.xr.openxr package.
    ///     There can be multiple instances of <see cref="InseyeEyeGazeInputAdapter" /> in loaded scenes and everything should
    ///     work as expected however, it is recommended to keep single instance for optimization reasons.
    /// </summary>
    public sealed class InseyeEyeGazeInputAdapter : MonoBehaviour
    {
        private static int registerCalls, initializedCount;

        [Tooltip("Which eye to use as a gaze position source.")]
        [SerializeField]
        private EyeOptions eyeOptions;

        private InputDevice _eyeGazeDevice;
        private PoseState _eyePose;
        private IGazeProvider _gazeProvider;
        private XRHMD _hmd;

        private bool _initialized;
        private PoseControl _poseControl;
        private bool _useHmd;

        private void Awake()
        {
            // register layout
            // OpenXR usually also registers this layout but implementation may depend on provider and is not guaranteed
            registerCalls++;
#if OPENXR_1_6_OR_NEWER
            InputSystem.RegisterLayout<EyeGazeDevice>(
                "EyeGaze",
                new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct("Eye Tracking OpenXR"));
#else
            InputSystem.RegisterLayout<EyeGazeDevice>(
                "EyeGaze",
                new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct("Eye Tracking Inseye"));
#endif
        }

        private void Update()
        {
            if (!_initialized)
                return;
            var eyes = eyeOptions switch
            {
                EyeOptions.System => InseyeSDK.GetMostAccurateEye(),
                EyeOptions.Both => Eyes.Both,
                EyeOptions.Left => Eyes.Left,
                EyeOptions.Right => Eyes.Right,
                _ => throw new NotImplementedException(eyeOptions.ToString("G"))
            };
            var lastFrameData = _gazeProvider.GetGazeDataFromLastFrame();
            if (lastFrameData.IsEmpty)
            {
                _eyePose.isTracked = false;
                _eyePose.trackingState = InputTrackingState.None;
                _eyePose.rotation = Quaternion.identity;
                _eyePose.position = Vector3.zero;
            }
            else
            {
                var trackingState = InputTrackingState.Rotation;
                var pos = lastFrameData.GazeMeanPosition(eyes);
                var rotation = pos.TrackerToLocalRotation();
                if (_useHmd)
                {
                    trackingState |= InputTrackingState.Position;
                    var devPos = _hmd.devicePosition;
                    _eyePose.position = new Vector3(devPos.x.value, devPos.y.value, devPos.z.value);
                    var devRot = _hmd.deviceRotation;
                    rotation = new Quaternion(devRot.x.value, devRot.y.value, devRot.z.value, devRot.w.value) *
                               rotation;
                    trackingState |= InputTrackingState.Position;
                }

                _eyePose.isTracked = true;
                _eyePose.trackingState = trackingState;
                _eyePose.rotation = rotation;
                if (!_eyeGazeDevice.added)
                {
                    _eyeGazeDevice = GetOrAddDevice();
                    _poseControl = (PoseControl) _eyeGazeDevice["pose"];
                }

                InputSystem.QueueDeltaStateEvent(_poseControl, _eyePose);
            }
        }

        private void OnEnable()
        {
            InputSystem.onDeviceChange += OnDeviceChangeHandler;
            InseyeSDK.EyeTrackerAvailabilityChanged += OnEyeTrackerAvailabilityChange;
            if (InseyeSDK.GetEyetrackerAvailability() != InseyeEyeTrackerAvailability.Available)
                Debug.LogWarning("Inseye Eye Tracker is not available and will not be registered as gaze device");
            else
                Initialize();
        }

        private void OnDisable()
        {
            InputSystem.onDeviceChange -= OnDeviceChangeHandler;
            InseyeSDK.EyeTrackerAvailabilityChanged -= OnEyeTrackerAvailabilityChange;
            Uninitialize();
        }

        private void OnDestroy()
        {
            if (--registerCalls == 0)
                InputSystem.RemoveLayout("EyeGaze");
        }

        private void OnEyeTrackerAvailabilityChange(InseyeEyeTrackerAvailability availability)
        {
            if (availability == InseyeEyeTrackerAvailability.Available)
                Initialize();
            else
                Uninitialize();
        }

        private void Initialize()
        {
            if (_initialized)
                return;
            _gazeProvider = InseyeSDK.GetGazeProvider();
            _eyeGazeDevice = GetOrAddDevice();
            if (_eyeGazeDevice is null)
            {
                _gazeProvider.Dispose();
                return;
            }

            initializedCount++;
            _poseControl = (PoseControl) _eyeGazeDevice["pose"];
            _hmd = InputSystem.GetDevice<XRHMD>();
            _useHmd = _hmd != null;
            _initialized = true;
        }

        private void Uninitialize()
        {
            if (!_initialized)
                return;
            --initializedCount;
            _initialized = false;
            _gazeProvider?.Dispose();
            if (_eyeGazeDevice is {added: true} && initializedCount == 0) InputSystem.RemoveDevice(_eyeGazeDevice);
        }

        private static InputDevice GetOrAddDevice()
        {
            var dev = InputSystem.GetDevice<EyeGazeDevice>();
            if (dev is not {added: true})
            {
                dev = InputSystem.AddDevice<EyeGazeDevice>();
                if (dev is null)
                {
                    Debug.LogError("Failed to create Eye Gaze device.");
                    return dev;
                }
            }

            return dev;
        }

        private void OnDeviceChangeHandler(InputDevice inputDevice, InputDeviceChange change)
        {
            if (inputDevice is not XRHMD hmdXrhmd)
                return;
            switch (change)
            {
                case InputDeviceChange.Added:
                case InputDeviceChange.Reconnected:
                case InputDeviceChange.Enabled:
                    _hmd = hmdXrhmd;
                    break;
                case InputDeviceChange.Disabled:
                case InputDeviceChange.Disconnected:
                case InputDeviceChange.Removed:
                    _hmd = null;
                    break;
                case InputDeviceChange.SoftReset:
                case InputDeviceChange.UsageChanged:
                case InputDeviceChange.ConfigurationChanged:
                case InputDeviceChange.HardReset:
                    break;
                default:
                    Debug.LogWarning($"Unhandled case for XRHMD: {inputDevice.name}, state: {change:G}");
                    break;
            }

            _useHmd = _hmd is not null && _hmd.added;
        }

        private enum EyeOptions
        {
            System,
            Left,
            Right,
            Both
        }
    }
}