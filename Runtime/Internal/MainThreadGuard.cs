// Module name: com.inseye.unity.sdk
// File name: MainThreadGuard.cs
// Last edit: 2023-11-26 by Mateusz Chojnowski mateusz.chojnowski@inseye.com
// Copyright (c) Inseye Inc.
// 
// This file is part of Inseye Software Development Kit subject to Inseye SDK License
// See  https://github.com/Inseye/Licenses/blob/master/SDKLicense.txt.
// All other rights reserved.

using System;
using System.Threading;
using UnityEngine;

namespace Inseye.Internal
{
	internal static class MainThreadGuard
	{
		public static int MainThreadId { get; private set; }
		private static SynchronizationContext UnitySynchronizationContext;

		public static bool IsOnMainThread => Thread.CurrentThread.ManagedThreadId == MainThreadId;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
		private static void GetMainThreadId()
		{
			MainThreadId = Thread.CurrentThread.ManagedThreadId;
			UnitySynchronizationContext = SynchronizationContext.Current;
		}

		public static void OnMainThread(Action action)
		{
			if (IsOnMainThread)
			{
				action();
				return;
			}
			UnitySynchronizationContext.Post(PostCallback, action);
		}

		private static void PostCallback(object state)
		{
			var action = (Action)state;
			try
			{
				action();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
	}
}