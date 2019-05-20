using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using SCPT.Helper;
using SCPT.Helper.VecParams;

namespace SCPT.Transformation
{
    /// <summary>
    /// <code>
    /// This transformation is not much different from NIP, except that the scale factor
    /// is divided on 3 components: m1,m2,m3. Here, too, the calculation of the parameters is
    /// carried out using the least squares method, only different the method of forming the matrix A.
    /// In NIP, these were partial derivatives of the helmert formula, and here we take the partial
    /// derivatives of the result of multiplying the reversal matrices for each of the axes.
    /// </code>
    /// </summary>
    public class NineAffine : AbstractTransformation
    {
        /// <inheritdoc />
        public override SystemCoordinate SourceSystemCoordinates { get; }

        /// <inheritdoc />
        public override SystemCoordinate DestinationSystemCoordinates { get; }

        /// <inheritdoc />
        public override RotationMatrix RotationMatrix { get; }

        /// <inheritdoc />
        public override DeltaCoordinateMatrix DeltaCoordinateMatrix { get; }

        /// <inheritdoc />
        public override double M { get; }


        private Matrix<double> _prevRotMat;
        private Matrix<double> _currRotMat;
        private Vector<double> _baseAlfaVector;
        private Matrix<double> _baseScaleMatrix;
        private Vector<double> _baseScaleVector;


        /// <inheritdoc />
        public NineAffine(SystemCoordinate srcListCord, SystemCoordinate destListCord) : base(srcListCord, destListCord)
        {
            SourceSystemCoordinates = srcListCord;
            DestinationSystemCoordinates = destListCord;

            var dxVector = Iteration();
            IVectorParameters helper = new A9VecParams(dxVector);
            RotationMatrix = helper.RotationMatrix;
            DeltaCoordinateMatrix = helper.DeltaCoordinateMatrix;
            M = helper.ScaleFactor;
        }

        private Vector<double> Iteration()
        {
            var iterationCount = 0;
            _prevRotMat = _currRotMat = Matrix<double>.Build.DenseDiagonal(3, 3, 1);
            _baseAlfaVector = Vector<double>.Build.Dense(3, 0);
            _baseScaleMatrix = Matrix<double>.Build.DenseDiagonal(3, 3, 1);
            _baseScaleVector = Vector<double>.Build.Dense(3, 0);
            Vector<double> currDxVector;
            var lVector = Vector<double>.Build.Dense(9, 0);

            do
            {
                var eMatrix = CreateEMatrix();
                var fMatrix = CreateFMatrix();
                var gMatrix = CreateGMatrix();
                var currRotMatrix = CalculateRotationMatrix(eMatrix, fMatrix, gMatrix);
                var mMatrix = CreateMMatrix();
                var nMatrix = CreateNMatrix();
                var pMatrix = CreatePMatrix();
                var currScaleMatrix = CalculateScaleMatrix(mMatrix, nMatrix, pMatrix);

                var aMatrix = CreateAMatrix(currRotMatrix, currScaleMatrix);

                if (iterationCount == 0)
                    lVector = CreateLVector(currRotMatrix, currScaleMatrix);

                currDxVector = CreateDxVector(aMatrix, lVector);

                _prevRotMat = _currRotMat;
                _currRotMat = currRotMatrix;
                _baseScaleMatrix = currScaleMatrix;
                _baseAlfaVector[0] = currDxVector[6];
                _baseAlfaVector[1] = currDxVector[7];
                _baseAlfaVector[2] = currDxVector[8];
                _baseScaleVector[0] = currDxVector[3];
                _baseScaleVector[1] = currDxVector[4];
                _baseScaleVector[2] = currDxVector[5];
            } while (iterationCount++ < 3);

            return currDxVector;
        }

        #region PrivateMembers

