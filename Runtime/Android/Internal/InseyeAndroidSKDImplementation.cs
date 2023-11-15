// Module name: com.inseye.unity.sdk
// File name: InseyeAndroidSKDImplementation.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

#nullable enable
using System;
using System.Collections.Generic;
using Inseye.Android.Internal.JavaInterop;
using Inseye.Exceptions;
using Inseye.Interfaces;
using Inseye.Internal;
using Inseye.Internal.Extensions;
using Inseye.Internal.Interfaces;
using UnityEngine;

namespace Inseye.Android.Internal
{
    // ReSharper disable once InconsistentNaming
    internal sealed class InseyeAndroidSKDImplementation : ISDKImplementation, ISDKEventsBroker, ISDKStateManager
    {
        public enum Transition
        {
            None,
            Enter,
            Exit
        }

        private readonly JavaLibrary _javaLibrary = new();
        private readonly PinnedInt _pinnedStateInt = new();
        private bool _androidMessageListenerContributesState;
        private Eyes _dominantEye;
        private IGazeDataSource _inseyeIGazeDataSource = EmptyInseyeIGazeDataSource.Instance;
        private InseyeGazeData _internalGazeData;

        private static readonly (InseyeComponentVersion minimum, InseyeComponentVersion maximum) VersionConstraints =
            new(new InseyeComponentVersion(0, 13, 0), new InseyeComponentVersion(1, 0, 0));

        public InseyeAndroidSKDImplementation()
        {
            Application.focusChanged += FocusChangedHandler;
#if DEBUG_INSEYE_SDK
            _javaLibrary.SetLogLevel(JavaLibrary.LogLevel.Debug);
#endif
        }

        event Action<InseyeEyeTrackerAvailability> ISDKEventsBroker.EyeTrackerAvailabilityChanged
        {
            add
            {
                AndroidMessageListener.AddListener(value);
                SubscribeToEyetrackerEvents();
            }
            remove
            {
                AndroidMessageListener.RemoveListener(value);
                OptionallyUnsubscribeFromEyetrackerEvents();
            }
        }

        public event Action<Eyes>? MostAccurateEyeChanged;

        public void TransferListenersTo(ISDKEventsBroker target)
        {
            var listeners = AndroidMessageListener.RemoveAllListeners();
            target.EyeTrackerAvailabilityChanged += listeners;
            target.MostAccurateEyeChanged += MostAccurateEyeChanged;
            MostAccurateEyeChanged = null;
        }

        public void InvokeEyeTrackerAvailabilityChanged(InseyeEyeTrackerAvailability availability)
        {
            AndroidMessageListener.Invoke(availability);
        }

        /// <inheritdoc cref="ISDKImplementation.EventBroker" />
        ISDKEventsBroker ISDKImplementation.EventBroker => this;

        IGazeDataSource ISDKImplementation.GazeDataSource => _inseyeIGazeDataSource;

        IDisposable ISDKImplementation.KeepEyeTrackerInitialized()
        {
            var initializationKeeper = new EyeTrackerKeepaliveHandle();
            try
            {
                ((ISDKStateManager) this).RequireState(initializationKeeper);
                return initializationKeeper;
            }
            catch
            {
                initializationKeeper.Dispose();
                throw;
            }
        }

        IGazeProvider ISDKImplementation.GetGazeProvider()
        {
            var gazeProvider = new InseyeGazeProvider();
            try
            {
                ((ISDKStateManager) this).RequireState(gazeProvider);
                return gazeProvider;
            }
            catch
            {
                gazeProvider.Dispose();
                throw;
            }
        }

        ICalibrationProcedure ISDKImplementation.StartCalibration()
        {
            if ((InseyeSDKState & InseyeSDKState.Calibration) == InseyeSDKState.Calibration)
                throw new SDKCalibrationException("Another calibration is ongoing");
            // initialize if needed
            if ((InseyeSDKState & InseyeSDKState.Initialized) != InseyeSDKState.Initialized)
                TransitionToState(InseyeSDKState.Initialized);
#if DEBUG_INSEYE_SDK
            Debug.Log(
                $"{nameof(InseyeAndroidSKDImplementation)}::{nameof(ISDKImplementation.StartCalibration)}: Creating new AndroidCalibrationProcedure");
#endif
            var dataHandler = new CalibrationDataHandle();
            AndroidCalibrationProcedure calibrationProcedure = default;
            try
            {
                var javaObjectProxy = _javaLibrary.StartCalibrationProcedure(dataHandler.GetCalibrationPointRequestPointer(),
                    dataHandler.GetCalibrationPointResponsePointer(),
                    dataHandler.GetCalibrationStatusPointer(), dataHandler.GetPointCounterPointer());
                calibrationProcedure = new AndroidCalibrationProcedure(dataHandler, javaObjectProxy);
                ((ISDKStateManager) this).RequireState(calibrationProcedure);
#if DEBUG_INSEYE_SDK
                Debug.Log(
                    $"{nameof(InseyeAndroidSKDImplementation)}::{nameof(ISDKImplementation.StartCalibration)}: Started calibration procedure");
#endif
                return calibrationProcedure;
            }
            catch(Exception exception)
            {
                throw new SDKCalibrationException(
                    $"Unexpected result of {nameof(ISDKImplementation.StartCalibration)}", exception);
            }
        }

