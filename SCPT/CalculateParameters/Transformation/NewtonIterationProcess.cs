using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using MathNet.Numerics.LinearAlgebra;
using SCPT.Helper;
using SCPT.Helper.VecParams;

namespace SCPT.Transformation
{
    /// <inheritdoc />
    /// <summary>
    /// <code>
    /// Method founded on fixed-point iteration and least squares. 
    /// Forming matrix conditional equations (A). 
    /// Forming matrix residual (Y). 
    /// Calculate transform parameters thought least squares.
    /// Do accuracy rating.
    /// </code>
    /// <remarks>
    /// Calculation transformation parameters possible with three and more source and destination point.
    /// </remarks> 
    /// </summary>
    public sealed class NewtonIterationProcess : AbstractTransformation
    {
        private const int MinListCount = 3;
        private const double TransformationAccuracy = 1E-15;

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

        /// <inheritdoc cref="AbstractTransformation.MeanSquareErrorsMatrix" />
        public new Vector<double> MeanSquareErrorsMatrix { get; }

        /// <inheritdoc />
        public NewtonIterationProcess(SystemCoordinate source, SystemCoordinate destination) : base(source, destination)
        {
            if (source.List.Count <= MinListCount)
                throw new ArgumentException("source list count cannot be less when 3");
            if (destination.List.Count <= MinListCount)
                throw new ArgumentException("source list count cannot be less when 3");

            SourceSystemCoordinates = source;
            DestinationSystemCoordinates = destination;

            var aMatrix = FormingAMatrix();
            var yMatrix = FormingYMatrix();
            var paramsTransformMatrix = GetMatrixWithTransformParameters(aMatrix, yMatrix);
            var vMatrix = GetVMatrix(aMatrix, yMatrix, paramsTransformMatrix);
            var fCoefficient = CalculateFCoefficient(vMatrix);
            var mCoefficient = CalculateMCoefficient(fCoefficient);
            var qMatrix = GetQMatrix(aMatrix);
            var mceMatrix = GetMeanSquareErrorsMatrix(qMatrix, mCoefficient);

            IVectorParameters helper = new NIPVecParams(paramsTransformMatrix.Column(0));
            RotationMatrix = helper.RotationMatrix;
            DeltaCoordinateMatrix = helper.DeltaCoordinateMatrix;
            M = helper.ScaleFactor;
            MeanSquareErrorsMatrix = mceMatrix;
        }

        #region PrivateForming&CalculationMethods

        private Matrix<double> FormingAMatrix()
        {
            var aMatrix = Matrix<double>.Build.Dense(SourceSystemCoordinates.List.Count * 3, 7);

            for (int matrixIndex = 0, listIndex = 0;
                matrixIndex < SourceSystemCoordinates.List.Count * 3 || listIndex < SourceSystemCoordinates.List.Count;
                matrixIndex += 3, listIndex++)
            {
                aMatrix[matrixIndex, 0] = 1;
                aMatrix[matrixIndex, 1] = 0;
                aMatrix[matrixIndex, 2] = 0;
                aMatrix[matrixIndex, 3] = 0;
                aMatrix[matrixIndex, 4] = -SourceSystemCoordinates.List[listIndex].Z;
                aMatrix[matrixIndex, 5] = SourceSystemCoordinates.List[listIndex].Y;
                aMatrix[matrixIndex, 6] = SourceSystemCoordinates.List[listIndex].X;

                aMatrix[matrixIndex + 1, 0] = 0;
                aMatrix[matrixIndex + 1, 1] = 1;
                aMatrix[matrixIndex + 1, 2] = 0;
                aMatrix[matrixIndex + 1, 3] = SourceSystemCoordinates.List[listIndex].Z;
                aMatrix[matrixIndex + 1, 4] = 0;
                aMatrix[matrixIndex + 1, 5] = -SourceSystemCoordinates.List[listIndex].X;
                aMatrix[matrixIndex + 1, 6] = SourceSystemCoordinates.List[listIndex].Y;

                aMatrix[matrixIndex + 2, 0] = 0;
                aMatrix[matrixIndex + 2, 1] = 0;
                aMatrix[matrixIndex + 2, 2] = 1;
                aMatrix[matrixIndex + 2, 3] = -SourceSystemCoordinates.List[listIndex].Y;
                aMatrix[matrixIndex + 2, 4] = SourceSystemCoordinates.List[listIndex].X;
                aMatrix[matrixIndex + 2, 5] = 0;
                aMatrix[matrixIndex + 2, 6] = SourceSystemCoordinates.List[listIndex].Z;
            }

            return aMatrix;
        }