        private Matrix<double> CreateEMatrix()
        {
            var eMat = Matrix<double>.Build.Dense(3, 3);
            eMat[0, 0] = 0;
            eMat[1, 0] = 0;
            eMat[2, 0] = 0;

            eMat[0, 1] = -_prevRotMat[0, 2];
            eMat[1, 1] = -_prevRotMat[1, 2];
            eMat[2, 1] = -_prevRotMat[2, 2];

            eMat[0, 2] = _prevRotMat[0, 1];
            eMat[1, 2] = -_prevRotMat[1, 1];
            eMat[2, 2] = _prevRotMat[2, 1];

            return eMat;
        }

        private Matrix<double> CreateFMatrix()
        {
            var fMat = Matrix<double>.Build.Dense(3, 3);

            fMat[0, 0] = -Math.Sin(_baseAlfaVector[1]) * Math.Cos(_baseAlfaVector[2]);
            fMat[1, 0] = Math.Sin(_baseAlfaVector[1]) * Math.Sin(_baseAlfaVector[2]);
            fMat[2, 0] = Math.Cos(_baseAlfaVector[1]);

            fMat[0, 1] = -_prevRotMat[2, 1] * Math.Cos(_baseAlfaVector[2]);
            fMat[1, 1] = _prevRotMat[2, 1] * Math.Sin(_baseAlfaVector[2]);
            fMat[2, 1] = Math.Sin(_baseAlfaVector[0]) * Math.Sin(_baseAlfaVector[1]);

            fMat[0, 2] = -_prevRotMat[2, 2] * Math.Cos(_baseAlfaVector[2]);
            fMat[1, 2] = _prevRotMat[2, 2] * Math.Sin(_baseAlfaVector[2]);
            fMat[2, 2] = -Math.Cos(_baseAlfaVector[0]) * Math.Sin(_baseAlfaVector[1]);

            return fMat;
        }

        private Matrix<double> CreateGMatrix()
        {
            var gMat = Matrix<double>.Build.Dense(3, 3);
            gMat[0, 0] = _prevRotMat[1, 0];
            gMat[1, 0] = -_prevRotMat[0, 0];
            gMat[2, 0] = 0;

            gMat[0, 1] = _prevRotMat[1, 1];
            gMat[1, 1] = -_prevRotMat[0, 1];
            gMat[2, 1] = 0;

            gMat[0, 2] = _prevRotMat[1, 2];
            gMat[1, 2] = -_prevRotMat[0, 2];
            gMat[2, 2] = 0;

            return gMat;
        }

        private Matrix<double> CalculateRotationMatrix(Matrix<double> eMatrix, Matrix<double> fMatrix,
            Matrix<double> gMatrix)
        {
            var diagonalRotMatrix = Matrix<double>.Build.DenseDiagonal(3, 3, 1);
            return diagonalRotMatrix +
                   (eMatrix * _baseAlfaVector[0] +
                    fMatrix * _baseAlfaVector[1] +
                    gMatrix * _baseAlfaVector[2]);
        }

        private Matrix<double> CreateMMatrix()
        {
            var mMatrix = Matrix<double>.Build.Dense(3, 3, 0);
            mMatrix[0, 0] = 1 + _baseScaleVector[0];
            return mMatrix;
        }

        private Matrix<double> CreateNMatrix()
        {
            var nMatrix = Matrix<double>.Build.Dense(3, 3, 0);
            nMatrix[1, 1] = 1 + _baseScaleVector[1];
            return nMatrix;
        }

        private Matrix<double> CreatePMatrix()
        {
            var pMatrix = Matrix<double>.Build.Dense(3, 3, 0);
            pMatrix[2, 2] = 1 + _baseScaleVector[2];
            return pMatrix;
        }

        private Matrix<double> CalculateScaleMatrix(Matrix<double> mMatrix, Matrix<double> nMatrix,
            Matrix<double> pMatrix)
        {
            var diagonalScaleMatrix = Matrix<double>.Build.DenseDiagonal(3, 3, 1);
            return diagonalScaleMatrix +
                   (mMatrix * _baseScaleVector[0] +
                    nMatrix * _baseScaleVector[1] +
                    pMatrix * _baseScaleVector[2]);
        }

