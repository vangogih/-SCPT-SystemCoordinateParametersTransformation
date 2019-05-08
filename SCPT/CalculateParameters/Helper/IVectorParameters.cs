namespace SCPT.Helper
{
    /// <summary>
    /// <code>
    /// In various methods of conversion obtained in various vector conversion settings.
    /// To separate the different generation logic different matrix (rotation, delta, scale)
    /// this all General information has been brought to this interface.
    /// If the conversion method you are adding requires that the output be converted in some way,
    /// please create a new class that will implement this interface.
    /// </code>
    /// <seealso cref="SCPT.Helper.VecParams.A9VecParams"/>
    /// <seealso cref="SCPT.Helper.VecParams.LPVecParams"/>
    /// <seealso cref="SCPT.Helper.VecParams.NIPVecParams"/>
    /// </summary>
    public interface IVectorParameters
    {
        /// <inheritdoc cref="AbstractTransformation.RotationMatrix"/>
        RotationMatrix RotationMatrix { get; }

        /// <inheritdoc cref="AbstractTransformation.DeltaCoordinateMatrix"/>
        DeltaCoordinateMatrix DeltaCoordinateMatrix { get; }

        /// <inheritdoc cref="AbstractTransformation.M"/> 
        double ScaleFactor { get; }
    }
}