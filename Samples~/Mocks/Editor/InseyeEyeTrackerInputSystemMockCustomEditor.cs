// Module name: com.inseye.unity.sdk.samples.mock
// File name: InseyeEyeTrackerInputSystemMockCustomEditor.cs
// Last edit: 2023-10-05 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

#if UNITY_EDITOR
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
                Eyes eyes;
                InseyeEyeTrackerAvailability availability;
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Eyetracker availability");
                    var prop = serializedObject.FindProperty(nameof(InseyeEyeTrackerInputSystemMock.availability));
                    var currentAvailability = (InseyeEyeTrackerAvailability) prop.enumValueIndex;
                    availability =
                        (InseyeEyeTrackerAvailability) EditorGUILayout.EnumPopup(currentAvailability);
                    prop.enumValueIndex = (int) availability;
                    EditorGUILayout.EndHorizontal();
                }
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Most accurate eye");
                    var prop = serializedObject.FindProperty(nameof(InseyeEyeTrackerInputSystemMock
                        .currentMostAccurateEye));
                    var currentAccurateEye = (Eyes) prop.enumValueIndex;
                    eyes =
                        (Eyes) EditorGUILayout.EnumPopup(currentAccurateEye);
                    prop.enumValueIndex = (int) eyes;
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