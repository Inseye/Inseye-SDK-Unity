// File name: TestGazeProviderFinalizer.cs

using System;
using System.Collections;
using Inseye;
using NUnit.Framework;
using Tests.Helpers.Android;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Runtime.Android
{
	public class TestGazeProviderAndroid
	{
		private AndroidServiceProxy _androidServiceProxy;
		private const int Port = 43512;

		[SetUp]
		public void SetUp()
		{
			if(Application.isEditor)
				Assert.Ignore("Android tests are skipped in editor.");
			_androidServiceProxy = new AndroidServiceProxy();
			_androidServiceProxy.MockGazeSource(Port);
		}

		[UnityTest]
		public IEnumerator TestGazeProviderFinalizer()
		{
			// regression test for GazeProvider finalizer on Android which made JNI calls on GC thread thus crashing android application
			var gazeProvider = InseyeSDK.GetGazeProvider();
			Assert.IsNotNull(gazeProvider);
			Assert.AreEqual(InseyeSDKState.MostRecentGazePointAvailable | InseyeSDKState.Initialized, InseyeSDK.InseyeSDKState);
			gazeProvider = null;
			GC.Collect(); // assumption is made that GC runs not on main thread
			yield return new WaitForSeconds(1); // wait for delayed execution of InseyeAndroidSDKInplmenetation.RemoveListener
			Assert.AreEqual(InseyeSDKState.Uninitialized, InseyeSDK.InseyeSDKState);
		}

		[TearDown]
		public void TearDown()
		{
			if(Application.isEditor)
				return;
			_androidServiceProxy.DisableGazeSourceMock();
			_androidServiceProxy.Dispose();
		}

	}
}