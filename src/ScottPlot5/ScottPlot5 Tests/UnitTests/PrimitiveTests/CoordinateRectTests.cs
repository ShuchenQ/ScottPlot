﻿namespace ScottPlotTests.UnitTests.PrimitiveTests;

internal class CoordinateRectTests
{
    [Test]
    public void Test_CoordinateRect_Defaults()
    {
        CoordinateRect cRect = new();
        cRect.Left.Should().Be(0);
        cRect.Right.Should().Be(0);
        cRect.Bottom.Should().Be(0);
        cRect.Top.Should().Be(0);
        cRect.HasArea.Should().BeFalse();
    }

    [Test]
    public void Test_CoordinateRect_Constructor()
    {
        CoordinateRect cRect = new(-3, 7, -13, 11);
        cRect.Left.Should().Be(-3);
        cRect.Right.Should().Be(7);
        cRect.Bottom.Should().Be(-13);
        cRect.Top.Should().Be(11);
        cRect.Width.Should().Be(10);
        cRect.Height.Should().Be(24);
        cRect.HasArea.Should().BeTrue();
    }

    [Test]
    public void Test_Coordinate_CustomToString()
    {
        new CoordinateRect().ToString().Should().Contain("XMin=");
    }
}
