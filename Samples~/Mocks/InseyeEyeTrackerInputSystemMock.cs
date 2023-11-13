// Module name: com.inseye.unity.sdk.samples.mock
// File name: InseyeEyeTrackerInputSystemMock.cs
// Last edit: 2023-10-05 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using Inseye.Extensions;
using Inseye.Interfaces;
using Inseye.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Inseye.Samples.Mocks
{
    /// <summary>
    ///     Eye tracking mock using input system action references as a source of gaze data.
    ///     Replaces original SKD in Awake.
    /// </summary>
    [DefaultExecutionOrder(-31000)]
    public sealed class InseyeEyeTrackerInputSystemMock : InseyeMockSDKImplementation, IGazeDataSource,
        IVersionedSpan<GazeData>
    {
        /// <summary>
        ///     Type of source for gaze data mock.
        /// </summary>
        public enum InputType
        {
            /// <summary>
            ///     Absolute gaze position on the screen.
            /// </summary>
            ScreenPosition,

            /// <summary>
            ///     Gaze input for controllers returning delta changes in position.
            /// </summary>
            Delta,

            /// <summary>
            ///     Static gaze position in the middle of screen.
            /// </summary>
            MiddleOfTheScreen,

            /// <summary>
            ///     Static gaze position in the middle of screen.
            /// </summary>
            ControllerPositionAndDirection
        }

        private readonly GazeData[] _array = new GazeData[1];
        private readonly InseyeGazeData[] _inseyeGazeArray = new InseyeGazeData[1];

        private Vector2 _cursorPosition;
        private bool _isDataValid;
        private Quaternion ControllerRotation => inputSource.action.ReadValue<Quaternion>();
        private Vector3 ControllerPosition => additionalInputSource.action.ReadValue<Vector3>();

        /// <summary>
        ///     Property for eye openness.
        /// </summary>
        public OpenedEyes OpenedEyes
        {
            get => openedEyes;
            set => openedEyes = value;
        }

        /// <inheritdoc cref="Inseye.Samples.Mocks.InseyeMockSDKImplementation.InternalGazeDataSource" />
        protected override IGazeDataSource InternalGazeDataSource => this;

        public ReadOnlySpan<InseyeGazeData> GetGazeDataFromLastFrame()
        {
            if (GetMostRecentGazeData(out var gazeData))
            {
                _inseyeGazeArray[0] = gazeData;
                return new ReadOnlySpan<InseyeGazeData>(_inseyeGazeArray);
            }

            return ReadOnlySpan<InseyeGazeData>.Empty;
        }

        public InseyeGazeDataEnumerator GetEnumerator()
        {
            if (inputSource.action is not {enabled: true})
                return new InseyeGazeDataEnumerator(new GazeDataEnumerator(Version, this));
            return new InseyeGazeDataEnumerator(new GazeDataEnumerator(Version - 1, this));
        }

        public bool GetMostRecentGazeData(out InseyeGazeData gazeData)
        {
            if (inputType == InputType.MiddleOfTheScreen)
            {
                gazeData = new InseyeGazeData(_array[0]);
                return true;
            }

            if (inputSource.action is not {enabled: true})
            {
                gazeData = default;
                return false;
            }

            gazeData = new InseyeGazeData(_array[0]);
            return true;
        }

        public int Version { get; private set; } = -1;
        ReadOnlySpan<GazeData> IVersionedSpan<GazeData>.Array => _array;

        private bool GetGazeDataFromMiddleOfScreen()
        {
            _array[0] = new GazeData
            {
                timeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                left_x = 0,
                left_y = 0,
                right_x = 0,
                right_y = 0,
                event_ = EyesToIntEvent(openedEyes)
            };
            Version = 0;
            return true;
        }

        private bool GetGazeDataFromPointerPosition(bool asDelta)
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
                return false;
            if (inputSource.action is not {enabled: true}) return false;
            var value = inputSource.action.ReadValue<Vector2>();
            _cursorPosition = asDelta ? _cursorPosition + value : value;
            _cursorPosition = new Vector2(Mathf.Clamp(_cursorPosition.x, 0, Screen.width),
                Mathf.Clamp(_cursorPosition.y, 0, Screen.height));
            var viewportPosition = new Vector2(_cursorPosition.x / Screen.width,
                _cursorPosition.y / Screen.height);
            var gazePosition = viewportPosition.ViewportToTrackerPoint(mainCamera);
            _array[0] = new GazeData
            {
                timeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                event_ = EyesToIntEvent(openedEyes),
                left_x = gazePosition.x,
                right_x = gazePosition.x,
                right_y = gazePosition.y,
                left_y = gazePosition.y
            };
            return true;
        }

        private bool GetGazeDataFromController()
        {
            _array[0] = default;
            var mainCamera = Camera.main;
            if (mainCamera == null)
                return false;

            var mainCameraTransform = mainCamera.transform;
            var forward = mainCameraTransform.localRotation * Vector3.forward; // camera forward in common parent space
            var plane = new Plane(-forward, forward * virtualDistanceFromCamera); // plane in common parent space
            var direction = ControllerRotation * Vector3.forward; // controller forward in common parent
            var origin = ControllerPosition - mainCameraTransform.localPosition; // common parent origin
            var ray = new Ray(origin, direction);
            if (!plane.Raycast(ray, out var enter))
            {
                _isDataValid = false;
                return _isDataValid;
            }

            var collisionPoint = ray.GetPoint(enter); // common parent space collision point
            var localUnrotated =
                Quaternion.Inverse(mainCameraTransform.localRotation) *
                collisionPoint; // camera local space collision point 
            var trackerPoint = new Vector2(Mathf.Atan2(localUnrotated.x, localUnrotated.z),
                Mathf.Atan2(localUnrotated.y, localUnrotated.z));
            _array[0] = new GazeData
            {
                timeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                event_ = EyesToIntEvent(openedEyes),
                left_x = trackerPoint.x,
                right_x = trackerPoint.x,
                right_y = trackerPoint.y,
                left_y = trackerPoint.y
            };

            return true;
        }

        private static int EyesToIntEvent(OpenedEyes openedEyes)
        {
            return openedEyes switch
            {
                OpenedEyes.Both => 0,
                OpenedEyes.Left => 2,
                OpenedEyes.Right => 1,
                OpenedEyes.None => 4,
                _ => throw new ArgumentOutOfRangeException(nameof(openedEyes), openedEyes, null)
            };
        }

        /// <summary>
        ///     Reference to InputSystem action used as source of mock data.
        ///     Represents controller rotation when used in ControllerPositionAndDirection mode.
        /// </summary>
        [SerializeField]
#pragma warning disable CS0628
        protected internal InputActionProperty inputSource; // protected for sake of documentation generator

        /// <summary>
        ///     Reference to additional input source used when controller position and direction is used as gaze source.
        ///     Represents controller position.
        /// </summary>
        [SerializeField]
        protected internal InputActionProperty additionalInputSource; // protected for sake of documentation generator

        /// <summary>
        ///     Stores information about which eyes are opened.
        /// </summary>
        [SerializeField]
        protected internal OpenedEyes openedEyes;

        /// <summary>
        ///     Distance from camera at which ray fired from controller is checked.
        /// </summary>
        [SerializeField]
        [Range(10, 1000)]
        [Tooltip("Distance from camera at which ray fired from controller is checked.")]
        protected internal float virtualDistanceFromCamera = 10f;

        /// <summary>
        ///     Source type of gaze mocked gaze data.
        /// </summary>
        [SerializeField]
        protected internal InputType inputType; // protected for sake of documentation generator

        /// <summary>
        ///     Pools <see cref="inputSource" /> based on selected <see cref="inputType" />
        /// </summary>
        protected void Update() // protected for sake of documentation generator
#pragma warning restore CS0628
        {
            Version++;
            _isDataValid = inputType switch
            {
                InputType.MiddleOfTheScreen => GetGazeDataFromMiddleOfScreen(),
                InputType.ScreenPosition => GetGazeDataFromPointerPosition(false),
                InputType.Delta => GetGazeDataFromPointerPosition(true),
                InputType.ControllerPositionAndDirection => GetGazeDataFromController(),
                _ => false
            };
        }
    }
}