using MathNet.Numerics.LinearAlgebra;

namespace SCPT.Helper.VecParams
{
    internal class NIPVecParams : IVectorParameters
    {
        /// <inheritdoc />
        public RotationMatrix RotationMatrix { get; }

        /// <inheritdoc />
        public DeltaCoordinateMatrix DeltaCoordinateMatrix { get; }

        /// <inheritdoc />
        public double ScaleFactor { get; }

        public NIPVecParams(Vector<double> vec)
        {
            DeltaCoordinateMatrix = new DeltaCoordinateMatrix(vec[0], vec[1], vec[2]);
            RotationMatrix = new RotationMatrix(vec[3], vec[4], vec[5]);
            ScaleFactor = vec[6];
        }
    }
}