using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace SCPT.Helper
{
    /// <summary>
    /// Representation Rotation matrix.
    /// Rotation Matrix can be two types:
    /// <code>
    /// 1. With scale factor (m)
    /// |1+m       wz*(1+m)  -wy*(1+m)|
    /// |-wz*(1+m) 1+m       wx*(1+m) |
    /// | wy*(1+m) -wx*(1+m) 1+m      |
    ///  
    /// 2. Without scale factor (m)
    /// |1  wz -wy|
    /// |-wz 1  wx|
    /// | wy -wx 1|
    /// </code>
    /// Different type transformation return different type Rotation Matrix.
    /// Be careful. And always check what 1) or 2) matrix you have.
    /// <example>
    /// <code>
    /// if (IsMatrixWithM)
    ///     var rotMat = instance.Matrix.Convert_RotMatrixWithM_To_RotMatrixWithoutM();
    /// else
    ///     var rotMat = instance.Matrix;
    /// </code>
    /// </example>
    /// </summary>
    public class RotationMatrix
    {
        /// <inheritdoc cref="AbstractTransformation.RotationMatrix"/>
        public Matrix<double> Matrix { get; }

        /// <summary>
        /// Rotation angle around X axis. Dimension is radians.
        /// </summary>
        public double Wx { get; }

        /// <summary>
        /// Rotation angle around Y axis. Dimension is radians
        /// </summary>
        public double Wy { get; }

        /// <summary>
        /// Rotation angle around Z axis. Dimension is radians
        /// </summary>
        public double Wz { get; }

        /// <summary>
        /// You always can check type matrix.
        /// True = with M, false = without M.
        /// </summary>
        public bool IsMatrixWithM { get; }

        /// <param name="rotationMatrix">source rotation matrix</param>
        /// <param name="isMatrixWithM">type source matrix <see cref="IsMatrixWithM"/></param>
        /// <exception cref="NullReferenceException">throw then matrix value is null</exception>
        /// <exception cref="ArgumentException">throw then: 1.matrix not square.  2. matrix not 3x3. 3. m is NaN or attained infinity.</exception>
        public RotationMatrix(Matrix<double> rotationMatrix, bool isMatrixWithM)
        {
            if (rotationMatrix == null)
                throw new NullReferenceException(nameof(rotationMatrix));
            if (rotationMatrix.ColumnCount != rotationMatrix.RowCount)
                throw new ArgumentException("Convertible matrix should be square matrix", nameof(rotationMatrix));
            if (rotationMatrix.ColumnCount != 3 || rotationMatrix.RowCount != 3)
                throw new ArgumentException("Convertible matrix should be order of 3 [3x3]", nameof(rotationMatrix));

            Matrix = rotationMatrix;
            IsMatrixWithM = isMatrixWithM;
            Wx = Matrix[1, 2];
            Wy = Matrix[2, 0];
            Wz = Matrix[0, 1];
        }

        /// <summary>
        /// Forming Rotation Matrix from wx,wy,wz parameters.
        /// <remarks>
        /// The matrix created in this way always type 2 (without m), see <see cref="RotationMatrix"/>
        /// </remarks> 
        /// </summary>
        /// <param name="wx"><see cref="Wx"/></param>
        /// <param name="wy"><see cref="Wy"/></param>
        /// <param name="wz"><see cref="Wz"/></param>
        /// <exception cref="ArgumentOutOfRangeException">throw then double parameters attained infinity</exception>
        /// <exception cref="ArgumentException">throw then double parameters is NaN</exception>
        public RotationMatrix(double wx, double wy, double wz)
        {
            if (double.IsInfinity(wx)) throw new ArgumentOutOfRangeException(nameof(wx) + "attained to infinity.");
            if (double.IsInfinity(wy)) throw new ArgumentOutOfRangeException(nameof(wy) + "attained to infinity.");
            if (double.IsInfinity(wz)) throw new ArgumentOutOfRangeException(nameof(wz) + "attained to infinity.");
            if (double.IsNaN(wx)) throw new ArgumentException(nameof(wx) + "Cannot be NaN");
            if (double.IsNaN(wy)) throw new ArgumentException(nameof(wy) + "Cannot be NaN");
            if (double.IsNaN(wz)) throw new ArgumentException(nameof(wz) + "Cannot be NaN");

            Wx = wx;
            Wy = wy;
            Wz = wz;
            IsMatrixWithM = false;
            Matrix = InitializeRotationMatrix(wx, wy, wz);
        }

        /// <param name="m">scale coefficient</param>
        /// <returns>3x3 rotation matrix multiply on (1+m)</returns>
        /// <exception cref="ArgumentException">throw then: m == NaN or m == +-infinity</exception>
        public RotationMatrix Convert_RotMatrixWithoutM_To_RotMatrixWithM(double m)
        {
            if (double.IsNaN(m))
                throw new ArgumentException("m cannot be NaN", nameof(m));
            if (double.IsInfinity(m))
                throw new ArgumentException("m attained infinity", nameof(m));

            // |1 wz -wy|    |1+m       wz*(1+m)  -wy*(1+m)|
            // |-wz 1 wx| => |-wz*(1+m) 1+m       wx*(1+m) |
            // | wy-wx 1|    | wy*(1+m) -wx*(1+m) 1+m      |
            return new RotationMatrix(Matrix * (1 + m), true);
        }

        /// <returns>3x3 matrix free from influence (1+m)</returns> 
        public RotationMatrix Convert_RotMatrixWithM_To_RotMatrixWithoutM()
        {
            // |1+m       wz*(1+m)  -wy*(1+m)|    |1  wz -wy|
            // |-wz*(1+m) 1+m       wx*(1+m) | => |-wz 1  wx|
            // | wy*(1+m) -wx*(1+m) 1+m      |    | wy -wx 1|

            try
            {
                var onePlusM = Matrix[0, 0];
                var wx = Matrix[1, 2] / onePlusM;
                var wy = Matrix[2, 0] / onePlusM;
                var wz = Matrix[0, 1] / onePlusM;
                var init = InitializeRotationMatrix(wx, wy, wz);
                return new RotationMatrix(init, false);
            }
            catch (DivideByZeroException e)
            {
                // |1 0 0|
                // |0 1 0|
                // |0 0 1|
                return new RotationMatrix(Matrix<double>.Build.DenseDiagonal(3, 3, 1), false);
            }
        }

        private Matrix<double> InitializeRotationMatrix(double wx, double wy, double wz)
        {
            // |1  wz -wy|
            // |-wz 1  wx|
            // | wy -wx 1|
            var rotationMatrix = Matrix<double>.Build.Dense(3, 3);
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