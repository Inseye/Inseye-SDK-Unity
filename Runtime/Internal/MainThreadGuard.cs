// Module name: com.inseye.unity.sdk
// File name: MainThreadGuard.cs
// Last edit: 2023-10-09 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System.Threading;
using UnityEngine;

namespace Inseye.Internal
{
    internal static class MainThreadGuard
    {
        public static int MainThreadId { get; private set; }

        public static bool IsOnMainThread => Thread.CurrentThread.ManagedThreadId == MainThreadId;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void GetMainThreadId()
        {
            MainThreadId = Thread.CurrentThread.ManagedThreadId;
        }
    }
}