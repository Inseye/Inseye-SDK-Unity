// Module name: com.inseye.unity.sdk.samples.mock
// File name: InseyeMockSDKImplementation.cs
// Last edit: 2023-10-05 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using System.Collections.Generic;
using Inseye.Interfaces;
using Inseye.Internal;
using Inseye.Internal.Extensions;
using Inseye.Internal.Interfaces;
using UnityEngine;

namespace Inseye.Samples.Mocks
{
    /// <summary>
    ///     Base class for any mock SDK implementation.
    ///     Replaces original SKD in Awake.
    /// </summary>
    public abstract class InseyeMockSDKImplementation : MonoBehaviour, ISDKImplementation, ISDKEventsBroker
    {
        [SerializeField]
        internal InseyeEyeTrackerAvailability availability = InseyeEyeTrackerAvailability.Available;

        [SerializeField]
        internal Eyes currentMostAccurateEye = Eyes.Both;

        private readonly SDKStateManager _stateManager = new();

        /// <summary>
        ///     Field backing <see cref="ISDKEventsBroker.EyeTrackerAvailabilityChanged" /> event.
        /// </summary>
        private Action<InseyeEyeTrackerAvailability> _eyeTrackerAvailabilityChangedField;

        private IGazeDataSource _gazeDataSource;

        private Action<Eyes> _mostAccurateEyeChangedField;

        /// <summary>
        ///     Gaze data source.
        /// </summary>
        protected abstract IGazeDataSource InternalGazeDataSource { get; }

        private void Awake()
        {
            InseyeSDK.SwapSDKImplementation(this);
            InheritorAwake();
        }

        private void OnDestroy()
        {
            if (ReferenceEquals(InseyeSDK.CurrentImplementation, this))
                InseyeSDK.SwapSDKImplementation();
        }

        /// <inheritdoc cref="InseyeSDK.EyeTrackerAvailabilityChanged" />
        event Action<InseyeEyeTrackerAvailability> ISDKEventsBroker.EyeTrackerAvailabilityChanged
        {
            add => _eyeTrackerAvailabilityChangedField += value;
            remove => _eyeTrackerAvailabilityChangedField -= value;
        }

        event Action<Eyes> ISDKEventsBroker.MostAccurateEyeChanged
        {
            add => _mostAccurateEyeChangedField += value;
            remove => _mostAccurateEyeChangedField -= value;
        }

        void ISDKEventsBroker.TransferListenersTo(ISDKEventsBroker target)
        {
            target.MostAccurateEyeChanged += _mostAccurateEyeChangedField;
            _mostAccurateEyeChangedField = null;
            target.EyeTrackerAvailabilityChanged += _eyeTrackerAvailabilityChangedField;
            _eyeTrackerAvailabilityChangedField = null;
        }

        public void InvokeEyeTrackerAvailabilityChanged(InseyeEyeTrackerAvailability availability)
        {
            _eyeTrackerAvailabilityChangedField?.Invoke(availability);
        }

        ISDKEventsBroker ISDKImplementation.EventBroker => this;

        IGazeDataSource ISDKImplementation.GazeDataSource => InternalGazeDataSource;
        ISDKStateManager ISDKImplementation.SDKStateManager => _stateManager;

        IDisposable ISDKImplementation.KeepEyeTrackerInitialized()
        {
            var keeper = new EyeTrackerKeepaliveHandle();
            ((ISDKStateManager) _stateManager).RequireState(keeper);
            return keeper;
        }

        IGazeProvider ISDKImplementation.GetGazeProvider()
        {
            var gazeProvider = new InseyeGazeProvider();
            ((ISDKStateManager) _stateManager).RequireState(gazeProvider);
            return gazeProvider;
        }

        /// <summary>
        ///     Starts mock calibration procedure.
        /// </summary>
        /// <returns>Calibration procedure.</returns>
        public ICalibrationProcedure StartCalibration()
        {
            return new MockCalibrationProcedure();
        }

        public InseyeEyeTrackerAvailability GetEyeTrackerAvailability()
        {
            return availability;
        }

        public IReadOnlyDictionary<InseyeSDKComponent, InseyeComponentVersion> GetVersions()
        {
            return new Dictionary<InseyeSDKComponent, InseyeComponentVersion>
            {
                {InseyeSDKComponent.UnitySDK, InseyeSDK.SDKVersion}
            };
        }


        /// <inheritdoc cref="Inseye.InseyeSDK.GetMostAccurateEye" />
        public Eyes GetMostAccurateEye()
        {
            return currentMostAccurateEye;
        }

        public bool IsCalibrated()
        {
            return true;
        }

        public void Dispose()
        {
            _stateManager.Dispose();
        }

        /// <summary>
        ///     Starts mock calibration procedure using custom calibration pattern.
        /// </summary>
        /// <param name="calibrationPattern">Calibration pattern.</param>
        /// <returns>Calibration procedure.</returns>
        public ICalibrationProcedure StartCalibration(CalibrationPoint[] calibrationPattern)
        {
            return new MockCalibrationProcedure(calibrationPattern);
        }

        /// <summary>
        ///     Overridable method called during Awake
        /// </summary>
        protected virtual void InheritorAwake()
        {
        }

        public void SetEyeTrackerAvailability(InseyeEyeTrackerAvailability availability)
        {
            if (this.availability == availability)
                return;
            this.availability = availability;
            _eyeTrackerAvailabilityChangedField?.SafeInvoke(availability);
        }

        /// <summary>
        ///     Sets most accurate eye. Invokes event if value is different from previous.
        /// </summary>
        /// <param name="eyes">Most accurate eye</param>
        public void SetMostAccurateEye(Eyes eyes)
        {
            if (eyes == currentMostAccurateEye)
                return;
            currentMostAccurateEye = eyes;
            _mostAccurateEyeChangedField?.Invoke(currentMostAccurateEye);
        }
    }
}