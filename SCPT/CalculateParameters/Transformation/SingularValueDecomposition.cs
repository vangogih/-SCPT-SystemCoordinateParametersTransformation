using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using SCPT.Helper;

namespace SCPT.Transformation
{
    /// <summary>
    /// <code>
    /// Obtaining transformation parameters using SVD decomposition.
    /// 
    /// The basic idea is that the rotation matrix (R) can be obtained by
    /// multiplying the matrices obtained from the decomposition (U * VT).
    /// 
    /// Next, calculate how much one coordinate system "more"
    /// another by comparing the two systems (trace1/trace2).
    /// 
    /// Then do the reverse conversion helmert and get dx,dy,dz.
    /// </code>
    /// <remarks>
    /// <code>
    /// Calculation transformation parameters possible with one source and destination point.
    /// Accuracy rating cannot be estimated
    /// </code>
    /// </remarks>
    /// </summary>
    internal class SingularValueDecomposition : AbstractTransformation
    {
        /// <summary>
        /// <inheritdoc cref="AbstractTransformation.SourceSystemCoordinates"/>
        /// </summary>
        public new List<Point> SourceSystemCoordinates { get; }

        /// <summary>
        /// <inheritdoc cref="AbstractTransformation.DestinationSystemCoordinates"/>
        /// </summary>
        public new List<Point> DestinationSystemCoordinates { get; }

        /// <summary>
        /// <inheritdoc cref="AbstractTransformation.RotationMatrix"/>
        /// </summary>
        public new Matrix<double> RotationMatrix { get; private set; }

        /// <summary>
        /// <inheritdoc cref="AbstractTransformation.DeltaCoordinateMatrix"/>
        /// </summary>
        public new Vector<double> DeltaCoordinateMatrix { get; private set; }

        /// <summary>
        /// <inheritdoc cref="AbstractTransformation.M"/>
        /// </summary>
        public new double M { get; private set; }

        /// <inheritdoc />
        public SingularValueDecomposition(List<Point> srcListCord, List<Point> destListCord) : base(srcListCord,
            destListCord)
        {
            SourceSystemCoordinates = srcListCord;
            DestinationSystemCoordinates = destListCord;

            CalculateTransformationParameters();
        }

        private void CalculateTransformationParameters()
        {
            var countPoints = SourceSystemCoordinates.Count;
            var srcMatrix = Matrix.Build.Dense(SourceSystemCoordinates.Count, 3);
            var dstMatrix = Matrix.Build.Dense(DestinationSystemCoordinates.Count, 3);
            for (int row = 0; row < countPoints; row++)
            {
                srcMatrix[row, 0] = SourceSystemCoordinates[row].X;
                srcMatrix[row, 1] = SourceSystemCoordinates[row].Y;
                srcMatrix[row, 2] = SourceSystemCoordinates[row].Z;

                dstMatrix[row, 0] = DestinationSystemCoordinates[row].X;
                dstMatrix[row, 1] = DestinationSystemCoordinates[row].Y;
                dstMatrix[row, 2] = DestinationSystemCoordinates[row].Z;
            }

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

            DeltaCoordinateMatrix = finalTVector;
            RotationMatrix = rotMatrix.Transpose();
            M = scale - 1d;
        }

        private Matrix<double> CreateEMatrix()
        {
            var count = SourceSystemCoordinates.Count;
            var eMatrix = Matrix<double>.Build.Dense(count, count, -1d / count);
            var diagonal = Vector.Build.Dense(count, 1d - 1d / count);
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