        InseyeEyeTrackerAvailability ISDKImplementation.GetEyeTrackerAvailability()
        {
            TransitionToState(InseyeSDKState | InseyeSDKState.Initialized); // temp enter state if needed
            try
            {
                return _javaLibrary.GetEyeTrackerAvailability();
            }
            finally
            {
                CheckState(); // validate state
            }
        }

        public IReadOnlyDictionary<InseyeSDKComponent, InseyeComponentVersion> GetVersions()
        {
            IReadOnlyDictionary<InseyeSDKComponent, InseyeComponentVersion> GetVersionsInternal()
            {
                var (serviceVersion, firmwareVersion) = _javaLibrary.GetVersions();
                var returnedDictionary = new Dictionary<InseyeSDKComponent, InseyeComponentVersion>
                {
                    {InseyeSDKComponent.AndroidService, serviceVersion},
                    {InseyeSDKComponent.UnitySDK, InseyeSDK.SDKVersion}
                };
                if (firmwareVersion.Major == 0 && firmwareVersion.Minor == 0 &&
                    firmwareVersion.Patch == 0) // missing firmware version
                    return returnedDictionary;
                returnedDictionary[InseyeSDKComponent.BoardFirmware] = firmwareVersion;
                return returnedDictionary;
            }

            if ((InseyeSDKState & InseyeSDKState.Initialized) == InseyeSDKState.Initialized)
                return GetVersionsInternal();
            TransitionToState(InseyeSDKState | InseyeSDKState.Initialized); // temp enter state if needed
            try
            {
                return GetVersionsInternal();
            }
            finally
            {
                CheckState();
            }
        }

        public Eyes GetMostAccurateEye()
        {
            return _dominantEye;
        }

