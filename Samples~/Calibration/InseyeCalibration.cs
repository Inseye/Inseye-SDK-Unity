// Module name: com.inseye.unity.sdk.samples.calibration
// File name: InseyeCalibration.cs
// Last edit: 2023-10-05 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

#nullable enable
using System.Collections;
using Inseye.Exceptions;
using Inseye.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Inseye.Samples.Calibration
{
    /// <summary>
    ///     Sample inseye calibration procedure. Uses instances of <see cref="AnimationHelpers.CalibrationAnimation" /> to
    ///     animate calibration point.
    ///     An assumption is made that this component will be attached to main camera without any additional translation and
    ///     rotation relative to the camera.
    /// </summary>
    public sealed class InseyeCalibration : MonoBehaviour
    {
        private Interfaces.ICalibrationProcedure? _calibrationProcedure;
        private InseyeCalibrationArguments.CalibrationFinishedCallback? _onCalibrationFinished;
        private static readonly float Precision = Mathf.Tan(Mathf.Deg2Rad * 0.1f);
        private AnimationHelpers.CalibrationAnimation CurrentAnimation => animations[animationIndex];
        /// <summary>
        ///     Starts calibration procedure.
        /// Invokes callback when calibration finished.
        /// Non-blocking call.
        /// </summary>
        /// <param name="calibrationProcedure">Valid calibration procedure.</param>
        /// <param name="onFinishCallback">Callback invoked when calibration finishes.</param>
        /// <exception cref="System.ArgumentNullException">Exception thrown when provided calibration procedure is null.</exception>
        public void StartCalibration(Interfaces.ICalibrationProcedure calibrationProcedure,
            InseyeCalibrationArguments.CalibrationFinishedCallback? onFinishCallback = null)
        {
            _calibrationProcedure =
                calibrationProcedure ?? throw new System.ArgumentNullException(nameof(calibrationProcedure));
            _onCalibrationFinished += onFinishCallback;
            StartCoroutine(CalibrationCoroutine());
        }

        private void OnDestroy()
        {
            _calibrationProcedure?.Dispose();
        }
        
        /// <summary>
        ///     Starts calibration procedure. Returns task that ends with calibration procedure.
        /// </summary>
        /// <returns>Task that finishes with calibration.</returns>
        /// <param name="arguments">Calibration arguments, must not be null</param>
        /// <param name="token">Cancellation token.</param>
        /// <exception cref="System.OperationCanceledException">Thrown when cancellation token is cancelled.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when arguments are null.</exception>
        public static async System.Threading.Tasks.Task<(InseyeCalibrationResult result, string? resultDescription)> PerformCalibrationAsync(InseyeCalibrationArguments arguments,
            System.Threading.CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            var tcs = new System.Threading.Tasks.TaskCompletionSource<(InseyeCalibrationResult result, string? resultDescription)>();
            arguments.AddCalibrationFinishedCallback((status, errorMessage) => tcs.TrySetResult((status, errorMessage)));
            if (token.CanBeCanceled)
            {
                var registration = token.Register(() =>
                {
                    tcs.TrySetCanceled(token);
                    arguments.CalibrationProcedure.Dispose();
                });
                PerformCalibration(arguments);
                try
                {
                    return await tcs.Task;
                }
                finally
                {
                    await registration.DisposeAsync();
                }
            }

            PerformCalibration(arguments);
            return await tcs.Task;
        }

        /// <summary>
        ///     Starts calibration procedure. Non-blocking call.
        /// </summary>
        /// <param name="arguments">Calibration arguments, must not be null</param>
        /// <exception cref="System.ArgumentNullException">Thrown when arguments are null.</exception>
        public static void PerformCalibration(InseyeCalibrationArguments arguments)
        {
            if (arguments is null)
                throw new System.ArgumentNullException(nameof(arguments));

            void ActiveSceneChangedHandler(Scene oldScene, Scene newScene)
            {
                var inseyeCalibration = FindObjectOfType<InseyeCalibration>(true);
                if (inseyeCalibration != null)
                {
                    inseyeCalibration._onCalibrationFinished = arguments.OnCalibrationFinished;
                    inseyeCalibration.StartCalibration(arguments.CalibrationProcedure);
                }
                else
                {
                    Debug.LogError(
                        $"Missing game object with {nameof(InseyeCalibration)} in current calibration scene, aborting");
                    try
                    {
                        arguments.CalibrationProcedure.Dispose();
                    }
                    finally
                    {
                        arguments.OnCalibrationFinished?.Invoke(InseyeCalibrationResult.MissingCalibrationGameObject, null);
                    }
                }

                SceneManager.activeSceneChanged -= ActiveSceneChangedHandler;
            }

            SceneManager.activeSceneChanged += ActiveSceneChangedHandler;
            SceneManager.LoadScene(arguments.SceneName, LoadSceneMode.Single);
        }

        private IEnumerator CalibrationCoroutine()
        {
            if (displayText != null)
            {
                yield return new WaitForSeconds(2.5f);
                displayText.gameObject.SetActive(false);
            }

            calibrationPoint.gameObject.SetActive(true);
            yield return new WaitForSeconds(.5f);
            if (_calibrationProcedure is not null)
            {
                if (_calibrationProcedure.InseyeCalibrationState is InseyeCalibrationState.NotStarted
                    or InseyeCalibrationState.Ongoing)
                {
                    try
                    {
                        _calibrationProcedure.ReportReadyToDisplayPoints();
                    }
                    catch (SDKCalibrationException exception)
                    {
                        Debug.LogException(exception);
                        InvokeOnCalibrationFinished(InseyeCalibrationResult.FailedToStartCalibration, _calibrationProcedure.GetCalibrationResultDescription());
                        yield break;
                    }
                }
                else
                {
                    InvokeOnCalibrationFinished(InseyeCalibrationResult.InvalidState, _calibrationProcedure.GetCalibrationResultDescription());
                    yield break;
                }
            }
            else
            {
                InvokeOnCalibrationFinished(InseyeCalibrationResult.NullCalibrationProcedure, null);
                yield break;
            }

            var informedAboutDisplayedPoint = false;
            var lastPointIndex = -1;
            while (_calibrationProcedure.InseyeCalibrationState == InseyeCalibrationState.Ongoing)
            {
                if (lastPointIndex < _calibrationProcedure!.CurrentPointIndex)
                {
                    lastPointIndex = _calibrationProcedure.CurrentPointIndex;
                    CurrentAnimation.ExitAnimation();
                    var globalPosition =
                        _calibrationProcedure.CurrentPoint.TrackerToWorldPoint(distanceFromCamera, transform);
                    calibrationPointDestination.position = globalPosition;
                    informedAboutDisplayedPoint = false;
                    Debug.Log($"Set new destination point to {globalPosition}");
                    animationIndex = 0;
                    CurrentAnimation.EnterAnimation();
                }

                // swap done animations
                while (CurrentAnimation.HasFinished)
                {
                    var curr = CurrentAnimation;
                    animationIndex = (animationIndex + 1) % animations.Length;
                    CurrentAnimation.EnterAnimation();
                }

                CurrentAnimation.Animate(Time.deltaTime);
                var distance = Vector3.Distance(calibrationPoint.transform.position,
                    calibrationPointDestination.position);
                if (!informedAboutDisplayedPoint && distance < distanceFromCamera * Precision)
                {
                    informedAboutDisplayedPoint = true;
                    _calibrationProcedure.MarkStartOfPointDisplay();
#if DEBUG_INSEYE_SDK
                    Debug.Log(
                        $"Marking calibration point display, current requested point: {_calibrationProcedure.CurrentPoint * Mathf.Rad2Deg}");
#endif
                }

                yield return null;
            }

            switch (_calibrationProcedure.InseyeCalibrationState)
            {
                case InseyeCalibrationState.FinishedFailed:
                    InvokeOnCalibrationFinished(InseyeCalibrationResult.CalibrationAborted, _calibrationProcedure.GetCalibrationResultDescription());
                    break;
                case InseyeCalibrationState.FinishedSuccessfully:
                    InvokeOnCalibrationFinished(InseyeCalibrationResult.Successful, _calibrationProcedure.GetCalibrationResultDescription());
                    break;
                case InseyeCalibrationState.NotStarted:
                    throw new System.InvalidOperationException(
                        $"Unexpected state of {nameof(InseyeCalibrationState.NotStarted)}.");
                default:
                    throw new System.NotImplementedException("Unexpected state");
            }
        }

        private void InvokeOnCalibrationFinished(InseyeCalibrationResult calibrationResult, string? errorMessage)
        {
#if DEBUG_INSEYE_SDK
            Debug.Log($"Calibration finished, status: {calibrationResult:G}");
#endif
            _onCalibrationFinished?.Invoke(calibrationResult, errorMessage);
        }

        /// <summary>
        ///     List of animations used during calibration procedure.
        /// </summary>
        [SerializeField]
#pragma warning disable CS0628
        protected AnimationHelpers.CalibrationAnimation[] animations = null!;

        /// <summary>
        ///     Current/initial index of played animation.
        /// </summary>
        [SerializeField]
        [Tooltip("Initial animation index")]
        protected int animationIndex;

        /// <summary>
        ///     Transform that guides animation where calibration point should be moved.
        /// </summary>
        [SerializeField]
        [Tooltip("Game object that guides animated point to its destination position.")]
        protected Transform calibrationPointDestination = null!;

        /// <summary>
        ///     Object used as calibration point.
        /// </summary>
        [SerializeField]
        protected Transform calibrationPoint = null!;

        /// <summary>
        ///     Text UI where instruction are presented
        /// </summary>
        [SerializeField]
        [Tooltip("Optional text to display before calibration.")]
        protected TextMeshProUGUI displayText = null!;

        /// <summary>
        ///     Distance from camera to calibration point.
        /// </summary>
        [SerializeField]
        private float distanceFromCamera = 100f;
#pragma warning restore CS0628
    }
}