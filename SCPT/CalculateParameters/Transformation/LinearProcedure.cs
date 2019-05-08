using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using SCPT.Helper;
using SCPT.Helper.VecParams;

namespace SCPT.Transformation
{
    /// <inheritdoc />
    /// <summary>
    /// <code>
    /// Method get transformation parameters thought least squares.
    /// Forming matrix conditional equations (Q). 
    /// Forming vector amendments' for X,Y,Z (Vx, Vy, Vz). 
    /// Thought least squares get rotation matrix multiply on (1+m).
    /// </code> 
    /// <remarks>
    /// Calculation transformation parameters possible with one source and destination point.  
    /// Accuracy rating cannot be estimated
    /// </remarks>
    /// </summary>
    public sealed class LinearProcedure : AbstractTransformation
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
        public LinearProcedure(SystemCoordinate srcListCord, SystemCoordinate destListCord) : base(srcListCord,
            destListCord)
        {
            SourceSystemCoordinates = srcListCord;
            DestinationSystemCoordinates = destListCord;

            var qMatrix = FormingQMatrix();
            var lxVector = FormingLxVector();
            var lyVector = FormingLyVector();
            var lzVector = FormingLzVector();
            var dxVector = CalculateDVectors(qMatrix, lxVector);
            var dyVector = CalculateDVectors(qMatrix, lyVector);
            var dzVector = CalculateDVectors(qMatrix, lzVector);

            IVectorParameters helper = new LPVecParams(dxVector, dyVector, dzVector);
            RotationMatrix = helper.RotationMatrix;
            DeltaCoordinateMatrix = helper.DeltaCoordinateMatrix;
            M = helper.ScaleFactor;
        }

        private Matrix<double> FormingQMatrix()
        {
            var qMatrix = Matrix<double>.Build.Dense(SourceSystemCoordinates.List.Count, 4);
            qMatrix.SetSubMatrix(0, SourceSystemCoordinates.List.Count, 0, 3, SourceSystemCoordinates.Matrix);
            qMatrix.SetColumn(3, 1);
            return qMatrix;
        }

        private Vector<double> FormingLxVector()
        {
            var lxVector = Vector<double>.Build.Dense(DestinationSystemCoordinates.List.Count);

            for (int row = 0; row < DestinationSystemCoordinates.List.Count; row++)
                lxVector[row] = -DestinationSystemCoordinates.List[row].X;

            return lxVector;
        }

        private Vector<double> FormingLyVector()
        {
            var lyVector = Vector<double>.Build.Dense(DestinationSystemCoordinates.List.Count);

            for (int row = 0; row < DestinationSystemCoordinates.List.Count; row++)
                lyVector[row] = -DestinationSystemCoordinates.List[row].Y;

            return lyVector;
        }


        private Vector<double> FormingLzVector()
        {
            var lzVector = Vector<double>.Build.Dense(DestinationSystemCoordinates.List.Count);

            for (int row = 0; row < DestinationSystemCoordinates.List.Count; row++)
                lzVector[row] = -DestinationSystemCoordinates.List[row].Z;

            return lzVector;
        }

        private Vector<double> CalculateDVectors(Matrix<double> qMatrix, Vector<double> lVector)
        {
            // dVector = -(Q^T*Q)*Q^T*L
            var dVector = Vector<double>.Build.Dense(4);

            var qTranspose = qMatrix.Transpose();
            var qInverse = (qTranspose * qMatrix).Inverse();
            dVector = -(qInverse * qTranspose) * lVector;

            return dVector;
        }


        #region InternalMethodsForTests

        internal Matrix<double> FormingQMatrixTst()
        {
            return FormingQMatrix();
        }

        internal Vector<double> FormingLxVectorTst()
        {
            return FormingLxVector();
        }

        internal Vector<double> FormingLyVectorTst()
        {
            return FormingLyVector();
        }

        internal Vector<double> FormingLzVectorTst()
        {
            return FormingLzVector();
        }

        internal Vector<double> CalculateDVectorsTst(Matrix<double> qMatrix, Vector<double> lxVector)
        {
            return CalculateDVectors(qMatrix, lxVector);
        }

        #endregion
    }
}