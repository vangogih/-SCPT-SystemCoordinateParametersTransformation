using MathNet.Numerics.LinearAlgebra;

namespace SCPT.Helper.VecParams
{
    internal class A9VecParams : IVectorParameters
    {
        /// <inheritdoc />
        public RotationMatrix RotationMatrix { get; }

        /// <inheritdoc />
        public DeltaCoordinateMatrix DeltaCoordinateMatrix { get; }

        /// <inheritdoc />
        public double ScaleFactor { get; }

        public A9VecParams(Vector<double> dxVector)
        {
            RotationMatrix = new RotationMatrix(dxVector[6], dxVector[7], dxVector[8]);
            DeltaCoordinateMatrix = new DeltaCoordinateMatrix(dxVector[0], dxVector[1], dxVector[2]);
            ScaleFactor = (dxVector[3] + dxVector[4] + dxVector[5]) / 3;
        }
    }
}