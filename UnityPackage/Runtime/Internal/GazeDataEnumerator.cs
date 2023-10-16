// Module name: com.inseye.unity.sdk
// File name: GazeDataEnumerator.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;

namespace Inseye.Internal
{
    internal struct GazeDataEnumerator : IEnumerator<GazeData>
    {
        private int _version;
        private readonly IVersionedSpan<GazeData> _dataSource;
        private int _index;

        internal GazeDataEnumerator(int version, IVersionedSpan<GazeData> dataSource)
        {
            _index = -1;
            _version = version;
            _dataSource = dataSource;
            Current = default;
        }

        public bool MoveNext()
        {
            if (!CheckVersion())
                return false;
            if (++_index >= _dataSource.Array.Length) return false;
            Current = _dataSource.Array[_index];
            return true;
        }

        public void Reset()
        {
            _index = -1;
        }

        public GazeData Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            _version = -1;
        }

        private bool CheckVersion()
        {
            if (_dataSource == null)
                return false;
            return _version == _dataSource.Version;
        }
    }
}