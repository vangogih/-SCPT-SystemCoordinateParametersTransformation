using System;
using CalculateParameters;
using Xunit;


public class SystemCoordinateTests
{
    [Fact]
    public void SystemCoordinate_InitCtorInvalidArgument_ThrowsInvalidArgumentException()
    {
        double x = double.NaN, y = double.NaN, z = double.NaN;

        Action test = () => new SystemCoordinate(x, y, z);

        Assert.Throws<ArgumentException>(test);
    }
}