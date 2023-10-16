// Module name: com.inseye.unity.sdk
// File name: InseyeGazeDataExtensions.cs
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
    ///     Extensions making processing of gaze samples easier.
    /// </summary>
    public static class InseyeGazeDataExtensions
    {
        /// <summary>
        ///     Calculates gaze mean position averaged between multiple samples.
        ///     Uses most accurate eye to calculate position.
        /// </summary>
        /// <param name="data">Gaze data series.</param>
        /// <returns>Mean position in eye tracker space.</returns>
        /// <exception cref="System.ArgumentException">Thrown when gaze positions are empty.</exception>
        public static Vector2 GazeMeanPosition(this ReadOnlySpan<InseyeGazeData> data)
        {
            return GazeMeanPosition(data, InseyeSDK.GetMostAccurateEye());
        }

        /// <summary>
        ///     Calculates gaze mean position averaged between multiple samples.
        /// </summary>
        /// <param name="data">Span of gaze positions.</param>
        /// <param name="eyesUsedInEvaluation">Which eye(s) should be explicitly used as gaze data source.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Thrown when gaze positions are empty.</exception>
        // ReSharper disable once RedundantNameQualifier
        public static Vector2 GazeMeanPosition(this System.ReadOnlySpan<InseyeGazeData> data, Eyes eyesUsedInEvaluation)
        {
            if (data.IsEmpty)
                throw new ArgumentException("Data array cannot be empty");
            var position = Vector2.zero;
            switch (eyesUsedInEvaluation)
            {
                case Eyes.Both:
                    for (var i = 0; i < data.Length; i++)
                        position += (data[i].LeftEyePosition + data[i].RightEyePosition) / 2;
                    break;
                case Eyes.Left:
                    for (var i = 0; i < data.Length; i++)
                        position += data[i].LeftEyePosition;
                    break;
                case Eyes.Right:
                    for (var i = 0; i < data.Length; i++)
                        position += data[i].RightEyePosition;
                    break;
            }

            return position / data.Length;
        }

        /// <summary>
        ///     Returns position based on currently set most accurate eye.
        /// </summary>
        /// <param name="data">Input gaze data</param>
        /// <returns>Position taken from most accurate eye.</returns>
        public static Vector2 MostAccurateEyePosition(this InseyeGazeData data)
        {
            return InseyeSDK.GetMostAccurateEye() switch
            {
                Eyes.Both => (data.LeftEyePosition + data.RightEyePosition) / 2,
                Eyes.Right => data.RightEyePosition,
                Eyes.Left => data.LeftEyePosition,
                _ => throw new NotImplementedException()
            };
        }

        /// <summary>
        ///     Calculates gaze ray in camera space based on camera and viewport converter.
        /// </summary>
        /// <param name="data">Gaze data from eyetracker.</param>
        /// <param name="cameraTransform">Player camera.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown if camera is null.</exception>
        public static Ray ToGazeRay(this in InseyeGazeData data, Transform cameraTransform)
        {
            if (cameraTransform is null)
                throw new ArgumentNullException(nameof(cameraTransform));
            var position = data.MostAccurateEyePosition();
            return position.ToGazeRayInternal(cameraTransform);
        }

        /// <summary>
        ///     Calculates gaze data position in world coordinates.
        ///     Uses most accurate eye as a source.
        /// </summary>
        /// <param name="data">Gaze data.</param>
        /// <param name="distanceFromCamera">Distance of gaze point from camera.</param>
        /// <param name="cameraTransform">Used camera.</param>
        /// <returns>Gaze data point in world coordinates.</returns>
        public static Vector3 ToWorldPoint(this in InseyeGazeData data, float distanceFromCamera,
            Transform cameraTransform)
        {
            return data.MostAccurateEyePosition().TrackerToWorldPoint(distanceFromCamera, cameraTransform);
        }

        /// <summary>
        ///     Calculates gaze data position in local coordinates.
        ///     Uses most accurate eye as a source.
        ///     An assumption is made that head direction is forward (0,0,1).
        /// </summary>
        /// <param name="data">Gaze data.</param>
        /// <param name="distanceFromCamera">Distance of gaze point from camera.</param>
        /// <returns>Gaze data point in local coordinates.</returns>
        public static Vector3 ToLocalPoint(this in InseyeGazeData data, float distanceFromCamera)
        {
            return data.MostAccurateEyePosition().TrackerToLocalPoint(distanceFromCamera);
        }

        public static Quaternion TrackerToLocalRotation(this in Vector2 data)
        {
            var loc = data.TrackerToLocalPoint(1.0f);
            return Quaternion.FromToRotation(Vector3.forward, loc);
        }
    }
}