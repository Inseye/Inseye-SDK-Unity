// Module name: com.inseye.unity.sdk.samples.calibration
// File name: DelayAnimation.cs
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
    ///     Empty animation.
    /// </summary>
    public sealed class DelayAnimation : CalibrationAnimation
    {
        /// <summary>
        ///     How long delay should take.
        /// </summary>
        [SerializeField]
#pragma warning disable CS0628
        protected float delay;
#pragma warning restore CS0628

        private float timeElapsed;

        /// <summary>
        ///     Registers start of delay.
        /// </summary>
        public override void EnterAnimation()
        {
            timeElapsed = 0.0f;
            HasFinished = false;
        }

        /// <summary>
        ///     Checks if delayed time has passed.
        /// </summary>
        /// <param name="deltaTime">
        ///     <inheritdoc cref="Inseye.Samples.Calibration.AnimationHelpers.CalibrationAnimation.Animate" />
        /// </param>
        public override void Animate(float deltaTime)
        {
            timeElapsed += deltaTime;
            if (timeElapsed >= delay)
                HasFinished = true;
        }
    }
}