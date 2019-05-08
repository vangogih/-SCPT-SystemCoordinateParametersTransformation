using MathNet.Numerics.LinearAlgebra;

namespace SCPT.Helper.VecParams
{
    internal class LPVecParams : IVectorParameters
    {
        /// <inheritdoc />
        public RotationMatrix RotationMatrix { get; }

        /// <inheritdoc />
        public DeltaCoordinateMatrix DeltaCoordinateMatrix { get; }

        /// <inheritdoc />
        public double ScaleFactor { get; }

        public LPVecParams(Vector<double> dxVector, Vector<double> dyVector, Vector<double> dzVector)
        {
            var rotMatrixWithM = FormingRotationMatrixWithM(dxVector, dyVector, dzVector);
            RotationMatrix = new RotationMatrix(rotMatrixWithM, true).Convert_RotMatrixWithM_To_RotMatrixWithoutM();
            DeltaCoordinateMatrix = new DeltaCoordinateMatrix(dxVector[3], dyVector[3], dzVector[3]);
            ScaleFactor = rotMatrixWithM[0, 0] - 1d;
        }

        private Matrix<double> FormingRotationMatrixWithM(Vector<double> dxVector, Vector<double> dyVector,
            Vector<double> dzVector)
        {
            var rotMatrix = Matrix<double>.Build.Dense(3, 3);

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
    }
}