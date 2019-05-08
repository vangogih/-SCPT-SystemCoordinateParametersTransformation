using System;
using MathNet.Numerics.LinearAlgebra;

namespace SCPT.Helper
{
    internal static class Extension
    {
        // TODO write Helmert transformation

        /// <summary>
        /// Extension on base Matrix where all diagonal elements set equal input value.
        /// </summary>
        /// <exception cref="ArgumentException">Throw then input matrix not square matrix</exception>
        public static Matrix<double> SetDiagonal(this Matrix<double> matrix, double value)
        {
            if (matrix.RowCount != matrix.ColumnCount)
                throw new ArgumentException("Matrix will be a square", nameof(matrix));
            var val = matrix;
            for (int row = 0; row < matrix.RowCount; row++)
            for (int col = 0; col < matrix.ColumnCount; col++)
                if (row == col)
                    val[row, col] = value;
            return matrix;
        }

        public static void SetColumn(this Matrix<double> matrix, int col, double value)
        {
            for (int row = 0; row < matrix.RowCount; row++)
                matrix[row, col] = value;
        }
    }
}