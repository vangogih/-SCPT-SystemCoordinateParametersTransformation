using System.Collections.Generic;
using Extreme.Mathematics;
using SCPT.Helper;

namespace SCPT.Transformation
{
    
    // TODO add transformation founded on Linear Procedure
    public sealed class LinearProcedure : AbstractTransformation
    {
        /// <summary>
        /// <inheritdoc cref="AbstractTransformation.SourceSystemCoordinates" />
        /// </summary>
        public new List<Point> SourceSystemCoordinates { get; }

        /// <summary>
        /// <inheritdoc cref="AbstractTransformation.DestinationSystemCoordinates"/>
        /// </summary>
        public new List<Point> DestinationSystemCoordinates { get; }

        /// <summary>
        /// <inheritdoc cref="AbstractTransformation.RotationMatrix"/>
        /// </summary>
        public new Matrix<double> RotationMatrix { get; private set; }

        /// <summary>
        /// <inheritdoc cref="AbstractTransformation.DeltaCoordinateMatrix"/>
        /// </summary>
        public new Vector<double> DeltaCoordinateMatrix { get; private set; }

        /// <summary>
        /// <inheritdoc cref="AbstractTransformation.M"/>
        /// </summary>
        public new double M { get; private set; }

        public LinearProcedure(List<Point> srcListCord, List<Point> destListCord) : base(srcListCord, destListCord)
        {
        }
    }
}