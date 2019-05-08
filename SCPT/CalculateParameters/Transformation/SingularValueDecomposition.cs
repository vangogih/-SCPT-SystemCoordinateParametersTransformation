using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using SCPT.Helper;

namespace SCPT.Transformation
{
    /// <inheritdoc />
    /// <summary>
    /// <code>
    /// Obtaining transformation parameters using SVD decomposition.
    /// The basic idea is that the rotation matrix (R) can be obtained by
    /// multiplying the matrices obtained from the decomposition (U * VT).
    /// Next, calculate how much one coordinate system "more"
    /// another by comparing the two systems (trace1/trace2).
    /// Then do the reverse conversion helmert and get dx,dy,dz.
    /// </code>
    /// <remarks>
    /// <code>
    /// Calculation transformation parameters possible with one source and destination point.
    /// Accuracy rating cannot be estimated
    /// </code>
    /// </remarks>
    /// </summary>
    public sealed class SingularValueDecomposition : AbstractTransformation
    {
        /// <inheritdoc />
        public override SystemCoordinate SourceSystemCoordinates { get; }

        /// <inheritdoc />
        public override SystemCoordinate DestinationSystemCoordinates { get; }

        /// <inheritdoc />
        public override RotationMatrix RotationMatrix { get; }

        /// <inheritdoc />
        public override DeltaCoordinateMatrix DeltaCoordinateMatrix { get; }

        /// <inheritdoc />
        public override double M { get; }

        /// <inheritdoc />
        public SingularValueDecomposition(SystemCoordinate srcListCord, SystemCoordinate destListCord) : base(srcListCord,
            destListCord)
        {
            SourceSystemCoordinates = srcListCord;
            DestinationSystemCoordinates = destListCord;

            var countPoints = SourceSystemCoordinates.List.Count;
            var srcMatrix = SourceSystemCoordinates.Matrix;
            var dstMatrix = DestinationSystemCoordinates.Matrix;
            
            var eMatrix = CreateEMatrix();
            var cMatrix = dstMatrix.Transpose() * eMatrix * srcMatrix;

            var svd = cMatrix.Svd();
            var rotMatrix = (svd.U * svd.VT).Transpose(); // rotation matrix
            var trace1 = cMatrix.Multiply(rotMatrix).Diagonal().Sum();
            var trace2 = (srcMatrix.Transpose() * eMatrix * srcMatrix).Diagonal().Sum();

            var scale = trace1 / trace2;

            // reverse Helmert transformation
            var dMatrixInterim = scale * (srcMatrix * rotMatrix);
            var deltaMatrix = (1d / countPoints * (dstMatrix - dMatrixInterim)).Transpose();
            var finalTVector = deltaMatrix.RowSums();

            DeltaCoordinateMatrix = new DeltaCoordinateMatrix(finalTVector);
            RotationMatrix = new RotationMatrix(rotMatrix.Transpose(), false);
            M = scale - 1d;
        }

        private Matrix<double> CreateEMatrix()
        {
            var count = SourceSystemCoordinates.List.Count;
            var eMatrix = Matrix<double>.Build.Dense(count, count, -1d / count);
            var diagonal = Vector<double>.Build.Dense(count, 1d - 1d / count);
            eMatrix.SetDiagonal(diagonal);
            return eMatrix;
        }

        #region InternalMembersForTests

        internal Matrix<double> CreateEMatrixTst()
        {
            return CreateEMatrix();
        }

        #endregion
    }
}