        private Matrix<double> CreateAMatrix(Matrix<double> rotMatrix, Matrix<double> scaleMatrix)
        {
            var m1 = _baseScaleMatrix[0, 0];
            var m2 = _baseScaleMatrix[1, 1];
            var m3 = _baseScaleMatrix[2, 2];

            var aMatrix = Matrix<double>.Build.Dense(SourceSystemCoordinates.List.Count * 3, 9);
            var srcVector = SourceSystemCoordinates.Vector;

            for (int row = 0; row < aMatrix.RowCount; row += 3)
            {
                var x = srcVector[row];
                var y = srcVector[row + 1];
                var z = srcVector[row + 2];

                // col 1 
                aMatrix[row, 0] = 1;
                aMatrix[row + 1, 0] = 0;
                aMatrix[row + 2, 0] = 0;

                // col 2
                aMatrix[row, 1] = 0;
                aMatrix[row + 1, 1] = 1;
                aMatrix[row + 2, 1] = 0;

                // col 3
                aMatrix[row, 2] = 0;
                aMatrix[row + 1, 2] = 0;
                aMatrix[row + 2, 2] = 1;

                // col 4
                aMatrix[row, 3] = rotMatrix[0, 0] * x;
                aMatrix[row + 1, 3] = rotMatrix[1, 0] * x;
                aMatrix[row + 2, 3] = rotMatrix[2, 0] * x;

                // col 5
                aMatrix[row, 4] = rotMatrix[0, 1] * y;
                aMatrix[row + 1, 4] = rotMatrix[1, 1] * y;
                aMatrix[row + 2, 4] = rotMatrix[2, 1] * y;

                // col 6
                aMatrix[row, 5] = rotMatrix[0, 2] * z;
                aMatrix[row + 1, 5] = rotMatrix[1, 2] * z;
                aMatrix[row + 2, 5] = rotMatrix[2, 2] * z;

                // col 7
                aMatrix[row, 6] = -m2 * rotMatrix[0, 2] * y +
                                  m3 * rotMatrix[0, 1] * z;
                aMatrix[row + 1, 6] = -m2 * rotMatrix[1, 2] * y +
                                      m3 * rotMatrix[1, 1] * z;
                aMatrix[row + 2, 6] = -m2 * rotMatrix[2, 2] * y +
                                      m3 * rotMatrix[2, 1] * z;

                // col 8
                aMatrix[row, 7] = -Math.Cos(_baseAlfaVector[2]) *
                                  (m1 * Math.Sin(_baseAlfaVector[1]) * x +
                                   m2 * rotMatrix[2, 1] * y +
                                   m3 * rotMatrix[2, 2] * z);
                aMatrix[row + 1, 7] = Math.Sin(_baseAlfaVector[2]) *
                                      (m1 * Math.Sin(_baseAlfaVector[1]) * x +
                                       m2 * rotMatrix[2, 1] * y +
                                       m3 * rotMatrix[2, 2] * z);
                aMatrix[row + 2, 7] = m1 * Math.Cos(_baseAlfaVector[1]) * x +
                                      m2 * Math.Sin(_baseAlfaVector[0]) * Math.Sin(_baseAlfaVector[1]) * y -
                                      m3 * Math.Cos(_baseAlfaVector[0]) * Math.Sin(_baseAlfaVector[1]) * z;

                // col 9
                aMatrix[row, 8] = m1 * rotMatrix[1, 0] * x + m2 * rotMatrix[1, 1] * y + m3 * rotMatrix[1, 2] * z;
                aMatrix[row + 1, 8] = -(m1 * rotMatrix[0, 0] * x + m2 * rotMatrix[0, 1] * y + m3 * rotMatrix[0, 2] * z);
                aMatrix[row + 2, 8] = 0;
            }


            return aMatrix;
        }


