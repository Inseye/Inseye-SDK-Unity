// Module name: com.inseye.unity.sdk
// File name: JavaAndroidCallback.cs
// Last edit: 2023-11-06 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc. - All rights reserved.
// 
// All information contained herein is, and remains the property of
// Inseye Inc. The intellectual and technical concepts contained herein are
// proprietary to Inseye Inc. and may be covered by U.S. and Foreign Patents, patents
// in process, and are protected by trade secret or copyright law. Dissemination
// of this information or reproduction of this material is strictly forbidden
// unless prior written permission is obtained from Inseye Inc. Access to the source
// code contained herein is hereby forbidden to anyone except current Inseye Inc.
// employees, managers or contractors who have executed Confidentiality and
// Non-disclosure agreements explicitly covering such access.

using UnityEngine;
using UnityEngine.Scripting;

namespace Inseye.Android.Internal.JavaInterop
{
    public abstract class JavaAndroidCallback : MonoBehaviour, IJavaAndroidCallbackReceiver
    {
        [Preserve]
        void IJavaAndroidCallbackReceiver.InvokeSDKStateChanged(string parsableString)
        {
            if (int.TryParse(parsableString, out int parsed))
                OnSDKStateChanged((InseyeSDKState) parsed);
            else
                Debug.LogError($"Failed to parse availability change event: {parsableString}");
        }
        
        [Preserve]
        void IJavaAndroidCallbackReceiver.InvokeEyeTrackerAvailabilityChanged(string parsableString)
        {
            if (int.TryParse(parsableString, out int parsed))
                OnAvailabilityChanged((InseyeEyeTrackerAvailability) parsed);
            else
                Debug.LogError($"Failed to parse sdk state change event: {parsableString}");
        }

        protected virtual void OnSDKStateChanged(InseyeSDKState sdkState) { }
        protected virtual void OnAvailabilityChanged(InseyeEyeTrackerAvailability availability) { }
    }
}