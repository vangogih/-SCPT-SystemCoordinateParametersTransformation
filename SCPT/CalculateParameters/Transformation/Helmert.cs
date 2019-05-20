using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using SCPT.Helper;

namespace SCPT.Transformation
{
    /// <summary>
    /// Helmert transformation from source (sc1) system coordinate to destination (sc2) system coordinate.
    /// <example>
    /// <code>
    /// var sc1Tosc2 =
    ///     new Helmert(sc1,sc2).FromSourceToDestination(delta, rotation, m);
    /// while you not call method FromSourceToDestination, no conversion. 
    /// </code>
    /// </example> 
    /// </summary>
    public class Helmert
    {
        private readonly SystemCoordinate _sourceSystemCoordinate;
        private readonly SystemCoordinate _destinationSystemCoordinate;

        private readonly DeltaCoordinateMatrix _deltaCoordinate;
        private readonly RotationMatrix _rotationMatrix;
        private readonly double _m;

        private bool _isTranformsByCoordinates;

        /// <inheritdoc cref="Helmert"/>
        /// <remarks>
        /// By this constructor you can create one instance with systems coordinate and transform
        /// with different parameters
        /// </remarks>
        /// <param name="sc1">Source system coordinate</param>
        /// <param name="sc2">Destination system coordinate</param>
        public Helmert(SystemCoordinate sc1, SystemCoordinate sc2)
        {
            _sourceSystemCoordinate = sc1;
            _destinationSystemCoordinate = sc2;
            _isTranformsByCoordinates = true;
        }

        
        /// <inheritdoc cref="Helmert"/>
        /// <remarks>
        /// By this constructor you can create one instance with parameters and transform
        /// with different source and destination system coordinate.
        /// </remarks>
        public Helmert(DeltaCoordinateMatrix coordinateMatrix, RotationMatrix rotationMatrix,
            double scale)
        {
            _deltaCoordinate = coordinateMatrix;
            _rotationMatrix = rotationMatrix;
            _m = scale;
            _isTranformsByCoordinates = false;
        }

        /// <summary>
        /// Transform from sc1 to sc2 with one list source and destination coordinates, but many parameters.
        /// </summary>
        /// <exception cref="ArgumentException">Throw then you instantiate class with delta parameters,
        /// and call method with transformation by parameters</exception>
        public List<Point> FromSourceToDestinationByParameters(DeltaCoordinateMatrix coordinateMatrix,
            RotationMatrix rotationMatrix,
            double scale)
        {
            if (!_isTranformsByCoordinates)
                throw new ArgumentException();
            
            return Transform(_sourceSystemCoordinate, _destinationSystemCoordinate, coordinateMatrix, rotationMatrix,
                scale);
        }

        /// <summary>
        /// Transform from sc1 to sc2 with one parameters, but many source and destination lists.  
        /// </summary>
        /// <exception cref="ArgumentException">Throw then you instantiate class with coordinates,
        /// and call method with transformation by coordinate</exception>
        public List<Point> FromSourceToDestinationBySystemsCoordinate(SystemCoordinate sc1, SystemCoordinate sc2)
        {
            if (_isTranformsByCoordinates)
                throw new ArgumentException();
            return Transform(sc1, sc2, _deltaCoordinate, _rotationMatrix, _m);
        }

        private List<Point> Transform(SystemCoordinate sc1, SystemCoordinate sc2,
            DeltaCoordinateMatrix coordinateMatrix, RotationMatrix rotationMatrix,
            double scale)
        {
            var interList = new List<Point>(sc1.List.Count);
            var rotWithM = scale * rotationMatrix.Matrix;
            var destVector = Vector<double>.Build.Dense(3);

            for (int row = 0; row < sc2.List.Count; row++)
            {
                destVector[0] = sc1.List[row].X;
                destVector[1] = sc1.List[row].Y;
                destVector[2] = sc1.List[row].Z;

                var interVector = rotWithM * destVector + coordinateMatrix.Vector;
                interList.Add(new Point(interVector[0], interVector[1], interVector[2]));
            }

            return interList;
        }
    }
}