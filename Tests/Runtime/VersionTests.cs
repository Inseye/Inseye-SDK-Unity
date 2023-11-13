using System;
using NUnit.Framework;

namespace Inseye.Tests
{
    [TestFixture]
    public class VersionTests
    {
        [Test]
        public void TestParseNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                InseyeComponentVersion.Parse(null);
            });
        }
        

        [Test]
        public void TestParseValidString()
        {
            var version = InseyeComponentVersion.Parse("1.3.4-extra");
            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(3, version.Minor);
            Assert.AreEqual(4, version.Patch);
            Assert.AreEqual("extra", version.Extra);
        }
        
        [Test]
        public void TestParseValidLongString()
        {
            var version = InseyeComponentVersion.Parse("123.3455.44-extra-rc1-test-long");
            Assert.AreEqual(123, version.Major);
            Assert.AreEqual(3455, version.Minor);
            Assert.AreEqual(44, version.Patch);
            Assert.AreEqual("extra-rc1-test-long", version.Extra);
        }

        [Test]
        public void TestParseOnlyMajor()
        {
            var version = InseyeComponentVersion.Parse("1");
            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(0, version.Minor);
            Assert.AreEqual(0, version.Patch);
            Assert.AreEqual("", version.Extra);
        }
        
        [Test]
        public void TestParseMajorAndMinor()
        {
            var version = InseyeComponentVersion.Parse("1.2");
            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(2, version.Minor);
            Assert.AreEqual(0, version.Patch);
            Assert.AreEqual("", version.Extra);
        }

        [Test]
        public void ThrowOnInvalidFormat()
        {
            Assert.Throws<FormatException>(() =>
            {
                var version = InseyeComponentVersion.Parse("1.2dupa");
            });
        }

        [Test]
        public void TestOnlyMajorAndMinorAndPatch()
        {
            var version = InseyeComponentVersion.Parse("0.0.0");
            Assert.AreEqual(0, version.Major);
            Assert.AreEqual(0, version.Minor);
            Assert.AreEqual(0, version.Patch);
        }
    }
}