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
    }
}