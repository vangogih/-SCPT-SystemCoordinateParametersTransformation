using System;
using System.Collections;
using System.Collections.Generic;
using Extreme.Mathematics;
using Extreme.Mathematics.LinearAlgebra;

namespace CalculateParameters
{
    public class NewtonIterationProcess : ITransform
    {
        private const int MinListCount = 3;

        private readonly List<SystemCoordinate> _sourceSystemCoordinates;
        private readonly List<SystemCoordinate> _destinationSystemCoordinates;

        public double Wx { get; set; }
        public double Wy { get; set; }
        public double Wz { get; set; }
        public double M { get; set; }

        public NewtonIterationProcess(List<SystemCoordinate> source, List<SystemCoordinate> destination)
        {
            if (source == null)
                throw new NullReferenceException("source list cannot be null");
            if (destination == null)
                throw new NullReferenceException("destination list cannot be null");
            if (source.Count <= MinListCount)
                throw new ArgumentException("source list count cannot be less when 4");
            if (destination.Count <= MinListCount)
                throw new ArgumentException("source list count cannot be less when 4");
            if (source.Count != destination.Count)
                throw new ArgumentException("source list and destination list must be of the same length");

            _sourceSystemCoordinates = source;
            _destinationSystemCoordinates = destination;

            Wx = 0;
            Wy = 0;
            Wz = 0;
            M = 0;
        }

        public void CalculateTransformationParameters()
        {
            var srcCordMatrix = FormingCoordinateMatrix(_sourceSystemCoordinates);
            var destCordMatrix = FormingCoordinateMatrix(_destinationSystemCoordinates);
            
            var aMatrix = FormingAMatrix();
            var yMatrix = FormingYMatrix();

            var vecParams = GetVectorWithTransformParameters(aMatrix, yMatrix);
        }

        private DenseMatrix<double> FormingCoordinateMatrix(List<SystemCoordinate> list)
        {
            var vectorMatrix = Matrix.Create<double>(list.Count, 3); // matrix [nx3]
            for (int i = 0; i < list.Count; i++)
            {
                vectorMatrix[i, 0] = list[i].X;
                vectorMatrix[i, 1] = list[i].Y;
                vectorMatrix[i, 2] = list[i].Z;
            }

            return vectorMatrix;
        }

        private DenseMatrix<double> FormingAMatrix()
        {
            var aMatrix = Matrix.Create<double>(_sourceSystemCoordinates.Count * 3, 7);

            for (int matrixIndex = 0, listIndex = 0;
                matrixIndex < _sourceSystemCoordinates.Count * 3 || listIndex < _sourceSystemCoordinates.Count;
                matrixIndex += 3, listIndex++)
            {
                aMatrix[matrixIndex, 0] = 1;
                aMatrix[matrixIndex, 1] = 0;
                aMatrix[matrixIndex, 2] = 0;
                aMatrix[matrixIndex, 3] = 0;
                aMatrix[matrixIndex, 4] = -_sourceSystemCoordinates[listIndex].Z;
                aMatrix[matrixIndex, 5] = _sourceSystemCoordinates[listIndex].Y;
                aMatrix[matrixIndex, 6] = _sourceSystemCoordinates[listIndex].X;

                aMatrix[matrixIndex + 1, 0] = 0;
                aMatrix[matrixIndex + 1, 1] = 1;
                aMatrix[matrixIndex + 1, 2] = 0;
                aMatrix[matrixIndex + 1, 3] = _sourceSystemCoordinates[listIndex].Z;
                aMatrix[matrixIndex + 1, 4] = 0;
                aMatrix[matrixIndex + 1, 5] = -_sourceSystemCoordinates[listIndex].X;
                aMatrix[matrixIndex + 1, 6] = _sourceSystemCoordinates[listIndex].Y;

                aMatrix[matrixIndex + 2, 0] = 0;
                aMatrix[matrixIndex + 2, 1] = 0;
                aMatrix[matrixIndex + 2, 2] = 1;
                aMatrix[matrixIndex + 2, 3] = -_sourceSystemCoordinates[listIndex].Y;
                aMatrix[matrixIndex + 2, 4] = _sourceSystemCoordinates[listIndex].X;
                aMatrix[matrixIndex + 2, 5] = 0;
                aMatrix[matrixIndex + 2, 6] = _sourceSystemCoordinates[listIndex].Z;
            }

            return aMatrix;
        }

        private DenseMatrix<double> FormingYMatrix()
        {
            var yMatrix = Matrix.Create<double>(_sourceSystemCoordinates.Count * 3, 1);

            for (int matrixIndex = 0, listIndex = 0;
                matrixIndex < _sourceSystemCoordinates.Count * 3 && listIndex < _sourceSystemCoordinates.Count;
                matrixIndex += 3, listIndex++)
            {
                yMatrix[matrixIndex, 0] =
                    _destinationSystemCoordinates[listIndex].X - _sourceSystemCoordinates[listIndex].X;
                yMatrix[matrixIndex + 1, 0] =
                    _destinationSystemCoordinates[listIndex].Y - _sourceSystemCoordinates[listIndex].Y;
                yMatrix[matrixIndex + 2, 0] =
                    _destinationSystemCoordinates[listIndex].Z - _sourceSystemCoordinates[listIndex].Z;
            }

            return yMatrix;
        }

        private DenseMatrix<double> GetVectorWithTransformParameters(Matrix<double> aMatrix, Matrix<double> yMatrix)
        {
            var currPMatrix = Matrix.Create<double>(7, 1);
            var prevPMatrix = Matrix.Create<double>(7, 1);
            for (int i = 0; i < prevPMatrix.RowCount; i++)
                prevPMatrix[i, 0] = double.MaxValue;

            while (IsSubtractMatrixValuesLessWhenDelta(prevPMatrix, currPMatrix, 0))
            {
                prevPMatrix = currPMatrix;
                // Pi = P(i-1) - ((AT*A)^-1 * AT * Y) *  (A * P(i-1) - Y)
                var AT = aMatrix.Transpose();
                var aInverse = Matrix.Multiply(AT, aMatrix).GetInverse();
                var dX = Matrix.Subtract(Matrix.Multiply(aMatrix, prevPMatrix), yMatrix);
                var pMatrix = Matrix.Subtract(prevPMatrix, Matrix.Multiply(Matrix.Multiply(aInverse, AT), dX));
                currPMatrix = (DenseMatrix<double>) pMatrix;
            }

            return currPMatrix;
        }

        private bool IsSubtractMatrixValuesLessWhenDelta(Matrix<double> first, Matrix<double> second,
            double delta)
        {
            var subtract = Matrix.Subtract(first, second);
            for (int i = 0; i < second.RowCount; i++)
                if (subtract[i, 0] < delta)
                    return false;
            return true;
        }

        #region InternalMethodsForTesting

        internal DenseMatrix<double> FormingCoordinateMatrixTst(List<SystemCoordinate> list)
        {
            return FormingCoordinateMatrix(list);
        }

        internal DenseMatrix<double> FormingAMatrixTst()
        {
            return FormingAMatrix();
        }

        internal DenseMatrix<double> FormingYMatrixTst()
        {
            return FormingYMatrix();
        }

        internal DenseMatrix<double> GetVectorWithTransformParametersTst(DenseMatrix<double> aMatrix,
            DenseMatrix<double> yMatrix)
        {
            return GetVectorWithTransformParameters(aMatrix, yMatrix);
        }

        #endregion
    }

    public interface ITransform
    {
        double Wx { get; set; }
        double Wy { get; set; }
        double Wz { get; set; }
        double M { get; set; }
        void CalculateTransformationParameters();
    }
}