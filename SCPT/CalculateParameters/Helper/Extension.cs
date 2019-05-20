using System;
using MathNet.Numerics.LinearAlgebra;

namespace SCPT.Helper
{
    internal static class Extension
    {
        public static void SetColumn(this Matrix<double> matrix, int col, double value)
        {
            for (int row = 0; row < matrix.RowCount; row++)
                matrix[row, col] = value;
        }
    }
}