// Module name: com.inseye.unity.sdk.samples.calibration
// File name: CalibrationAnimation.cs
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
    ///     Base class for all animations.
    /// </summary>
    public abstract class CalibrationAnimation : MonoBehaviour
    {
        /// <summary>
        ///     True if animation has finished, otherwise false.
        /// </summary>
        public bool HasFinished { get; protected set; }

        /// <summary>
        ///     Called by <see cref="Inseye.Samples.Calibration.InseyeCalibration" /> when animation ends.
        /// </summary>
        public virtual void EnterAnimation()
        {
        }

        /// <summary>
        ///     Called by <see cref="Inseye.Samples.Calibration.InseyeCalibration" /> when animation starts.
        /// </summary>
        public virtual void ExitAnimation()
        {
        }

        /// <summary>
        ///     Performs step of animation.
        /// </summary>
        /// <param name="deltaTime">Difference in time between previous and current call.</param>
        public abstract void Animate(float deltaTime);
    }
}