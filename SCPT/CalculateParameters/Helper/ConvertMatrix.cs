using System;
using Extreme.Mathematics;

namespace SCPT.Helper
{
    /// <summary>
    /// In someone calculations can result (1+m)*rotationMatrix, or rotation matrix.
    /// This helper class to convert on type matrix to another. 
    /// </summary>
    public static class ConvertMatrix
    {
        internal static Vector<double> VectorParametersToDeltaCoordinate(Matrix<double> convertMatrix)
        {
            var result = Matrix.Create<double>(3, 1);
            var dx = convertMatrix[0, 0];
            var dy = convertMatrix[1, 0];
            var dz = convertMatrix[1, 0];

            result[0, 0] = dx;
            result[1, 0] = dy;
            result[2, 0] = dz;

            return result.ReshapeAsVector();
        }

        internal static Matrix<double> VectorParametersToRotateMatrix(Matrix<double> convertMatrix)
        {
            var wx = convertMatrix[3, 0];
            var wy = convertMatrix[4, 0];
            var wz = convertMatrix[5, 0];

            return InitializeRotationMatrix(wx, wy, wz);
        }


        /// <param name="convertMatrix">3x3 rotation matrix multiply on (1+m)</param>
        /// <param name="m">scale coefficient</param>
        /// <returns>3x3 rotation matrix multiply on (1+m)</returns>
        /// <exception cref="NullReferenceException">throw then matrix value is null</exception>
        /// <exception cref="ArgumentException">throw then: 1.matrix not square.  2. matrix not 3x3. 3. m is NaN or attained infinity.</exception>
        public static Matrix<double> Convert_RotMatrixWithoutM_To_RotMatrixWithM(Matrix<double> convertMatrix, double m)
        {
            if (double.IsNaN(m))
                throw new ArgumentException("m cannot be NaN");
            if (double.IsInfinity(m))
                throw new ArgumentException("m attained infinity");

            CheckCorrectInputMatrix(convertMatrix);

            // |1 wz -wy|    |1+m       wz*(1+m)  -wy*(1+m)|
            // |-wz 1 wx| => |-wz*(1+m) 1+m       wx*(1+m) |
            // | wy-wx 1|    | wy*(1+m) -wx*(1+m) 1+m      |
            return convertMatrix * (1 + m);
        }

        /// <param name="convertMatrix">3x3 rotation matrix multiply on (1+m)</param>
        /// <returns>3x3 matrix free from influence (1+m)</returns>
        /// <exception cref="NullReferenceException">throw then matrix value is null</exception>
        /// <exception cref="ArgumentException">throw then matrix not square and 3x3</exception> 
        public static Matrix<double> Convert_RotMatrixWithM_To_RotMatrixWithoutM(Matrix<double> convertMatrix)
        {
            CheckCorrectInputMatrix(convertMatrix);

            // |1+m       wz*(1+m)  -wy*(1+m)|    |1  wz -wy|
            // |-wz*(1+m) 1+m       wx*(1+m) | => |-wz 1  wx|
            // | wy*(1+m) -wx*(1+m) 1+m      |    | wy -wx 1|

            var onePlusM = convertMatrix[0, 0];
            var wx = convertMatrix[1, 2] / onePlusM;
            var wy = convertMatrix[2, 0] / onePlusM;
            var wz = convertMatrix[0, 1] / onePlusM;

            return InitializeRotationMatrix(wx, wy, wz);
        }


        private static void CheckCorrectInputMatrix(Matrix<double> matrix)
        {
            if (matrix == null)
                throw new NullReferenceException("input matrix cannot be null");
            if (matrix.ColumnCount != matrix.RowCount)
                throw new ArgumentException("convertible matrix should be square matrix");
            if (matrix.ColumnCount != 3 || matrix.RowCount != 3)
                throw new ArgumentException("convertible matrix should be order of 3 [3x3]");
        }

        private static Matrix<double> InitializeRotationMatrix(double wx, double wy, double wz)
        {
            // |1  wz -wy|
            // |-wz 1  wx|
            // | wy -wx 1|
            var rotationMatrix = Matrix.Create<double>(3, 3);
            rotationMatrix[0, 0] = 1;
            rotationMatrix[0, 1] = wz;
            rotationMatrix[0, 2] = -wy;

            rotationMatrix[1, 0] = -wz;
            rotationMatrix[1, 1] = 1;
            rotationMatrix[1, 2] = wx;

            rotationMatrix[2, 0] = wy;
            rotationMatrix[2, 1] = -wx;
            rotationMatrix[2, 2] = 1;
            return rotationMatrix;
        }
    }
}