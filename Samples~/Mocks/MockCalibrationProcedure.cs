// Module name: com.inseye.unity.sdk.samples.mock
// File name: MockCalibrationProcedure.cs
// Last edit: 2023-10-05 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.
#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Inseye.Samples.Mocks
{
    /// <summary>
    ///     Mock calibration procedure, that presents new calibration point with about 3 seconds interval.
    /// </summary>
    public sealed class MockCalibrationProcedure : Interfaces.ICalibrationProcedure
    {
        public static readonly CalibrationPoint[] DefaultCalibrationPattern =
        {
            new(0, 0, 3000),
            new(15f, 15f, 3000),
            new(15f, -15f, 3000),
            new(-15f, -15f, 3000),
            new(-15f, 15f, 3000),
            new(0, 15f, 3000),
            new(15f, 0, 3000),
            new(0, -15f, 3000),
            new(-15f, 0, 3000),
            new(0, 0, 3000)
        };

        private bool _reportReadyCalled;
        private readonly CancellationTokenSource _cts = new();

        private readonly CalibrationPoint[] _calibrationPattern;
        private bool _disposed;
        private int _lastMarkedPointIndex = -1;
        private int _index;

        /// <summary>
        ///     Creates new calibration procedure simulating connection with calibration service
        /// </summary>
        public MockCalibrationProcedure()
        {
            InseyeCalibrationState = InseyeCalibrationState.Ongoing;
            _calibrationPattern = DefaultCalibrationPattern;
        }

        public MockCalibrationProcedure(CalibrationPoint[] calibrationPattern) : this()
        {
            _calibrationPattern = calibrationPattern;
        }

        /// <summary>
        ///     Disposes mock calibration procedure and terminates associated task.
        ///     If calibration was disposed while executing then marks calibration as failed.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            _cts?.Cancel();
            _cts?.Dispose();
            if (InseyeCalibrationState is InseyeCalibrationState.NotStarted or InseyeCalibrationState.Ongoing)
                InseyeCalibrationState = InseyeCalibrationState.FinishedFailed;
        }

        public int CurrentPointIndex => _index;


        /// <inheritdoc cref="Inseye.Interfaces.ICalibrationProcedure.CurrentPoint" />
        public Vector2 CurrentPoint => _calibrationPattern[_index].Position * Mathf.Deg2Rad;

        /// <inheritdoc cref="Inseye.Interfaces.ICalibrationProcedure.InseyeCalibrationState" />
        public InseyeCalibrationState InseyeCalibrationState { get; private set; }

        public string? GetCalibrationResultDescription()
        {
            return null;
        }

        /// <inheritdoc cref="Inseye.Interfaces.ICalibrationProcedure.ReportReadyToDisplayPoints" />
        public void ReportReadyToDisplayPoints()
        {
            if (_reportReadyCalled)
                throw new InvalidOperationException(
                    $"{nameof(ReportReadyToDisplayPoints)} can be called single time in calibration lifetime");
            _lastMarkedPointIndex = -1;
            _reportReadyCalled = true;
            CalibrationTask();
        }

        /// <summary>
        ///     Logs start of point display
        /// </summary>
        public void MarkStartOfPointDisplay()
        {
            if (!_reportReadyCalled)
                throw new InvalidOperationException(
                    $"{nameof(MarkStartOfPointDisplay)} can be called after {nameof(ReportReadyToDisplayPoints)}");
            if (_lastMarkedPointIndex > _index - 1)
                throw new InvalidOperationException(
                    $"{nameof(MarkStartOfPointDisplay)} can be called at most as much times as displayed points");
            _lastMarkedPointIndex++;
        }

        /// <inheritdoc cref="Interfaces.ICalibrationProcedure.AbortCalibration" />
        public void AbortCalibration()
        {
            Dispose();
        }

        /// <summary>
        ///     Task responsible for changing calibration point
        /// </summary>
        private async void CalibrationTask()
        {
            var token = _cts.Token;
            try
            {
                while (_index + 1 < _calibrationPattern.Length && !token.IsCancellationRequested)
                    if (_index == _lastMarkedPointIndex) // wait until client reports that is on position
                    {
                        await Task.Delay(_calibrationPattern[_index].DurationMs,
                            token);
                        _index++; // set new calibration point
                    }
                    else
                        await Task.Yield();
            }
            catch (TaskCanceledException)
            {
            } // just silently ignore - it is expected
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                if (_disposed)
                    InseyeCalibrationState = InseyeCalibrationState.FinishedFailed;
                else if (_cts.Token.IsCancellationRequested)
                    InseyeCalibrationState = InseyeCalibrationState.FinishedFailed;
                else
                    InseyeCalibrationState = InseyeCalibrationState.FinishedSuccessfully;
            }
        }
    }
}