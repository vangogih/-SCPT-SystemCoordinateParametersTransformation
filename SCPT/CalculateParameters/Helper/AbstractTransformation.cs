using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using SCPT.Transformation;

namespace SCPT.Helper
{
    /// <summary>
    /// Abstract class for orderings formations classes with type transformations.
    /// <example>
    /// <code>
    /// For add new transformation type into library tou need: 
    /// 1. Create class with new type transformation (etc. AffineTransformationSevenParams) 
    /// 2. Take AbstractTransformation parent new class (new class inherited AbstractTransformation) 
    /// 3. Inherit constructor and redefine all fields (use key word "new").
    /// For more understanding look how formations classes:
    /// </code>
    /// <seealso cref="NewtonIterationProcess"/>
    /// <seealso cref="LinearProcedure"/>
    /// <seealso cref="SingularValueDecomposition"/>
    /// </example>
    /// </summary>
    public abstract class AbstractTransformation
    {
        /// <summary>
        /// list X,Y,Z source coordinates in maters dimension.
        /// </summary>
        protected List<Point> SourceSystemCoordinates { get; }

        /// <summary>
        /// list X,Y,Z destination coordinates in meters dimension. 
        /// </summary>
        protected List<Point> DestinationSystemCoordinates { get; }

        /// <summary>
        /// [3,3] matrix containing parameters of rotation source SC regarding destination SC.
        ///  All values has dimension in radians.
        ///  <example>
        /// |1  wz -wy|
        /// |-wz 1  wx|
        /// | wy -wx 1|
        /// </example>
        /// </summary>
        protected Matrix<double> RotationMatrix { get; private set; }

        /// <summary>
        /// [3,1] vector containing deltas (dx,dy,dz) of source SC regarding destination SC.
        ///  All values has dimension in meters.
        /// <example>
        /// [dx,dy,dz]
        /// </example>
        /// </summary>
        protected Vector<double> DeltaCoordinateMatrix { get; private set; }

        /// <summary>
        /// scale factor in ppm (1 + m)
        /// </summary>
        protected double M { get; private set; }

        /// <summary>
        /// [7,1] vector containing mean square errors parameters transformation.
        ///  m(dx),m(dy),m(dz) has dimension in meters.
        ///  m(wx),m(wy),m(wz) has dimension in radians.
        ///  m has dimension in ppm (1+m) 
        /// <example>
        /// [m(dx),m(dy),m(dz),m(wx),m(wy),m(wz),m(m)], where m(f) is mean square errors f value
        /// </example>
        /// </summary>
        protected Vector<double> MeanSquareErrorsMatrix { get; private set; }

        /// <param name="srcListCord">list source coordinates which will be translated to destination coordinates</param>
        /// <param name="destListCord">list destination coordinates</param>
        /// <exception cref="NullReferenceException">throw then source list and destination list reference is null</exception>
        /// <exception cref="ArgumentException">throw then source list and destination list have different length</exception>
        protected AbstractTransformation(List<Point> srcListCord, List<Point> destListCord)
        {
            if (srcListCord == null)
                throw new NullReferenceException("source list cannot be null");
            if (destListCord == null)
                throw new NullReferenceException("destination list cannot be null");
            if (srcListCord.Count != destListCord.Count)
                throw new ArgumentException("source and destination lists must be the same length");
        }   
    }
}