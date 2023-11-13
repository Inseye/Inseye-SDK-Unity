using NUnit.Framework;

namespace Inseye.Tests
{
    [TestFixture]
    public class EnumeratorTests
    {
        [Test]
        public static void TestDefaultMoveNext()
        {
            InseyeGazeDataEnumerator enumerator = default;
            Assert.False(enumerator.MoveNext());
        }

        [Test]
        public static void TestDefaultCurrentUninitialized()
        {
            InseyeGazeDataEnumerator enumerator = default;
            Assert.AreEqual(enumerator.Current, new InseyeGazeData());
        }
    }
}