        private Vector<double> CreateLVector(Matrix<double> rotMatrix, Matrix<double> scaleMatrix)
        {
            var lVector = Vector<double>.Build.Dense(SourceSystemCoordinates.Vector.Count);

            var m1 = scaleMatrix[0, 0];
            var m2 = scaleMatrix[1, 1];
            var m3 = scaleMatrix[2, 2];

            for (int row = 0; row < lVector.Count; row += 3)
            {
                lVector[row] = DestinationSystemCoordinates.Vector[row] -
                               (m1 * rotMatrix[0, 0] * SourceSystemCoordinates.Vector[row] +
                                m2 * rotMatrix[0, 1] * SourceSystemCoordinates.Vector[row + 1] +
                                m3 * rotMatrix[0, 2] * SourceSystemCoordinates.Vector[row + 2]);
                lVector[row + 1] = DestinationSystemCoordinates.Vector[row + 1] -
                                   (m1 * rotMatrix[1, 0] * SourceSystemCoordinates.Vector[row] +
                                    m2 * rotMatrix[1, 1] * SourceSystemCoordinates.Vector[row + 1] +
                                    m3 * rotMatrix[1, 2] * SourceSystemCoordinates.Vector[row + 2]);
                lVector[row + 2] = DestinationSystemCoordinates.Vector[row + 2] -
                                   (m1 * rotMatrix[2, 0] * SourceSystemCoordinates.Vector[row] +
                                    m2 * rotMatrix[2, 1] * SourceSystemCoordinates.Vector[row + 1] +
                                    m3 * rotMatrix[2, 2] * SourceSystemCoordinates.Vector[row + 2]);
            }

            return lVector;
        }

        private Vector<double> CreateDxVector(Matrix<double> aMatrix, Vector<double> lVector)
        {
            var dxVector = Vector<double>.Build.Dense(9);

            dxVector = aMatrix.TransposeThisAndMultiply(aMatrix).Inverse() * aMatrix.Transpose() * lVector;

            return dxVector;
        }

        #endregion

        #region InternalmembersForTests

        internal Matrix<double> CreateEMatrixTst()
        {
            return CreateEMatrix();
        }


        internal Matrix<double> CreateFMatrixTst()
        {
            return CreateFMatrix();
        }

        internal Matrix<double> CreateGMatrixTst()
        {
            return CreateGMatrix();
        }

        internal Matrix<double> CalculateRotationMatrixTst(Matrix<double> eMatrix, Matrix<double> fMatrix,
            Matrix<double> gMatrix)
        {
            return CalculateRotationMatrix(eMatrix, fMatrix, gMatrix);
        }

        internal Matrix<double> CreateMMatrixTst()
        {
            return CreateMMatrix();
        }


        internal Matrix<double> CreateNMatrixTst()
        {
            return CreateNMatrix();
        }


        internal Matrix<double> CreatePMatrixTst()
        {
            return CreatePMatrix();
        }

        internal Matrix<double> CalculateScaleMatrixTst(Matrix<double> mMatrix, Matrix<double> nMatrix,
            Matrix<double> pMatrix)
        {
            return CalculateRotationMatrix(mMatrix, nMatrix, pMatrix);
        }

        internal Matrix<double> CreateAMatrixTst(Matrix<double> rotMatrix, Matrix<double> scaleMatrix)
        {
            return CreateAMatrix(rotMatrix, scaleMatrix);
        }


        internal Vector<double> CreateLVectorTst(Matrix<double> rotMatrix, Matrix<double> scaleMatrix)
        {
            return CreateLVector(rotMatrix, scaleMatrix);
        }

        internal Vector<double> CreateDxVectorTst(Matrix<double> aMatrix, Vector<double> lVector)
        {
            return CreateDxVector(aMatrix, lVector);
        }

        #endregion
    }
}