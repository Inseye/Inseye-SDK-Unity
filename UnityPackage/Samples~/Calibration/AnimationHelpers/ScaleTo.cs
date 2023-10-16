// Module name: com.inseye.unity.sdk.samples.calibration
// File name: ScaleTo.cs
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
    ///     Scales object to desired size.
    /// </summary>
    public sealed class ScaleTo : CalibrationAnimation
    {
        private float speed;

        /// <summary>
        ///     Calculates speed of scale change.
        /// </summary>
        public override void EnterAnimation()
        {
            speed = animationTransform.GetSpeed(Vector3.Distance(transform.localScale, endValue));
            HasFinished = false;
        }

        /// <summary>
        ///     Scales object to desired value.
        /// </summary>
        /// <param name="deltaTime">
        ///     <inheritdoc cref="Inseye.Samples.Calibration.AnimationHelpers.CalibrationAnimation.Animate" />
        /// </param>
        public override void Animate(float deltaTime)
        {
            var newScale = Vector3.MoveTowards(transform.localScale, endValue, deltaTime * speed);
            if (newScale == endValue)
                HasFinished = true;
            transform.localScale = newScale;
        }

        /// <summary>
        ///     Type of animation.
        /// </summary>
        [SerializeField]
#pragma warning disable CS0628
        protected ValueTransform animationTransform;

        /// <summary>
        ///     Object local scale at the end of animation.
        /// </summary>
        [SerializeField]
        protected Vector3 endValue;
#pragma warning restore CS0628
    }
}