        private Matrix<double> FormingYMatrix()
        {
            var yMatrix = Matrix<double>.Build.Dense(SourceSystemCoordinates.List.Count * 3, 1);

            for (int matrixIndex = 0, listIndex = 0;
                matrixIndex < SourceSystemCoordinates.List.Count * 3 && listIndex < SourceSystemCoordinates.List.Count;
                matrixIndex += 3, listIndex++)
            {
                yMatrix[matrixIndex, 0] =
                    DestinationSystemCoordinates.List[listIndex].X -
                    SourceSystemCoordinates.List[listIndex].X;
                yMatrix[matrixIndex + 1, 0] = DestinationSystemCoordinates.List[listIndex].Y -
                                              SourceSystemCoordinates.List[listIndex].Y;
                yMatrix[matrixIndex + 2, 0] = DestinationSystemCoordinates.List[listIndex].Z -
                                              SourceSystemCoordinates.List[listIndex].Z;
            }

            return yMatrix;
        }

        private Matrix<double> GetMatrixWithTransformParameters(Matrix<double> aMatrix, Matrix<double> yMatrix)
        {
            var currPMatrix = Matrix<double>.Build.Dense(7, 1);
            var prevPMatrix = Matrix<double>.Build.Dense(7, 1);
            for (int i = 0; i < prevPMatrix.RowCount; i++)
                prevPMatrix[i, 0] = double.MaxValue;

            while (IsSubtractMatrixValuesLessWhenDelta(prevPMatrix, currPMatrix, TransformationAccuracy))
            {
                prevPMatrix = currPMatrix;
                // Pi = P(i-1) - ((AT*A)^-1 * AT * Y) *  (A * P(i-1) - Y)
                var AT = aMatrix.Transpose();
                var aInverse = (AT * aMatrix).Inverse();
                var dX = aMatrix * prevPMatrix - yMatrix;
                var pMatrix = prevPMatrix - aInverse * AT * dX;
                currPMatrix = pMatrix;
            }

            return currPMatrix;
        }

        private bool IsSubtractMatrixValuesLessWhenDelta(Matrix<double> first, Matrix<double> second,
            double delta)
        {
            var subtract = first - second;
            for (int i = 0; i < second.RowCount; i++)
                if (subtract[i, 0] < delta)
                    return false;
            return true;
        }

        private Matrix<double> GetVMatrix(Matrix<double> aMatrix, Matrix<double> yMatrix,
            Matrix<double> vecParamsMatrix)
        {
            var vMatrix = Matrix<double>.Build.Dense(SourceSystemCoordinates.List.Count * 3, 1);
            var Ap = aMatrix * vecParamsMatrix;
            // V = A * P - Y
            vMatrix = Ap - yMatrix;

            return vMatrix;
        }

        private double CalculateFCoefficient(Matrix<double> vMatrix)
        {
            return (vMatrix.Transpose() * vMatrix)[0, 0];
        }

        private double CalculateMCoefficient(double fCoefficient)
        {
            return Math.Sqrt(fCoefficient / (SourceSystemCoordinates.List.Count * 3 - 7));
        }

        private Matrix<double> GetQMatrix(Matrix<double> aMatrix)
        {
            return (aMatrix.Transpose() * aMatrix).Inverse();
        }

        private Vector<double> GetMeanSquareErrorsMatrix(Matrix<double> qMatrix, double mCoefficient)
        {
            var qDiagonal = qMatrix.Diagonal().PointwiseSqrt();
            return qDiagonal * mCoefficient;
        }

        #endregion

        #region InternalMethodsForTesting

        internal Matrix<double> FormingAMatrixTst()
        {
            return FormingAMatrix();
        }

        internal Matrix<double> FormingYMatrixTst()
        {
            return FormingYMatrix();
        }

        internal Matrix<double> GetVectorWithTransformParametersTst(Matrix<double> aMatrix,
            Matrix<double> yMatrix)
        {
            return GetMatrixWithTransformParameters(aMatrix, yMatrix);
        }

        internal Matrix<double> GetVMatrixTst(Matrix<double> aMatrix, Matrix<double> yMatrix,
            Matrix<double> vecParamsMatrix)
        {
            return GetVMatrix(aMatrix, yMatrix, vecParamsMatrix);
        }

        internal double CalculateFCoefficientTst(Matrix<double> paramsTransformMatrix)
        {
            return CalculateFCoefficient(paramsTransformMatrix);
        }


        internal double CalculateMCoefficientTst(double fCoefficient)
        {
            return CalculateMCoefficient(fCoefficient);
        }

        internal Matrix<double> GetQMatrixTst(Matrix<double> aMatrix)
        {
            return GetQMatrix(aMatrix);
        }

        internal Vector<double> GetMeanSquareErrorsMatrixTst(Matrix<double> qMatrix, double mCoefficient)
        {
            return GetMeanSquareErrorsMatrix(qMatrix, mCoefficient);
        }

        #endregion
    }
}