        private void SubscribeToEyetrackerEvents()
        {
            try
            {
                if (!_androidMessageListenerContributesState)
                {
                    ((ISDKStateManager) this).RequireState(AndroidMessageListener.Instance);
                    _androidMessageListenerContributesState = true;
                }
            }
            catch (SDKInitializationException exception)
            {
                switch (exception.FailReason)
                {
                    case SDKInitializationException.Reason.UnableToConnectToService:
                    case SDKInitializationException.Reason.InvalidServiceVersion:
                        break;
                    default:
                        Debug.LogException(exception);
                        break;
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                Debug.LogError("Unknown exception occured during subscription to eye tracker events");
                throw;
            }
        }

        private void OptionallyUnsubscribeFromEyetrackerEvents()
        {
            try
            {
                if (!AndroidMessageListener.Instantiated) return;
                if (AndroidMessageListener.SubscribersCount == 0 && _androidMessageListenerContributesState)
                {
                    ((ISDKStateManager) this).RemoveListener(AndroidMessageListener.Instance);
                    _androidMessageListenerContributesState = false;
                }
            }
            catch (Exception)
            {
                Debug.LogError("SDK failed to switch state after last unsubscribe from events");
                throw;
            }
        }

        private void CheckDominantEye()
        {
            Eyes dominantEye;
            dominantEye = (InseyeSDKState & InseyeSDKState.Initialized) != InseyeSDKState.Initialized
                ? Eyes.Both
                : _javaLibrary.GetDominantEyes();
            if (dominantEye == _dominantEye)
                return;
            _dominantEye = dominantEye;
            MostAccurateEyeChanged?.SafeInvoke(_dominantEye);
        }

        private void FocusChangedHandler(bool hasFocus)
        {
            if (!hasFocus)
                return;
#if DEBUG_INSEYE_SDK
            Debug.Log("Checking eye in response to focus change.");
#endif
            CheckDominantEye();
            CheckState();
        }

        private void CheckState()
        {
            if (_sdkStateManager.InseyeSDKState != InseyeSDKState)
                TransitionToState(_sdkStateManager.InseyeSDKState);
        }

        #region StateTransitionMethods

        private void EnterInitialized()
        {
#if DEBUG_INSEYE_SDK
            Debug.Log($"{nameof(InseyeAndroidSKDImplementation)}::{nameof(EnterInitialized)}: initializing");
#endif
            var initializationReturnCode = _javaLibrary.Initialize(_pinnedStateInt.GetValuePointer(), 2000);
            switch (initializationReturnCode)
            {
                case ErrorCodes.Successful:
                case ErrorCodes.SDKAlreadyConnected:
                    try
                    {
                        var versions = _javaLibrary.GetVersions();
                        if (versions.serviceVersion < VersionConstraints.minimum)
                            throw new SDKInitializationException(
                                $"Minimum required service version is {VersionConstraints.minimum}. Installed service has {versions.serviceVersion} version",
                                SDKInitializationException.Reason.InvalidServiceVersion);
                        if (versions.serviceVersion > VersionConstraints.maximum)
                            throw new SDKInitializationException(
                                $"Maximum service version supported by SDK is {VersionConstraints.maximum}. Installed service has {versions.serviceVersion} version");
                    }
                    catch (Exception exc)
                    {
                        throw new SDKInitializationException(
                            $"Failed to obtain service version. Service must have version between {VersionConstraints.minimum} and {VersionConstraints.maximum}.",
                            SDKInitializationException.Reason.InvalidServiceVersion, exc);
                    }

                    CheckDominantEye();
                    return;
                case ErrorCodes.UnknownErrorCheckErrorMessage:
                    throw new SDKInitializationException(
                        $"Failed to connect to java service, error message: {_javaLibrary.GetLastErrorMessage()}");
                default:
                    throw new SDKInitializationException(
                        $"Failed to connect to java service: {initializationReturnCode:F}",
                        SDKInitializationException.ReasonFromErrorCode(initializationReturnCode));
            }
        }

        private void ExitInitialized()
        {
            _javaLibrary.Dispose(_pinnedStateInt.GetValuePointer());
        }

        private void EnterGazeReadingState()
        {
#if DEBUG_INSEYE_SDK
            Debug.Log(
                $"{nameof(InseyeAndroidSKDImplementation)}::{nameof(EnterGazeReadingState)}: entering gaze reading state");
#endif
            ErrorCodes returnCode;
            returnCode = _javaLibrary.GetGazeDataStreamUDPPort(out var port);
            Exception? exc = null;
            switch (returnCode)
            {
                case ErrorCodes.Successful:
                    break;
                case ErrorCodes.UnknownErrorCheckErrorMessage:
                    exc = new SDKInternalException(_javaLibrary.GetLastErrorMessage());
                    break;
                default:
                    exc = new SDKInternalException($"Unexpected error code: {returnCode:G}");
                    break;
            }

            if (exc is not null)
                throw exc;
#if DEBUG_INSEYE_SDK
            Debug.Log($"Creating reader on port: {port}");
#endif
            _inseyeIGazeDataSource.Dispose();
            _inseyeIGazeDataSource = new InseyeIGazeDataSourceFromInseyeIGazeProvider(new UDPGazeReader(port));
#if DEBUG_INSEYE_SDK
            Debug.Log($"{nameof(InseyeAndroidSKDImplementation)}::{nameof(EnterGazeReadingState)}: gaze reading ready");
#endif
        }

        private void ExitGazeReadingState()
        {
            _inseyeIGazeDataSource.Dispose();
            _inseyeIGazeDataSource = EmptyInseyeIGazeDataSource.Instance;
            var errorCodes = _javaLibrary.StopGazeDataStream();
            switch (errorCodes)
            {
                case ErrorCodes.Successful:
                    break;
                case ErrorCodes.UnknownErrorCheckErrorMessage:
                    Debug.LogError(_javaLibrary.GetLastErrorMessage());
                    break;
                default:
                    throw new SDKInternalException($"Unexpected error message: {errorCodes:G}");
            }
        }

        private void TransitionToState(InseyeSDKState inseyeSDKState)
        {
            Transition RequiresTransition(InseyeSDKState checkedState, InseyeSDKState desiredState)
            {
#if DEBUG_INSEYE_SDK
                Debug.Log($"Current SDK state: {InseyeSDKState:G}, and desired {desiredState:G}");
#endif
                var currentlyToggled = (checkedState & InseyeSDKState) == checkedState;
                var requiresToggling = (checkedState & desiredState) == checkedState;
                if (currentlyToggled == requiresToggling)
                    return Transition.None;
                return requiresToggling ? Transition.Enter : Transition.Exit;
            }

            var initializeTransition = RequiresTransition(InseyeSDKState.Initialized, inseyeSDKState);
            if (initializeTransition == Transition.Enter)
                EnterInitialized();
            {
                var gazeReadingTransition =
                    RequiresTransition(InseyeSDKState.MostRecentGazePointAvailable, inseyeSDKState);
                if (gazeReadingTransition == Transition.Enter)
                    EnterGazeReadingState();
                else if (gazeReadingTransition == Transition.Exit)
                    ExitGazeReadingState();
            }
            {
                var eventTransition = RequiresTransition(InseyeSDKState.SubscribedToEyeTrackerEvents, inseyeSDKState);
                if (eventTransition == Transition.Enter)
                    EnterEventSubscribedState();
                else if (eventTransition == Transition.Exit)
                    ExitEventSubscribedState();
            }
            if (initializeTransition == Transition.Exit)
                ExitInitialized();
#if DEBUG_INSEYE_SDK
            Debug.Log(
                $"{nameof(InseyeAndroidSKDImplementation)}::{nameof(TransitionToState)}: current state {InseyeSDKState:G}");
#endif
        }


        private void EnterEventSubscribedState()
        {
            ErrorCodes errorCode;
            try
            {
                errorCode = _javaLibrary.SubscribeToEvents(AndroidMessageListener.Instance.name);
            }
            catch (AndroidJavaException androidJavaException)
            {
                throw new SDKInternalException(
                    "Unexpected android exception occured while subscribing to eye tracker events",
                    androidJavaException);
            }

            switch (errorCode)
            {
                case ErrorCodes.Successful:
                case ErrorCodes.AlreadySubscribedToEvents:
                    break;
                case ErrorCodes.UnknownErrorCheckErrorMessage:
                    throw new SDKInternalException(
                        _javaLibrary!.GetLastErrorMessage());
                default:
                    throw new SDKInternalException($"Invalid/unexpected error code: {errorCode:G}");
            }
        }

        private void ExitEventSubscribedState()
        {
            ErrorCodes errorCode;
            try
            {
                errorCode = _javaLibrary.UnsubscribeFromEvents();
            }
            catch (AndroidJavaException androidJavaException)
            {
                throw new SDKInternalException(
                    "Unexpected android exception occured while unsubscribing from eye tracker events",
                    androidJavaException);
            }

            switch (errorCode)
            {
                case ErrorCodes.Successful:
                case ErrorCodes.AlreadySubscribedToEvents:
                    break;
                case ErrorCodes.UnknownErrorCheckErrorMessage:
                    throw new SDKInternalException(
                        _javaLibrary.GetLastErrorMessage());
                default:
                    throw new SDKInternalException($"Invalid/unexpected error code: {errorCode:G}");
            }
        }

        #endregion

        #region ISDKStateManager

        private readonly ISDKStateManager _sdkStateManager = new SDKStateManager();
        public ISDKStateManager SDKStateManager => this;

        /// <summary>
        ///     SDK state controlled by java library.
        /// </summary>
        public InseyeSDKState InseyeSDKState => (InseyeSDKState) _pinnedStateInt.Value;

        void ISDKStateManager.RequireState(IStateUser stateUser)
        {
            // may throw and that is fine
            TransitionToState(stateUser.RequiredInseyeSDKState | _sdkStateManager.InseyeSDKState);
            _sdkStateManager.RequireState(stateUser);
        }

        void ISDKStateManager.RemoveListener(IStateUser user)
        {
            _sdkStateManager.RemoveListener(user);
            try
            {
                TransitionToState(_sdkStateManager.InseyeSDKState);
            }
            catch (Exception exception)
            {
#if DEBUG_INSEYE_SDK || DEVELOPMENT_BUILD
                Debug.LogError("While removing ISDKUser exception occured.");
                Debug.LogException(exception);
#endif
            }
        }

        void ISDKStateManager.RegisterMultipleUsers(IEnumerable<IStateUser> users)
        {
            _sdkStateManager.RegisterMultipleUsers(users);
            try
            {
                TransitionToState(_sdkStateManager.InseyeSDKState);
            }
            catch (Exception exception)
            {
#if DEBUG_INSEYE_SDK || DEVELOPMENT_BUILD
                Debug.LogError("While registering multiple ISDKUser(s) exception occured.");
                Debug.LogException(exception);
#endif
            }
        }

        void ISDKStateManager.TransferAllStateUsersTo(ISDKStateManager target)
        {
            _sdkStateManager.TransferAllStateUsersTo(target);
            TransitionToState(InseyeSDKState.Uninitialized);
        }

        #endregion

        #region IDisposable

        private bool _disposed;

        ~InseyeAndroidSKDImplementation()
        {
            Dispose(false);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            _sdkStateManager.Dispose();
            _disposed = true;
            if (disposing)
                GC.SuppressFinalize(this);
            try
            {
                if (disposing)
                    _javaLibrary.Dispose(_pinnedStateInt.GetValuePointer());
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                _pinnedStateInt.Dispose();
                Application.focusChanged -= FocusChangedHandler;
            }
        }

        #endregion
    }
}