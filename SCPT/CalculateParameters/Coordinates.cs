using System;

namespace CalculateParameters
{
    public class Coordinates
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public Coordinates(double x, double y, double z)
        {
            if (double.IsNaN(x))
                throw new ArgumentException("x coordinate cannot be NaN");
            if (double.IsNaN(y))
                throw new ArgumentException("y coordinate cannot be NaN");
            if (double.IsNaN(z))
                throw new ArgumentException("z coordinate cannot be NaN");
            if (double.IsInfinity(x))
                throw new ArithmeticException("x coordinate attained infinity",
                    new ArgumentOutOfRangeException());
            if (double.IsInfinity(y))
                throw new ArithmeticException("y coordinate attained infinity",
                    new ArgumentOutOfRangeException());
            if (double.IsInfinity(z))
                throw new ArithmeticException("z coordinate attained infinity",
                    new ArgumentOutOfRangeException());
            
            X = x;
            Y = y;
            Z = z;
        }
    }
}