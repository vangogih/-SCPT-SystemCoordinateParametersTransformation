using System.Collections.Generic;
using Extreme.Mathematics;
using Extreme.Mathematics.LinearAlgebra;
using SCPT.Helper;

namespace SCPT.Transformation
{
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
        /// <summary>
        /// <inheritdoc cref="AbstractTransformation.SourceSystemCoordinates" />
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
        public LinearProcedure(List<Point> srcListCord, List<Point> destListCord) : base(srcListCord, destListCord)
        {
            SourceSystemCoordinates = srcListCord;
            DestinationSystemCoordinates = destListCord;

            CalculateTransformationParameters();
        }

        private void CalculateTransformationParameters()
        {
            var qMatrix = FormingQMatrix();
            var lxVector = FormingLxVector();
            var lyVector = FormingLyVector();
            var lzVector = FormingLzVector();
            var dxVector = CalculateDVectors(qMatrix, lxVector);
            var dyVector = CalculateDVectors(qMatrix, lyVector);
            var dzVector = CalculateDVectors(qMatrix, lzVector);
            var rotationMatrixWithM = FormingRotationMatrixWithM(dxVector, dyVector, dzVector);
            RotationMatrix = ConvertMatrix.Convert_RotMatrixWithM_To_RotMatrixWithoutM(rotationMatrixWithM);

            DeltaCoordinateMatrix = Vector.Create<double>(3);
            DeltaCoordinateMatrix[0] = dxVector[3];
            DeltaCoordinateMatrix[1] = dyVector[3];
            DeltaCoordinateMatrix[2] = dzVector[3];

            M = rotationMatrixWithM[0, 0] - 1;
        }

        private Matrix<double> FormingQMatrix()
        {
            var qMatrix = Matrix.Create<double>(SourceSystemCoordinates.Count, 4);
            for (int row = 0; row < SourceSystemCoordinates.Count; row++)
            {
                qMatrix[row, 0] = SourceSystemCoordinates[row].X;
                qMatrix[row, 1] = SourceSystemCoordinates[row].Y;
                qMatrix[row, 2] = SourceSystemCoordinates[row].Z;
                qMatrix[row, 3] = 1;
            }

            return qMatrix;
        }

        private Vector<double> FormingLxVector()
        {
            var lxVector = Vector.Create<double>(DestinationSystemCoordinates.Count);

            for (int row = 0; row < DestinationSystemCoordinates.Count; row++)
                lxVector[row] = -DestinationSystemCoordinates[row].X;

            return lxVector;
        }

        private Vector<double> FormingLyVector()
        {
            var lyVector = Vector.Create<double>(DestinationSystemCoordinates.Count);

            for (int row = 0; row < DestinationSystemCoordinates.Count; row++)
                lyVector[row] = -DestinationSystemCoordinates[row].Y;

            return lyVector;
        }


        private Vector<double> FormingLzVector()
        {
            var lzVector = Vector.Create<double>(DestinationSystemCoordinates.Count);

            for (int row = 0; row < DestinationSystemCoordinates.Count; row++)
                lzVector[row] = -DestinationSystemCoordinates[row].Z;

            return lzVector;
        }

        private Vector<double> CalculateDVectors(Matrix<double> qMatrix, Vector<double> lVector)
        {
            // dVector = -(Q^T*Q)*Q^T*L
            var dVector = Vector.Create<double>(4, 1);

            var qTranspose = qMatrix.Transpose();
            var qInverse = Matrix.Multiply(qTranspose, qMatrix).GetInverse();
            dVector = -Vector.Multiply(Matrix.Multiply(qInverse, qTranspose), lVector) as DenseVector<double>;

            return dVector;
        }

        private Matrix<double> FormingRotationMatrixWithM(Vector<double> dxVector, Vector<double> dyVector,
            Vector<double> dzVector)
        {
            var rotMatrix = Matrix.Create<double>(3, 3);

            rotMatrix[0, 0] = dxVector[0];
            rotMatrix[0, 1] = dxVector[1];
            rotMatrix[0, 2] = dxVector[2];

            rotMatrix[1, 0] = dyVector[0];
            rotMatrix[1, 1] = dyVector[1];
            rotMatrix[1, 2] = dyVector[2];

            rotMatrix[2, 0] = dzVector[0];
            rotMatrix[2, 1] = dzVector[1];
            rotMatrix[2, 2] = dzVector[2];

            return rotMatrix;
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

        internal Matrix<double> FormingRotationMatrixTst(Vector<double> dxVector, Vector<double> dyVector,
            Vector<double> dzVector)
        {
            return FormingRotationMatrixWithM(dxVector, dyVector, dzVector);
        }

        #endregion
    }
}