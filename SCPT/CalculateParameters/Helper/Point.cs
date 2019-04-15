using System;

namespace SCPT.Helper
{
    public class Point
    {
        /// <summary>
        /// value characterizing the displacement of the point relative to 0 on the X axis
        /// </summary>
        public double X { get; }

        /// <summary>
        /// value characterizing the displacement of the point relative to 0 on the Y axis
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// value characterizing the displacement of the point relative to 0 on the Z axis
        /// </summary>
        public double Z { get; }

        /// <summary>
        /// Point, an entity that has a location in space or on a plane, but has no extent
        /// </summary>
        /// <exception cref="ArgumentException">throw then coordinate cannot be NaN</exception>
        /// <exception cref="ArithmeticException">throw then coordinate attained infinity</exception>
        public Point(double x, double y, double z)
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