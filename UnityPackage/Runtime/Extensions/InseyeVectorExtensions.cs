// Module name: com.inseye.unity.sdk
// File name: InseyeVectorExtensions.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using UnityEngine;

namespace Inseye.Extensions
{
    /// <summary>
    ///     Extensions with various converters from eye tracker coordinates to other (and vice versa).
    /// </summary>
    public static class InseyeVectorExtensions
    {
        /// <summary>
        ///     Converts point from Inseye eye tracker space to local coordinates.
        ///     An assumption is made that head direction is forward (0,0,1).
        /// </summary>
        /// <param name="angularPosition">Eye rotation in horizontal and vertical axis.</param>
        /// <param name="distanceFromOrigin">Distance from origin.</param>
        /// <returns>Local point.</returns>
        public static Vector3 TrackerToLocalPoint(this in Vector2 angularPosition, float distanceFromOrigin)
        {
            var y = Mathf.Tan(angularPosition.y);
            var x = Mathf.Tan(angularPosition.x);
            var vec = new Vector3(x, y, 1);
            return vec.normalized * distanceFromOrigin;
        }

        /// <summary>
        ///     Converts point from Inseye eye tracker space to world coordinates.
        ///     An assumption is made that head direction is forward (0,0,1).
        /// </summary>
        /// <param name="angularPosition">Eye rotation in horizontal and vertical axis.</param>
        /// <param name="distanceFromCamera">Point distance from camera.</param>
        /// <param name="cameraTransform">Camera transform.</param>
        /// <returns>World point.</returns>
        public static Vector3 TrackerToWorldPoint(this in Vector2 angularPosition, float distanceFromCamera,
            Transform cameraTransform)
        {
            if (cameraTransform is null)
                throw new ArgumentNullException(nameof(cameraTransform));
            var localPoint = TrackerToLocalPoint(angularPosition, distanceFromCamera);
            var rotatedLocal = cameraTransform.rotation * localPoint;
            return rotatedLocal + cameraTransform.position;
        }

        /// <summary>
        ///     Calculates gaze ray in camera space based on camera and viewport converter.
        /// </summary>
        /// <param name="position">Gaze position data from eyetracker.</param>
        /// <param name="cameraTransform">Player camera transform.</param>
        /// <returns>Gaze ray that points at angle specified by position starting at camera origin.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if camera is null.</exception>
        public static Ray TrackerToGazeRay(this in Vector2 position, Transform cameraTransform)
        {
            if (cameraTransform is null)
                throw new ArgumentNullException(nameof(cameraTransform));
            return position.ToGazeRayInternal(cameraTransform);
        }

        /// <summary>
        ///     Converts viewport position to angles in eye tracker space.
        /// </summary>
        /// <param name="viewportPosition">Viewport position.</param>
        /// <param name="camera">Camera.</param>
        /// <returns>Point in eye tracker space.</returns>
        /// <exception cref="System.ArgumentException">Thrown if camera is null.</exception>
        public static Vector2 ViewportToTrackerPoint(this Vector2 viewportPosition, Camera camera)
        {
            if (camera == null)
                throw new ArgumentException(nameof(camera));
            return camera.ViewportToWorldPoint(new Vector3(viewportPosition.x, viewportPosition.y, 1f))
                .WorldToTrackerPointInternal(camera.transform);
        }

        /// <summary>
        ///     Converts world position to angles in eye tracker space.
        /// </summary>
        /// <param name="worldPoint">Point in world.</param>
        /// <param name="cameraTransform">Transform component of camera object.</param>
        /// <returns>Point in eye tracker space. Returns (-180,-180) if point behind camera object.</returns>
        /// <exception cref="System.ArgumentException">Thrown if cameraTransform is null.</exception>
        public static Vector2 WorldToTrackerPoint(this Vector3 worldPoint, Transform cameraTransform)
        {
            if (cameraTransform == null)
                throw new ArgumentException(nameof(cameraTransform));
            return WorldToTrackerPointInternal(worldPoint, cameraTransform);
        }

        /// <summary>
        ///     Converts camera local position to angles in eye tracker space.
        /// </summary>
        /// <param name="localPoint">Local point relative to camera.</param>
        /// <returns>Point in eye tracker space. Returns (-180,-180) if point z is negative (behind camera).</returns>
        public static Vector2 LocalToTrackerPoint(this Vector3 localPoint)
        {
            var tanx = localPoint.x / localPoint.z;
            var tany = localPoint.y / localPoint.z;
            if (localPoint.z < 0)
                return new Vector2(-180, -180);
            return new Vector2(Mathf.Atan(tanx), Mathf.Atan(tany));
        }

        internal static Vector2 WorldToTrackerPointInternal(this Vector3 worldPoint, Transform cameraTransform)
        {
            var localPoint = cameraTransform.worldToLocalMatrix.MultiplyPoint(worldPoint);
            return LocalToTrackerPoint(localPoint);
        }

        internal static Ray ToGazeRayInternal(this Vector2 position, Transform cameraTransform)
        {
            var camPos = cameraTransform.position;
            var y = Mathf.Tan(position.y);
            var x = Mathf.Tan(position.x);
            var localPos = new Vector3(x, y, 1);
            var rotatedLocal = cameraTransform.rotation * localPos;
            return new Ray(camPos, rotatedLocal);
        }
    }
}