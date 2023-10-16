// Module name: com.inseye.unity.sdk
// File name: InseyeIGazeDataSourceFromInseyeIGazeProvider.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using Inseye.Interfaces;
using Inseye.Internal.Extensions;
using Inseye.Internal.Interfaces;
using UnityEngine;

namespace Inseye.Internal
{
    internal sealed class InseyeIGazeDataSourceFromInseyeIGazeProvider : IGazeDataSource, IVersionedSpan<GazeData>
    {
        private readonly IEyeTrackerReader _reader;
        private (int version, int elements, GazeData[] array) _gazeData = (-1, 0, new GazeData[50]);
        private (int version, InseyeGazeData[] array) _inseyeGazeData = (-3, System.Array.Empty<InseyeGazeData>());
        private (int version, InseyeGazeData gazeData) _mostRecentGazeData = (-2, new InseyeGazeData());

        internal InseyeIGazeDataSourceFromInseyeIGazeProvider(IEyeTrackerReader reader)
        {
            _reader = reader;
        }


        public InseyeGazeDataEnumerator GetEnumerator()
        {
            CheckAndUpdateDataArray();
            return new InseyeGazeDataEnumerator(new GazeDataEnumerator(Version, this));
        }

        public bool GetMostRecentGazeData(out InseyeGazeData gazeData)
        {
            CheckAndUpdateDataArray();
            if (_mostRecentGazeData.version != _gazeData.version)
            {
                if (_gazeData.elements == 0)
                {
                    gazeData = default;
                    return false;
                }

                gazeData.EyeTrackerEvents = InseyeGazeEvent.None;
                var array = _gazeData.array;
                for (var i = 0; i < _gazeData.elements; i++) gazeData.EyeTrackerEvents |= array[i].ConvertEvent();

                var last = array[_gazeData.elements - 1];
                gazeData.TimeMilliseconds = last.timeMilliseconds;
                gazeData.LeftEyePosition = new Vector2(last.left_x, last.left_y);
                gazeData.RightEyePosition = new Vector2(last.right_x, last.right_y);
                _mostRecentGazeData = (_gazeData.version, gazeData);
                return true;
            }

            gazeData = _mostRecentGazeData.gazeData;
            return true;
        }

        public ReadOnlySpan<InseyeGazeData> GetGazeDataFromLastFrame()
        {
            CheckAndUpdateDataArray();
            if (_gazeData.version != _inseyeGazeData.version)
            {
                if (_inseyeGazeData.array.Length < _gazeData.array.Length)
                    _inseyeGazeData.array = new InseyeGazeData[_gazeData.array.Length];
                var sourceArray = _gazeData.array;
                var targetArray = _inseyeGazeData.array;
                for (var i = 0; i < _gazeData.elements; i++) targetArray[i] = new InseyeGazeData(sourceArray[i]);

                _inseyeGazeData.version = _gazeData.version;
            }

            return new ReadOnlySpan<InseyeGazeData>(_inseyeGazeData.array, 0, _gazeData.elements);
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        public int Version => _gazeData.version;
        public ReadOnlySpan<GazeData> Array => new(_gazeData.array, 0, _gazeData.elements);

        private void CheckAndUpdateDataArray()
        {
            if (_gazeData.version == Time.frameCount)
                return;
            var elements = 0;
            var array = _gazeData.array;
            while (_reader.GetGazeData(ref array[elements]))
            {
                elements++;
                if (array.Length == elements)
                    System.Array.Resize(ref array, array.Length * 2);
            }

            _gazeData = (Time.frameCount, elements, array);
        }
    }
}