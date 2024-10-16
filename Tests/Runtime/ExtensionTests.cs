﻿using System;
using Inseye.Extensions;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Inseye.Tests
{
    [TestFixture]
    public class ExtensionTests
    {
        private static InseyeGazeData[] data = {
            new()
            {
                LeftEyePosition = new Vector2(1, 1),
                RightEyePosition = new Vector2(-1, -1),
            },
            new()
            {
                LeftEyePosition = new Vector2(-10, 1),
                RightEyePosition = new Vector2(10, -10)
            },
            new()
            {
                LeftEyePosition = new Vector2(5, -3),
                RightEyePosition = new Vector2(8, -2)
            }
        };

        [SetUp]
        public void Setup()
        {
            var gameObject = new GameObject();
            gameObject.AddComponent<Camera>();
            gameObject.tag = "MainCamera";
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(Camera.main!.gameObject);
        }
        [Test]
        public void TestGazeMeanPositionBoth()
        {
            var asSpan = new ReadOnlySpan<InseyeGazeData>(data);
            var both = asSpan.GazeMeanPosition(Eyes.Both);
            var expectedxSum = 0f + 0f + 13f / 2;
            var expectedySum = 0f + -9f / 2f + -5f / 2f;
            var eBoth = new Vector2(expectedxSum, expectedySum) / 3;
            Assert.AreEqual(eBoth.x, both.x, float.Epsilon);
            Assert.AreEqual(eBoth.y, both.y, float.Epsilon);
        }

        [Test]
        public void TestGazeMeanPositionLeft()
        {
            var asSpan = new ReadOnlySpan<InseyeGazeData>(data);
            var left = asSpan.GazeMeanPosition(Eyes.Left);
            var expectedxSum = (1f + -10f + 5f) / 3;
            var expectedySum = (1f + 1f - 3f) / 3;
            Assert.AreEqual(expectedxSum, left.x, float.Epsilon);
            Assert.AreEqual(expectedySum, left.y, float.Epsilon);
        }
        
        [Test]
        public void TestGazeMeanPositionRight()
        {
            var asSpan = new ReadOnlySpan<InseyeGazeData>(data);
            var right = asSpan.GazeMeanPosition(Eyes.Right);
            var expectedxSum = (-1f + 10f + 8f) / 3;
            var expectedySum = (-1f - 10f - 2f) / 3;
            Assert.AreEqual(expectedxSum, right.x, float.Epsilon);
            Assert.AreEqual(expectedySum, right.y, float.Epsilon);
        }

        [Test]
        public void TestWorldToTrackerAndTrackerToWorld()
        {
            var world = new Vector3(0.7f, 0.5f, 2);
            var camTransform = Camera.main!.transform;
            var distanceFromCamera = Vector3.Distance(world, camTransform.position);
            var tracker = world.WorldToTrackerPoint(camTransform);
            var worldBack = tracker.TrackerToWorldPoint(distanceFromCamera, camTransform);
            Assert.AreEqual(world, worldBack);
        }
    }
}