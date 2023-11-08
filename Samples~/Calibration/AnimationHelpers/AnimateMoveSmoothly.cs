// Module name: com.inseye.unity.sdk.samples.calibration
// File name: AnimateMoveSmoothly.cs
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
    ///     Moves object smoothly (with constant speed) towards points stored in
    ///     <see cref="Inseye.Samples.Calibration.InseyeCalibration.MoveDestinationPoint" />
    ///     and scales it back object reference scale over time.
    /// </summary>
    public sealed class AnimateMoveSmoothly : CalibrationAnimation
    {
        private float _animationMoveSpeed;
        private float _animationScaleSpeed;
        private Vector3 _referenceScale;

        private void Awake()
        {
            _referenceScale = transform.localScale;
        }

        /// <summary>
        ///     Calculates speed for position and scale change.
        /// </summary>
        public override void EnterAnimation()
        {
            var initialPosition = transform.position;
            _animationMoveSpeed =
                moveTransform.GetSpeed(Vector3.Distance(initialPosition, destinationPosition.position));
            _animationScaleSpeed = scaleTransform.GetSpeed(Vector3.Distance(_referenceScale, transform.localScale));
            HasFinished = false;
        }

        /// <summary>
        ///     Moves and scales point instantly to desired values.
        /// </summary>
        public override void ExitAnimation()
        {
            var trans = transform;
            trans.position = destinationPosition.position;
            trans.localScale = _referenceScale;
            HasFinished = true;
        }

        /// <summary>
        ///     Step of smooth move to destination position.
        /// </summary>
        /// <param name="deltaTime">
        ///     <inheritdoc cref="Inseye.Samples.Calibration.AnimationHelpers.CalibrationAnimation.Animate" />
        /// </param>
        public override void Animate(float deltaTime)
        {
            var trans = transform;
            var destinationPositionPosition = destinationPosition.position;
            var newPosition =
                Vector3.MoveTowards(trans.position, destinationPositionPosition, _animationMoveSpeed * deltaTime);
            var newScale = Vector3.MoveTowards(transform.localScale, _referenceScale, _animationScaleSpeed * deltaTime);
            if (newPosition == destinationPositionPosition && newScale == _referenceScale)
                HasFinished = true;
            trans.position = newPosition;
            trans.localScale = newScale;
        }

        /// <summary>
        ///     Current calibration reference.
        /// </summary>
        [SerializeField]
#pragma warning disable CS0628
        protected Transform destinationPosition;

        /// <summary>
        ///     Type of animation form position change.
        /// </summary>
        [SerializeField]
        protected ValueTransform moveTransform;

        /// <summary>
        ///     Type of animation for scale change.
        /// </summary>
        [SerializeField]
        protected ValueTransform scaleTransform;
#pragma warning restore CS0628
    }
}