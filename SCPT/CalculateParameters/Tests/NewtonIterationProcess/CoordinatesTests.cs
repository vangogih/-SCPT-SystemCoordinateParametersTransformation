using System;
using System.Runtime.InteropServices;
using CalculateParameters;
using Xunit;


public class CoordinatesTests
{
    [Fact]
    private void Coordinate_InitCtorInvalidArgumentNaN_ThrowsArgumentException()
    {
        double x = double.NaN, y = double.NaN, z = double.NaN;

        Action test = () => new Coordinates(x, y, z);

        Assert.Throws<ArgumentException>(test);
    }

    [Fact]
    private void Coordinate_InitCtorInvalidArgumentInfinity_ThrowsArithmeticException()
    {
        double xPositiveInfinity = double.PositiveInfinity;
        double yPositiveInfinity = double.PositiveInfinity;
        double zPositiveInfinity = double.PositiveInfinity;

        double xNegativeInfinity = Double.NegativeInfinity;
        double yNegativeInfinity = Double.NegativeInfinity;
        double zNegativeInfinity = Double.NegativeInfinity;

        Action test1 = () => new Coordinates(xPositiveInfinity, yPositiveInfinity, zPositiveInfinity);
        Action test2 = () => new Coordinates(xNegativeInfinity, yNegativeInfinity, zNegativeInfinity);

        Assert.Throws<ArithmeticException>(test1);
        Assert.Throws<ArithmeticException>(test2);
    }

    [Fact]
    private void Coordinate_TryGetXYZProperty_ValidXYZReturnValue()
    {
        var xExpected = 235654.242;
        var yExpected = 267654.567;
        var zExpected = 223654.234;

        var coords = new Coordinates(xExpected, yExpected, zExpected);

        Assert.Equal(xExpected, coords.X);
        Assert.Equal(yExpected, coords.Y);
        Assert.Equal(zExpected, coords.Z);
    }
}