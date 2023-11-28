using System;
using UnityEngine;

namespace Tests.Helpers.Android
{
    public class AndroidCalibrationProxy : IDisposable
    {
        private readonly AndroidJavaObject _javaProxyObject;
        private bool _disposed;

        public AndroidCalibrationProxy(AndroidJavaObject javaObject)
        {
            _javaProxyObject = javaObject;
        }

        /// <summary>
        /// Sets response used when Unity calls <see cref="Inseye.Interfaces.ICalibrationProcedure.ReportReadyToDisplayPoints"/>
        /// </summary>
        public void SetReportReadyToDisplayPointsErroneousResponse(string errorMessage)
        {
            ThrowIfDisposed();
            _javaProxyObject.Call("setReadyToReceiveCalibrationPointResponse", errorMessage, 0f, 0f);
        }

        public void SetReportReadyToDisplayPointsSuccessfulResponse(Vector2 initialPosition)
        {
            ThrowIfDisposed();
            _javaProxyObject.Call("setReadyToReceiveCalibrationPointResponse", "", initialPosition.x,
                initialPosition.y);
        }

        public void FinishCalibrationAsServiceSuccessfully() =>
            FinishCalibrationAsServiceErroneously(string.Empty);


        public void FinishCalibrationAsServiceErroneously(string errorMessage)
        {
            ThrowIfDisposed();
            _javaProxyObject.Call("finishCalibrationAsService", errorMessage);
        }

        public void SetNextCalibrationPoint(Vector2 point)
        {
            ThrowIfDisposed();
            _javaProxyObject.Call("setNextCalibrationPointAsService", point.x, point.y);
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _javaProxyObject?.Dispose();
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AndroidCalibrationProxy));
        }
    }
}