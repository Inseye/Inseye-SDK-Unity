// Module name: com.inseye.unity.sdk.samples.calibration
// File name: InstantPointMove.cs
// Last edit: 2023-10-05 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using UnityEngine;

namespace Inseye.Samples.Calibration.AnimationHelpers
{
    /// <summary>
    ///     Instantly moves point to destination
    /// </summary>
    public sealed class InstantPointMove : CalibrationAnimation
    {
        /// <summary>
        ///     Inseye calibration reference.
        /// </summary>
        [SerializeField]
#pragma warning disable CS0628
        protected Transform destinationPosition;
#pragma warning restore CS0628

        /// <summary>
        ///     Moves point to desired location.
        /// </summary>
        public override void EnterAnimation()
        {
            transform.position = destinationPosition.position;
        }

        /// <summary>
        ///     Does nothing.
        /// </summary>
        /// <param name="deltaTime">
        ///     <inheritdoc cref="Inseye.Samples.Calibration.AnimationHelpers.CalibrationAnimation.Animate" />
        /// </param>
        public override void Animate(float deltaTime)
        {
        }
    }
}