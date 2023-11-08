using Inseye;
using NUnit.Framework;

namespace EditorTests
{
    public static class TestComponentVersion
    {
        [Test]
        public static void TestMajorComparision()
        {
            var first = new InseyeComponentVersion(0, 1, 2);
            var second = new InseyeComponentVersion(1, 1, 2);
            Assert.IsTrue(first < second);
            Assert.IsTrue(second > first);
        }
        
        [Test]
        public static void TestMinorComparision()
        {
            var first = new InseyeComponentVersion(0, 0, 2);
            var second = new InseyeComponentVersion(0, 1, 2);
            Assert.IsTrue(first < second);
            Assert.IsTrue(second > first);
        }
        
        [Test]
        public static void TestPatchComparision()
        {
            var first = new InseyeComponentVersion(0, 0, 1);
            var second = new InseyeComponentVersion(0, 0, 2);
            Assert.IsTrue(first < second);
            Assert.IsTrue(second > first);
        }
        
        [Test]
        public static void TestEqual()
        {
            var first = new InseyeComponentVersion(1, 0, 1);
            var second = new InseyeComponentVersion(1, 0, 1);
            Assert.IsTrue(first == second);
            Assert.IsFalse(first != second);
            Assert.IsFalse(first < second);
            Assert.IsFalse(second > first);
        }
        
        
    }
}