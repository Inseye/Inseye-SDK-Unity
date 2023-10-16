// Module name: com.inseye.unity.sdk
// File name: InseyeGazeData.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Text;
using Inseye.Internal;
using Inseye.Internal.Extensions;
using UnityEngine;

namespace Inseye
{
    /// <summary>
    ///     Gaze data structure available to SDK user.
    /// </summary>
    public struct InseyeGazeData
    {
        /// <summary>
        ///     Recording timestamp in milliseconds UTC.
        /// </summary>
        public long TimeMilliseconds;

        /// <summary>
        ///     Left eye direction in radians.
        ///     Angle is measurement of rotation made with conjunction of rotation around headset up axis by 'x' and rotation
        ///     around headset left axis by 'y'.
        ///     'x' value should be in range of (-half of device horizontal field of view, half of device horizontal field of view)
        ///     where positive value represent position in right part of user field of view and negative values corresponds to left
        ///     part of field of view.
        ///     'y' value should be in range of (-half of device vertical field of view, half of device vertical field of view)
        ///     where positive value represent position in upper part of user field of view and negative values corresponds to
        ///     lower part of field of view.
        /// </summary>
        public Vector2 LeftEyePosition;

        /// <summary>
        ///     Right eye direction in radians.
        ///     Angle is measurement of rotation made with conjunction of rotation around headset up axis by 'x' and rotation
        ///     around headset left axis by 'y'.
        ///     'x' value should be in range of (-half of device horizontal field of view, half of device horizontal field of view)
        ///     where positive value represent position in right part of user field of view and negative values corresponds to left
        ///     part of field of view.
        ///     'y' value should be in range of (-half of device vertical field of view, half of device vertical field of view)
        ///     where positive value represent position in upper part of user field of view and negative values corresponds to
        ///     lower part of field of view.
        /// </summary>
        public Vector2 RightEyePosition;

        /// <summary>
        ///     Eye tracker events.
        /// </summary>
        public InseyeGazeEvent EyeTrackerEvents;

        internal InseyeGazeData(GazeData source)
        {
            TimeMilliseconds = source.timeMilliseconds;
            LeftEyePosition = new Vector2(source.left_x, source.left_y);
            RightEyePosition = new Vector2(source.right_x, source.right_y);
            EyeTrackerEvents = source.ConvertEvent();
        }

        /// <summary>
        ///     Prints data structure in format ts: TimeMilliseconds, left: (x, y), right: (x, y), events: event_0, event_1 ...
        /// </summary>
        /// <returns>Formated string representation.</returns>
        public override string ToString()
        {
            // TODO: Consider escaping form string builder allocation
            var sb = new StringBuilder("ts: ");
            sb.Append(TimeMilliseconds);
            sb.Append(", left: (").Append(LeftEyePosition.x).Append(", ").Append(LeftEyePosition.y);
            sb.Append("), right: (").Append(RightEyePosition.x).Append(", ").Append(RightEyePosition.y);
            sb.Append("), events: ").Append(EyeTrackerEvents.ToString("G"));
            return sb.ToString();
        }

        /// <summary>
        ///     Same as <see cref="ToString" /> but left and right rotation is in degrees.
        /// </summary>
        /// <returns>Formated string representation.</returns>
        public string ToStringDegrees()
        {
            var sb = new StringBuilder("ts: ");
            sb.Append(TimeMilliseconds);
            sb.Append(", left: (").Append(LeftEyePosition.x * Mathf.Rad2Deg).Append(", ")
                .Append(LeftEyePosition.y * Mathf.Rad2Deg);
            sb.Append("), right: (").Append(RightEyePosition.x * Mathf.Rad2Deg).Append(", ")
                .Append(RightEyePosition.y * Mathf.Rad2Deg);
            sb.Append("), events: ").Append(EyeTrackerEvents.ToString("G"));
            return sb.ToString();
        }
    }
}