// Module name: com.inseye.unity.sdk.samples.mock
// File name: InseyeEyeTrackerInputSystemMockCustomEditor.cs
// Last edit: 2023-11-17 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Inseye.Samples.Mocks.Editor
{
    [CustomEditor(typeof(InseyeEyeTrackerInputSystemMock))]
    public sealed class InseyeEyeTrackerInputSystemMockCustomEditor : UnityEditor.Editor
    {
        private static readonly GUIContent InputActionLabel = new("InputAction");
        private static readonly GUIContent ControllerDirectionLabel = new("Controller direction");
        private static readonly GUIContent ControllerPositionLabel = new("Controller position");
        private static readonly List<InseyeEyeTrackerAvailability> AvailabilityValues;
        private static readonly List<Eyes> EyeValues;

        static InseyeEyeTrackerInputSystemMockCustomEditor()
        {
            var avails = Enum.GetValues(typeof(InseyeEyeTrackerAvailability));
            AvailabilityValues = new List<InseyeEyeTrackerAvailability>(avails.Length);
            for (int i = 0; i < avails.Length; i++)
            {
                AvailabilityValues.Add((InseyeEyeTrackerAvailability) avails.GetValue(i));
            }
            var eyes = Enum.GetValues(typeof(Eyes));
            EyeValues = new List<Eyes>(eyes.Length);
            for (int i = 0; i < eyes.Length; i++)
            {
                EyeValues.Add((Eyes) eyes.GetValue(i));
            }
        }
        public override void OnInspectorGUI()
        {
            var mock = target as InseyeEyeTrackerInputSystemMock;
            using (new LocalizationGroup(target))
            {
                EditorGUI.BeginChangeCheck();
                serializedObject.UpdateIfRequiredOrScript();
                using (new EditorGUI.DisabledScope(true))
                {
                    var scriptSerializedPropertyPath = serializedObject.FindProperty("m_Script");
                    EditorGUILayout.PropertyField(scriptSerializedPropertyPath, true);
                }

                if (mock is null)
                    return;
                {
                    var inputTypeSerializedProperty =
                        serializedObject.FindProperty(nameof(InseyeEyeTrackerInputSystemMock.inputType));
                    EditorGUILayout.PropertyField(inputTypeSerializedProperty, true);
                }
                {
                    var mainInput = serializedObject.FindProperty(nameof(InseyeEyeTrackerInputSystemMock.inputSource));
                    switch (mock.inputType)
                    {
                        case InseyeEyeTrackerInputSystemMock.InputType.Delta
                            or InseyeEyeTrackerInputSystemMock.InputType.ScreenPosition:
                        {
                            EditorGUILayout.PropertyField(mainInput, InputActionLabel);
                            break;
                        }
                        case InseyeEyeTrackerInputSystemMock.InputType.ControllerPositionAndDirection:
                        {
                            var auxInput =
                                serializedObject.FindProperty(nameof(InseyeEyeTrackerInputSystemMock
                                    .additionalInputSource));
                            EditorGUILayout.PropertyField(mainInput, ControllerDirectionLabel);
                            EditorGUILayout.PropertyField(auxInput, ControllerPositionLabel);
                            var distanceFiled =
                                serializedObject.FindProperty(nameof(InseyeEyeTrackerInputSystemMock
                                    .virtualDistanceFromCamera));
                            EditorGUILayout.PropertyField(distanceFiled);
                            break;
                        }
                    }
                }
                {
                    var inputTypeSerializedProperty =
                        serializedObject.FindProperty(nameof(InseyeEyeTrackerInputSystemMock.openedEyes));
                    EditorGUILayout.PropertyField(inputTypeSerializedProperty, true);
                }
                InseyeEyeTrackerAvailability availability;
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Eyetracker availability");
                    var prop = serializedObject.FindProperty(nameof(InseyeEyeTrackerInputSystemMock.availability));
                    
                    InseyeEyeTrackerAvailability currentAvailability;
                    try
                    {
                        currentAvailability = AvailabilityValues[prop.enumValueIndex];
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        currentAvailability = InseyeEyeTrackerAvailability.Available;
                    }
                    availability =
                        (InseyeEyeTrackerAvailability) EditorGUILayout.EnumPopup(currentAvailability);

                    prop.enumValueIndex = AvailabilityValues.IndexOf(availability);
                    EditorGUILayout.EndHorizontal();
                }
                Eyes eyes;
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Most accurate eye");
                    var prop = serializedObject.FindProperty(nameof(InseyeEyeTrackerInputSystemMock
                        .currentMostAccurateEye));
                    Eyes currentAccurateEye;
                    try
                    {
                        currentAccurateEye = EyeValues[prop.enumValueIndex];
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        currentAccurateEye = Eyes.Both;
                    }
                    eyes =
                        (Eyes) EditorGUILayout.EnumPopup(currentAccurateEye);
                    prop.enumValueIndex = EyeValues.IndexOf(eyes);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.EndChangeCheck();
                if (Application.isPlaying) // if called in editor then all changes are lost
                {
                    mock.SetEyeTrackerAvailability(availability);
                    mock.SetMostAccurateEye(eyes);
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif