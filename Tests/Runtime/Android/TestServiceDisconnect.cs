// Module name: Inseye.SDK.Tests
// File name: TestServiceDisconnect.cs
// Last edit: 2023-11-07 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using System.Collections;
using Inseye;
using NUnit.Framework;
using Tests.Helpers.Android;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Runtime.Android
{
    public class TestServiceDisconnect
    {
        private AndroidServiceProxy _androidServiceProxy;
        private IDisposable _initializationKeeper;

        [SetUp]
        public void Setup()
        {
            if(Application.isEditor)
                Assert.Ignore("Android tests are skipped in editor.");
            _androidServiceProxy = new AndroidServiceProxy();
            _initializationKeeper = InseyeSDK.KeepEyeTrackerInitialized();
        }

        [UnityTest]
        public IEnumerator TestDisconnectAvailabilityChanges()
        {
            yield return null;
            Assert.True(
                InseyeSDK.GetEyetrackerAvailability() is InseyeEyeTrackerAvailability.Available
                    or InseyeEyeTrackerAvailability.NotCalibrated,
                "Eye tracker is not connected at the beginning of test, check if service is installed on device.");
            InseyeEyeTrackerAvailability lastSeenAvailability = InseyeEyeTrackerAvailability.Available;
            Action<InseyeEyeTrackerAvailability> callback = (a) => lastSeenAvailability = a;
            InseyeSDK.EyeTrackerAvailabilityChanged += callback;
            try
            {
                _androidServiceProxy.DisconnectService();
                yield return null;
                Assert.True(lastSeenAvailability is InseyeEyeTrackerAvailability.Disconnected,
                    "Eye tracker is connected after proxy disconnect.");
                _androidServiceProxy.ReconnectService();
                Assert.True(
                    InseyeSDK.GetEyetrackerAvailability() is InseyeEyeTrackerAvailability.Available
                        or InseyeEyeTrackerAvailability.NotCalibrated,
                    "Eye tracker is not connected after proxy call to reconnect was made.");
                yield return null;
                Assert.True(
                    lastSeenAvailability is InseyeEyeTrackerAvailability.Available
                        or InseyeEyeTrackerAvailability.NotCalibrated,
                    "Last seen availability have not changed after proxy reconnected.");
            }
            finally
            {
                InseyeSDK.EyeTrackerAvailabilityChanged -= callback;
            }
        }

        [UnityTest]
        public IEnumerator TestDisconnectCalibration()
        {
            yield return null;
            yield return null;
            Assert.True(
                InseyeSDK.GetEyetrackerAvailability() is InseyeEyeTrackerAvailability.Available
                    or InseyeEyeTrackerAvailability.NotCalibrated,
                "Eye tracker is not connected at the beginning of test, check if service is installed on device.");
            var calibrationProcedure = InseyeSDK.StartCalibration();
            _androidServiceProxy.DisconnectService();
            yield return null;
            _androidServiceProxy.ReconnectService();
            Assert.True(calibrationProcedure.InseyeCalibrationState == InseyeCalibrationState.FinishedFailed, 
                $"Calibration is not finished failed after service disconnected, but is {calibrationProcedure.InseyeCalibrationState:G} instead.");
            Assert.AreEqual("Service disconnected.", calibrationProcedure.GetCalibrationResultDescription());
        }

        [TearDown]
        public void TearDown()
        {
            if(Application.isEditor)
                return;
            _initializationKeeper?.Dispose();
            _androidServiceProxy.Dispose();
        }
    }
}