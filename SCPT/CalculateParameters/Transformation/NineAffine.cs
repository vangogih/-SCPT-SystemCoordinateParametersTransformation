using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using SCPT.Helper;

namespace SCPT.Transformation
{
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

        private Matrix<double> _baseRotMat;
        private Vector<double> _baseAlfaVector;
        private Matrix<double> _baseScaleMatrix;
        private Vector<double> _baseScaleVector;

        /// <inheritdoc />
        public NineAffine(SystemCoordinate srcListCord, SystemCoordinate destListCord) : base(srcListCord, destListCord)
        {
            SourceSystemCoordinates = srcListCord;
            DestinationSystemCoordinates = destListCord;

            CalculateParametersTransformation();
        }

        private void CalculateParametersTransformation()
        {
            _baseRotMat = Matrix<double>.Build.Dense(3, 3).SetDiagonal(1);
            _baseAlfaVector = Vector<double>.Build.Dense(3, 0);
            _baseScaleMatrix = Matrix<double>.Build.Dense(3, 3, 0).SetDiagonal(1);
            _baseScaleVector = Vector<double>.Build.Dense(3, 0);

            var eMatrix = CreateEMatrix();
            var fMatrix = CreateFMatrix();
            var gMatrix = CreateGMatrix();
            var currRotMatrix = CalculateRotationMatrix(eMatrix, fMatrix, gMatrix);
            var mMatrix = CreateMMatrix();
            var nMatrix = CreateNMatrix();
            var pMatrix = CreatePMatrix();
            var currScaleMatrix = CalculateScaleMatrix(mMatrix, nMatrix, pMatrix);
            
            var aMatrix = CreateAMatrix(currRotMatrix, currScaleMatrix);
        }

        #region PrivateMembers

        private Matrix<double> CreateEMatrix()
        {
            var eMat = Matrix<double>.Build.Dense(3, 3);
            eMat[0, 0] = 0;
            eMat[1, 0] = 0;
            eMat[2, 0] = 0;

            eMat[0, 1] = -_baseRotMat[0, 2];
            eMat[1, 1] = -_baseRotMat[1, 2];
            eMat[2, 1] = -_baseRotMat[2, 2];

            eMat[0, 2] = _baseRotMat[0, 1];
            eMat[1, 2] = -_baseRotMat[1, 1];
            eMat[2, 2] = _baseRotMat[2, 1];

            return eMat;
        }

        private Matrix<double> CreateFMatrix()
        {
            var fMat = Matrix<double>.Build.Dense(3, 3);
            fMat[0, 0] = -Math.Sin(_baseAlfaVector[1]) * Math.Cos(_baseAlfaVector[2]);
            fMat[1, 0] = Math.Sin(_baseAlfaVector[1]) * Math.Sin(_baseAlfaVector[2]);
            fMat[2, 0] = Math.Cos(_baseAlfaVector[1]);

            fMat[0, 1] = -_baseRotMat[2, 1] * Math.Cos(_baseAlfaVector[2]);
            fMat[1, 1] = _baseRotMat[2, 1] * Math.Sin(_baseAlfaVector[2]);
            fMat[2, 1] = Math.Sin(_baseAlfaVector[0]) * Math.Sin(_baseAlfaVector[1]);

            fMat[0, 2] = -_baseRotMat[2, 2] * Math.Cos(_baseAlfaVector[2]);
            fMat[1, 2] = _baseRotMat[2, 2] * Math.Sin(_baseAlfaVector[2]);
            fMat[2, 2] = -Math.Cos(_baseAlfaVector[0]) * Math.Sin(_baseAlfaVector[1]);
            return fMat;
        }

        private Matrix<double> CreateGMatrix()
        {
            var gMat = Matrix<double>.Build.Dense(3, 3);
            gMat[0, 0] = _baseRotMat[1, 0];
            gMat[1, 0] = -_baseRotMat[0, 0];
            gMat[2, 0] = 0;

            gMat[0, 1] = _baseRotMat[1, 1];
            gMat[1, 1] = -_baseRotMat[0, 1];
            gMat[2, 1] = 0;

            gMat[0, 2] = _baseRotMat[1, 2];
            gMat[1, 2] = -_baseRotMat[0, 2];
            gMat[2, 2] = 0;

            return gMat;
        }

        private Matrix<double> CalculateRotationMatrix(Matrix<double> eMatrix, Matrix<double> fMatrix,
            Matrix<double> gMatrix)
        {
            return _baseRotMat +
                   eMatrix * _baseAlfaVector[0] +
                   fMatrix * _baseAlfaVector[1] +
                   gMatrix * _baseAlfaVector[2];
        }

        private Matrix<double> CreateMMatrix()
        {
            var mMatrix = Matrix<double>.Build.Dense(3, 3, 0);
            mMatrix[0, 0] = 1;
            return mMatrix;
        }

        private Matrix<double> CreateNMatrix()
        {
            var nMatrix = Matrix<double>.Build.Dense(3, 3, 0);
            nMatrix[1, 1] = 1;
            return nMatrix;
        }

        private Matrix<double> CreatePMatrix()
        {
            var pMatrix = Matrix<double>.Build.Dense(3, 3, 0);
            pMatrix[2, 2] = 1;
            return pMatrix;
        }

        private Matrix<double> CalculateScaleMatrix(Matrix<double> mMatrix, Matrix<double> nMatrix,
            Matrix<double> pMatrix)
        {
            return _baseRotMat +
                   mMatrix * _baseScaleVector[0] +
                   nMatrix * _baseScaleVector[1] +
                   pMatrix * _baseScaleVector[2];
        }

        private Matrix<double> CreateAMatrix(Matrix<double> rotMatrix, Matrix<double> scaleMatrix)
        {
            return null;
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

        #endregion
    }
}