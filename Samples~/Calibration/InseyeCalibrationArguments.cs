// Module name: com.inseye.unity.sdk.samples.calibration
// File name: InseyeCalibrationArguments.cs
// Last edit: 2023-10-05 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

#nullable enable
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Inseye.Samples.Calibration
{
    public sealed class InseyeCalibrationArguments
    {
        public delegate void CalibrationFinishedCallback(InseyeCalibrationResult calibrationResult,
            string? errorMessage);
        public static readonly string SampleCalibrationScenePath =
            $"Samples/InsEye Unity SDK/{InseyeSDK.SDKVersion.ToString()}/Calibration/CalibrationScene";

        internal CalibrationFinishedCallback? OnCalibrationFinished;

        /// <summary>
        ///     Creates calibration arguments.
        /// </summary>
        /// <param name="sceneName">Name of calibration scene to load</param>
        /// <param name="procedure">Calibration procedure.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if procedure is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if selected scene was not included in build</exception>
        public InseyeCalibrationArguments(string sceneName, Interfaces.ICalibrationProcedure procedure)
        {
            if (procedure is null)
                throw new System.ArgumentNullException(nameof(procedure));
            var sceneBuildIndex = SceneUtility.GetBuildIndexByScenePath(sceneName);
            if (sceneBuildIndex == -1)
            {
                procedure.Dispose();
                throw new System.ArgumentException(
                    "Scene name is not valid. Check if scene was added to build in build settings.", nameof(sceneName));
            }

            SceneName = sceneName;
            CalibrationProcedure = procedure;
        }

        /// <summary>
        ///     Name of scene loaded for calibration
        /// </summary>
        public string SceneName { get; }

        /// <summary>
        ///     Calibration procedure.
        /// </summary>
        public Interfaces.ICalibrationProcedure CalibrationProcedure { get; }

        /// <summary>
        ///     Adds callback that will be invoked when calibration ends.
        ///     <exception cref="System.ArgumentNullException">Thrown when callback is null.</exception>
        /// </summary>
        /// <returns>Modified calibration arguments.</returns>
        public InseyeCalibrationArguments AddCalibrationFinishedCallback(CalibrationFinishedCallback callback)
        {
            OnCalibrationFinished += callback ?? throw new System.ArgumentNullException(nameof(callback));
            return this;
        }

        /// <summary>
        ///     Creates arguments for calibration procedure that uses scene included in samples and returns to current scene after
        ///     calibration is finished.
        /// </summary>
        /// <param name="procedure">Calibration procedure.</param>
        /// <returns>Calibration arguments.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if procedure is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if selected scene was not included in build</exception>
        public static InseyeCalibrationArguments PerformStandardCalibrationAndReturn(
            Interfaces.ICalibrationProcedure procedure)
        {
            var calib = StandardCalibration(procedure);
            var activeScene = SceneManager.GetActiveScene();
            var activeSceneBuildIndex = activeScene.buildIndex;

            void Callback(InseyeCalibrationResult result, string? errorMessage)
            {
                SceneManager.LoadScene(activeSceneBuildIndex);
                if (result != InseyeCalibrationResult.Successful && errorMessage != null) Debug.LogError($"Calibration finished unsuccessfully with error message: {errorMessage}");
            }

            calib.OnCalibrationFinished = Callback;
            return calib;
        }

        /// <summary>
        ///     Creates arguments for calibration procedure that uses scene included in samples.
        /// </summary>
        /// <param name="procedure">Calibration procedure</param>
        /// <returns>Calibration arguemnts.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if procedure is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if selected scene was not included in build</exception>
        public static InseyeCalibrationArguments StandardCalibration(Interfaces.ICalibrationProcedure procedure)
        {
            return new InseyeCalibrationArguments(SampleCalibrationScenePath, procedure);
        }
    }
}