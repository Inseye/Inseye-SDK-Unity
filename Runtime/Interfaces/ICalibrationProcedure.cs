// Module name: com.inseye.unity.sdk
// File name: ICalibrationProcedure.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using UnityEngine;

namespace Inseye.Interfaces
{
    /// <summary>
    ///     Calibration procedure interface.
    /// </summary>
    public interface ICalibrationProcedure : IDisposable
    {
        /// <summary>
        ///     Index of current calibration point to display in calibration procedure.
        ///     Number that increments each time when <see cref="CurrentPoint" /> was set by calibration procedure.
        /// </summary>
        public int CurrentPointIndex { get; }

        /// <summary>
        ///     Current calibration point angle position in radians.
        ///     Angle is measurement of rotation made with conjunction of rotation around headset up axis by 'x' and rotation
        ///     around headset left axis by 'y'.
        ///     'x' value should be in range of (-half of device horizontal field of view, half of device horizontal field of view)
        ///     where positive value represent position in right part of user field of view and negative values corresponds to left
        ///     part of field of view.
        ///     'y' value should be in range of (-half of device vertical field of view, half of device vertical field of view)
        ///     where positive value represent position in upper part of user field of view and negative values corresponds to
        ///     lower part of field of view.
        /// </summary>
        Vector2 CurrentPoint { get; }

        /// <summary>
        ///     State of calibration.
        /// </summary>
        InseyeCalibrationState InseyeCalibrationState { get; }

        /// <summary>
        /// Returns rich information about success or failure of calibration procedure.
        /// Should by called only after calibration finished.
        /// </summary>
        /// <returns>Optional string describing how well user is calibrated or why calibration procedure failed. Unstructured message.</returns>
        /// <exception cref="System.InvalidOperationException">Exception thrown when call to this method is made before calibration ended.</exception>
        string? GetCalibrationResultDescription(); 

        /// <summary>
        ///     Method called by client to inform calibration procedure that client is ready for the first calibration point.
        ///     Must not be called more than once per during calibration procedure lifetime.
        /// </summary>
        /// <exception cref="Exceptions.SDKCalibrationException">Thrown when calibration failed to report readiness.</exception>
        void ReportReadyToDisplayPoints();

        /// <summary>
        ///     Method called by client to inform calibration procedure that client started displaying point stored
        ///     in <see cref="CurrentPoint" /> property in given location.
        /// </summary>
        void MarkStartOfPointDisplay();

        /// <summary>
        ///     Aborts calibration and disposes the object.
        /// </summary>
        void AbortCalibration();
    }
}