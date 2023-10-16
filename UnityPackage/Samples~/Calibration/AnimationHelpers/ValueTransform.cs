// Module name: com.inseye.unity.sdk.samples.calibration
// File name: ValueTransform.cs
// Last edit: 2023-10-05 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using UnityEngine;

namespace Inseye.Samples.Calibration.AnimationHelpers
{
    /// <summary>
    ///     Allows defining if animation should have constant duration or animated value should be changed with constant delta.
    /// </summary>
    [Serializable]
    public struct ValueTransform
    {
        /// <summary>
        ///     Type of value transformation
        /// </summary>
        public enum TransformType
        {
            /// <summary>
            ///     Value animation speed doesn't depend on animation duration.
            /// </summary>
            DeterminedSpeed,

            /// <summary>
            ///     Value animation speed does depend on animation duration.
            /// </summary>
            DeterminedDuration
        }

        [SerializeField]
        private TransformType type;

        [SerializeField]
        private float value;

        /// <summary>
        ///     Type of value transform
        /// </summary>
        public TransformType Type => type;

        /// <summary>
        ///     Speed value or duration value depending on value chosen in <see cref="Type" />
        /// </summary>
        public float Value => value;

        /// <summary>
        ///     Calculates speed of change for animated value.
        /// </summary>
        /// <param name="distance">Value change that should be made over time.</param>
        /// <returns>Speed of change.</returns>
        public float GetSpeed(float distance)
        {
            if (type == TransformType.DeterminedSpeed)
                return value;
            return distance / value;
        }
    }
}