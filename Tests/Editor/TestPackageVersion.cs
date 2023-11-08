#nullable enable
using System.Collections;
using System.Linq;
using Inseye;
using NUnit.Framework;
using UnityEditor.PackageManager;
using UnityEngine.TestTools;

namespace EditorTests
{
   public class TestPackageVersion
   {
      [UnityTest]
      public IEnumerator TestVersion()
      {
         var packageListRequest = Client.List();
         while (!packageListRequest.IsCompleted)
         {
            yield return null;
         }
         Assert.IsTrue(packageListRequest.Status == StatusCode.Success);
         var packageCollection = packageListRequest.Result;
         var packageInfo = packageCollection.FirstOrDefault(p => p.name == "com.inseye.unity.sdk");
         Assert.IsNotNull(packageInfo);
         Assert.AreEqual(packageInfo.version, InseyeSDK.SDKVersion.ToString());
      }
   }
}
