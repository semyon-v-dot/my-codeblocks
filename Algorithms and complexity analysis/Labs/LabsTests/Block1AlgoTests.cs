using System;
using System.Drawing;
using NUnit.Framework;

using Block1Algo;

namespace LabsTests;

public class Block1AlgoTests
{
    private readonly PolygonAlgo _algo = new();

    [TestCase(1, 1, 2, 2, 1.414213562373095)] // sqrt(2)
    [TestCase(1, 0, 1, 1, 1)]
    [TestCase(0, 1, 1, 1, 1)]
    [TestCase(3, 2, 9, 7, 7.810249675906654)] // sqrt(61)
    public void GetDistanceTests(int ax, int ay, int bx, int by, double distance)
    {
        Assert.AreEqual(
            distance, 
            _algo.GetDistance(new Point(ax, ay), new Point(bx, by)),
            0.000001);
    }
    
    [TestCase(2, 2, 2, 1, Math.PI / 2)]
    [TestCase(2, 1, 2, 2, -Math.PI / 2)]
    [TestCase(1, 1, 2, 2, -Math.PI / 4)]
    [TestCase(2, 2, 1, 1, 3 * Math.PI / 4)]
    public void GetPolarAngleCounterclockwiseTests(int ax, int ay, int bx, int by, double angle)
    {
        Assert.AreEqual(
            angle, 
            _algo.GetPolarAngleCounterclockwise(new Point(ax, ay), new Point(bx, by)),
            0.000001);
    }
    
    [TestCase(1,1,2,2,3,3, true)]
    [TestCase(5,5,7,3,4,2, true)]
    [TestCase(5,5,7,3,9,3, false)]
    public void TurnIsLeftOrForwardTests(int ax, int ay, int bx, int by, int cx, int cy, bool isLeft)
    {
        Assert.AreEqual(
            isLeft,
            (_algo.TurnIsLeft(new Point(ax, ay), new Point(bx, by), new Point(cx, cy))
             || _algo.TurnIsForward(new Point(ax, ay), new Point(bx, by), new Point(cx, cy)))
            && !_algo.TurnIsRight(new Point(ax, ay), new Point(bx, by), new Point(cx, cy)));
    }

    [Test]
    public void GetConvexHullAreaTests()
    {
        //TODO
    }
}