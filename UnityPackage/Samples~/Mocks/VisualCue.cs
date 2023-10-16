// Module name: com.inseye.unity.sdk.samples.mock
// File name: VisualCue.cs
// Last edit: 2023-10-05 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using Inseye.Extensions;
using Inseye.Interfaces;
using UnityEngine;

namespace Inseye.Samples.Mocks
{
	/// <summary>
	///     A helper class that displays visible object on screen at the point of current gaze position.
	/// </summary>
	[RequireComponent(typeof(MeshRenderer))]
    public sealed class VisualCue : MonoBehaviour
    {
        private MeshRenderer _cueObjectRenderer;
        private IGazeProvider _gazeProvider;
#pragma warning disable CS0628
	    /// <summary>
	    ///     How far from camera visual cue should be displayed.
	    /// </summary>
	    [SerializeField]
        protected float distanceFromCamera;

	    /// <summary>
	    ///     Instantiates gaze provided.
	    /// </summary>
	    protected void Start()
        {
	        try
	        {
		        _gazeProvider = InseyeSDK.GetGazeProvider();
		        _cueObjectRenderer = GetComponent<MeshRenderer>();
	        }
	        finally
	        {
		        var finalEnabled = true;
		        if (_cueObjectRenderer is null)
		        {
			        Debug.Log("Missing MeshRenderer, disabling VisualCue.");
			        finalEnabled = false;
		        }

		        if (_gazeProvider is null)
		        {
			        Debug.Log("Gaze provider is null, disabling VisualCue.");
			        finalEnabled = false;
		        }
		        enabled = finalEnabled;
	        }
        }

	    /// <summary>
	    ///     Disposes gaze provider.
	    /// </summary>
	    protected void OnDestroy()
        {
            _gazeProvider?.Dispose();
        }

	    /// <summary>
	    ///     Updates position of visual cue.
	    /// </summary>
	    protected void Update()
#pragma warning restore CS0628
        {
            if (_gazeProvider != null && _gazeProvider.GetMostRecentGazeData(out var gazeData))
            {
                _cueObjectRenderer.enabled = true;
                var mainCamera = Camera.main;
                if (mainCamera != null)
                    transform.position = gazeData.ToWorldPoint(distanceFromCamera, mainCamera.transform);
            }
            else
            {
                _cueObjectRenderer.enabled = false;
            }
        }
    }
}