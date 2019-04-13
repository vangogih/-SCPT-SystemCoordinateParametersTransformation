using System;

namespace CalculateParameters
{
    public class SystemCoordinate
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }


        public SystemCoordinate(double x, double y, double z)
        {
            if (double.IsNaN(x))
                throw new ArgumentException("x coordinate cannot be NaN");
            if (double.IsNaN(y))
                throw new ArgumentException("y coordinate cannot be NaN");
            if (double.IsNaN(z))
                throw new ArgumentException("z coordinate cannot be NaN");
            if (double.IsPositiveInfinity(x))
                throw new ArithmeticException("x coordinate attained positive infinity",
                    new ArgumentOutOfRangeException());
            if (double.IsPositiveInfinity(y))
                throw new ArithmeticException("y coordinate attained positive infinity",
                    new ArgumentOutOfRangeException());
            if (double.IsPositiveInfinity(z))
                throw new ArithmeticException("z coordinate attained positive infinity",
                    new ArgumentOutOfRangeException());
            if (double.IsPositiveInfinity(x))
                throw new ArithmeticException("x coordinate attained negative infinity",
                    new ArgumentOutOfRangeException());
            if (double.IsPositiveInfinity(y))
                throw new ArithmeticException("y coordinate attained negative infinity",
                    new ArgumentOutOfRangeException());
            if (double.IsPositiveInfinity(z))
                throw new ArithmeticException("z coordinate attained negative infinity",
                    new ArgumentOutOfRangeException());


            X = x;
            Y = y;
            Z = z;
        }
    }
}