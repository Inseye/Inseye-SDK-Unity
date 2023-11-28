// Module name: Inseye.SDK.Tests.Android
// File name: TestCalibrationAndroid.cs
// Last edit: 2023-11-28 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Collections;
using Inseye;
using NUnit.Framework;
using Tests.Helpers.Android;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Runtime.Android
{
    public class TestCalibrationAndroid
    {
        private AndroidServiceProxy _androidServiceProxy;
        private AndroidCalibrationProxy _androidCalibrationProxy;

        [SetUp]
        public void SetUp()
        {
            if (Application.isEditor)
                Assert.Ignore("Android tests are skipped in editor.");
            _androidServiceProxy = new AndroidServiceProxy();
            _androidCalibrationProxy = _androidServiceProxy.EnableCalibrationProxy();
        }

        [Test]
        public void TestErrorMessageOnFailedCalibration()
        {
            using var calibration = InseyeSDK.StartCalibration();
            Assert.IsNotNull(calibration);
            const string expectedErrorMessage = "an error message";
            _androidCalibrationProxy.FinishCalibrationAsServiceErroneously(expectedErrorMessage);
            var result = calibration.GetCalibrationResultDescription();
            Assert.AreEqual(expectedErrorMessage, result);
        }

        [TearDown]
        public void TearDown()
        {
            _androidCalibrationProxy?.Dispose();

            _androidServiceProxy?.RemoveProxyCalibration();
            _androidServiceProxy?.Dispose();
        }
    }
}