using System;
using MathNet.Numerics.LinearAlgebra;

namespace SCPT.Helper
{
    /// <summary>
    /// Representation Delta Coordinate Matrix.
    /// <seealso cref="AbstractTransformation.DeltaCoordinateMatrix"/>
    /// </summary>
    public class DeltaCoordinateMatrix
    {
        /// <inheritdoc cref="AbstractTransformation.DeltaCoordinateMatrix"/>
        public Vector<double> Vector { get; }

        /// <summary>
        /// offset in meters relative to the original axis X coordinate system
        /// </summary>
        public double Dx { get; }

        /// <summary>
        /// offset in meters relative to the original axis Y coordinate system
        /// </summary>
        public double Dy { get; }

        /// <summary>
        /// offset in meters relative to the original axis Z coordinate system
        /// </summary>
        public double Dz { get; }

        /// <summary>
        /// Forming delta coordinate vector from dx,dy,dz parameters
        /// </summary>
        /// <param name="dx"><see cref="Dx"/></param>
        /// <param name="dy"><see cref="Dy"/></param>
        /// <param name="dz"><see cref="Dz"/></param>
        /// <exception cref="ArgumentOutOfRangeException">Throw then one of dx,dy,dz attained to infinity</exception>
        /// <exception cref="ArgumentException">Throw then one of dx,dy,dz is NaN</exception>
        public DeltaCoordinateMatrix(double dx, double dy, double dz)
        {
            if (double.IsInfinity(dx)) throw new ArgumentOutOfRangeException(nameof(dx) + "attained to infinity.");
            if (double.IsInfinity(dy)) throw new ArgumentOutOfRangeException(nameof(dy) + "attained to infinity.");
            if (double.IsInfinity(dz)) throw new ArgumentOutOfRangeException(nameof(dz) + "attained to infinity.");
            if (double.IsNaN(dx)) throw new ArgumentException(nameof(dx) + "Cannot be NaN");
            if (double.IsNaN(dy)) throw new ArgumentException(nameof(dy) + "Cannot be NaN");
            if (double.IsNaN(dz)) throw new ArgumentException(nameof(dz) + "Cannot be NaN");

            Dx = dx;
            Dy = dy;
            Dz = dz;

            Vector = Vector<double>.Build.Dense(3);
            Vector[0] = dx;
            Vector[1] = dy;
            Vector[2] = dz;
        }

        /// <summary>
        /// Forming delta coordinate vector from vector
        /// </summary>
        /// <exception cref="ArgumentNullException">Throw then input parameter is NULL</exception>
        /// <exception cref="ArgumentException">Throw then Count input vector not equal 3</exception>
        public DeltaCoordinateMatrix(Vector<double> vector)
        {
            if (vector == null) throw new ArgumentNullException(nameof(vector));
            if (vector.Count != 3) throw new ArgumentException("Value cannot be an empty collection.", nameof(vector));

            Vector = vector;
            Dx = vector[0];
            Dy = vector[1];
            Dz = vector[2];
        }
    }
}