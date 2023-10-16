// Module name: com.inseye.unity.sdk.samples.calibration
// File name: AnimateIdle.cs
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
    ///     Animation where object is periodically linearly changing its scale from <see cref="minScale" /> to
    ///     <see cref="maxScale" /> and back.
    /// </summary>
    public sealed class AnimateIdle : CalibrationAnimation
    {
        private bool _increaseSize;

        /// <summary>
        ///     Checks if animated object should increase or reduce its scale.
        /// </summary>
        public override void EnterAnimation()
        {
            _increaseSize = AnyComponentOver(transform.localScale, maxScale);
        }

        /// <summary>
        ///     Increases and decreases object scale.
        /// </summary>
        /// <param name="deltaTime">
        ///     <inheritdoc cref="Inseye.Samples.Calibration.AnimationHelpers.CalibrationAnimation.Animate" />
        /// </param>
        public override void Animate(float deltaTime)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, _increaseSize ? maxScale : minScale,
                deltaTime * scaleAnimationSpeed);
            if (_increaseSize && AnyComponentOver(transform.localScale, maxScale))
                _increaseSize = false;
            if (!_increaseSize && AnyComponentOver(minScale, transform.localScale))
                _increaseSize = true;
        }

        private static bool AnyComponentOver(Vector3 checkedComponent, Vector3 threshold)
        {
            return checkedComponent.x >= threshold.x || checkedComponent.y >= threshold.y ||
                   checkedComponent.z >= threshold.z;
        }

        /// <summary>
        ///     Maximum object scale.
        /// </summary>
        [SerializeField]
#pragma warning disable CS0628
        protected Vector3 maxScale;

        /// <summary>
        ///     Minimum object scale.
        /// </summary>
        [SerializeField]
        protected Vector3 minScale;

        /// <summary>
        ///     Scale change speed.
        /// </summary>
        [SerializeField]
        protected float scaleAnimationSpeed;
#pragma warning restore CS0628
    }
}