// Module name: com.inseye.unity.sdk.samples.mock
// File name: CalibrationPoint.cs
// Last edit: 2023-10-05 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using UnityEngine;

namespace Inseye.Samples.Mocks
{
    /// <summary>
    ///     Defines calibration point.
    /// </summary>
    public readonly struct CalibrationPoint
    {
        /// <summary>
        ///     Calibration point angle position in degrees.
        ///     Angle is measurement of rotation made with conjunction of rotation around headset up axis by 'x' and rotation
        ///     around left axis by 'y'.
        ///     'x' value should be in range of (-half of device horizontal field of view, half of device horizontal field of view)
        ///     where positive value represent position in right part of user field of view and negative values corresponds to left
        ///     part of field of view.
        ///     'y' value should be in range of (-half of device vertical field of view, half of device vertical field of view)
        ///     where positive value represent position in upper part of user field of view and negative values corresponds to
        ///     lower part of field of view.
        /// </summary>
        public readonly Vector2 Position;

        /// <summary>
        ///     Display duration of calibration point.
        /// </summary>
        public readonly int DurationMs;

        /// <summary>
        ///     Creates new calibration point.
        /// </summary>
        /// <param name="position">Calibration point angle position in degrees. Check <see cref="Position" />.</param>
        /// <param name="durationMs">Display duration of calibration point in milliseconds.</param>
        public CalibrationPoint(Vector2 position, int durationMs)
        {
            Position = position;
            DurationMs = durationMs;
        }

        /// <summary>
        ///     Creates new calibration point.
        /// </summary>
        /// <param name="x">Horizontal position in degrees.</param>
        /// <param name="y">Vertical position in degrees.</param>
        /// <param name="durationMs">Display duration of calibration point in milliseconds.</param>
        public CalibrationPoint(float x, float y, int durationMs)
        {
            Position = new Vector2(x, y);
            DurationMs = durationMs;
        }

        /// <summary>
        ///     Creates new calibration point.
        /// </summary>
        /// <param name="x">Calibration point horizontal angle position in radians. Check <see cref="Position" />.</param>
        /// <param name="y">Calibration point vertical angle position in radians. Check <see cref="Position" />.</param>
        /// <param name="durationMs">Display duration of calibration point in milliseconds.</param>
        /// <returns>Calibration point.</returns>
        public static CalibrationPoint FromRadians(float x, float y, int durationMs)
        {
            return new CalibrationPoint(x * Mathf.Rad2Deg, y * Mathf.Rad2Deg, durationMs);
        }

        /// <summary>
        ///     Creates new calibration point.
        /// </summary>
        /// <param name="position">Calibration point horizontal angle position in radians. Check <see cref="Position" />.</param>
        /// <param name="durationMs">Display duration of calibration point in milliseconds.</param>
        /// <returns>Calibration point.</returns>
        public static CalibrationPoint FromRadians(Vector2 position, int durationMs)
        {
            return new CalibrationPoint(position * Mathf.Rad2Deg, durationMs);
        }
    }
}