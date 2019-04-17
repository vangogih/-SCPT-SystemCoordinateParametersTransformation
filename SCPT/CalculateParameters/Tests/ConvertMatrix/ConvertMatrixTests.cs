using System;
using System.Collections.Generic;
using Extreme.Mathematics;
using SCPT.Helper;
using SCPT.Transformation;
using Xunit;


internal partial class Incapsulation
{
    public class ConvertMatrixTests : BaseTest
    {
        private string _pathToTxt = PathToTest + "\\ConvertMatrix";

        [Fact]
        private void ConvertMatrix_ConvertRotMatrixWithoutMToRotMatrixWithM_ThrowsNullReferenceException()
        {
            Action test = () => ConvertMatrix.Convert_RotMatrixWithoutM_To_RotMatrixWithM(null, 0);

            Assert.Throws<NullReferenceException>(test);
        }

        [Fact]
        private void ConvertMatrix_ConvertRotMatrixWithMToRotMatrixWithoutM_ThrowsNullReferenceException()
        {
            Action test = () => ConvertMatrix.Convert_RotMatrixWithM_To_RotMatrixWithoutM(null);

            Assert.Throws<NullReferenceException>(test);
        }

        [Fact]
        private void ConvertMatrix_ConvertRotMatrixWithoutMToRotMatrixWithM_ThrowsArgumentException()
        {
            var testMatrix1x1 = Matrix.Create<double>(1, 1);
            var testMatrix3x4 = Matrix.Create<double>(3, 4);
            Action test1 = () => ConvertMatrix.Convert_RotMatrixWithoutM_To_RotMatrixWithM(testMatrix1x1, 0);
            Action test2 = () => ConvertMatrix.Convert_RotMatrixWithoutM_To_RotMatrixWithM(testMatrix3x4, 0);

            Assert.Throws<ArgumentException>(test1);
            Assert.Throws<ArgumentException>(test2);
        }

        [Fact]
        private void ConvertMatrix_ConvertRotMatrixWithMToRotMatrixWithoutM_ThrowsArgumentException()
        {
            var testMatrix1x1 = Matrix.Create<double>(1, 1);
            var testMatrix3x4 = Matrix.Create<double>(3, 4);
            Action test1 = () => ConvertMatrix.Convert_RotMatrixWithM_To_RotMatrixWithoutM(testMatrix1x1);
            Action test2 = () => ConvertMatrix.Convert_RotMatrixWithM_To_RotMatrixWithoutM(testMatrix3x4);

            Assert.Throws<ArgumentException>(test1);
            Assert.Throws<ArgumentException>(test2);
        }

        [Fact]
        private void ConvertMatrix_PassMisNaN_ThrowsArgumentException()
        {
            var testMatrix3x3 = Matrix.Create<double>(3, 3);

            Action test = () =>
                ConvertMatrix.Convert_RotMatrixWithoutM_To_RotMatrixWithM(testMatrix3x3, double.NaN);

            Assert.Throws<ArgumentException>(test);
        }

        [Fact]
        private void ConvertMatrix_PassMisInfinity_ThrowsArgumentException()
        {
            var testMatrix3x3 = Matrix.Create<double>(3, 3);

            Action test1 = () =>
                ConvertMatrix.Convert_RotMatrixWithoutM_To_RotMatrixWithM(testMatrix3x3,
                    double.NegativeInfinity);
            Action test2 = () =>
                ConvertMatrix.Convert_RotMatrixWithoutM_To_RotMatrixWithM(testMatrix3x3,
                    double.PositiveInfinity);

            Assert.Throws<ArgumentException>(test1);
            Assert.Throws<ArgumentException>(test2);
        }

        [Fact]
        private void ConvertMatrix_ConvertRotMatrixWithoutMToRotMatrixWithM_ValidRotMatrixWithM()
        {
            var listSrc = new List<Point>();
            var listDest = new List<Point>();
            FillListsCoordinationData(_pathToTxt, ref listSrc, ref listDest);

            var rotationMatrixExpected = ReadControlDataFromFile(_pathToTxt + "\\rotMatrixWithM.txt", 3, 3);

            var nip = new NewtonIterationProcess(listSrc, listDest);
            var rotMatrixWithMActual =
                ConvertMatrix.Convert_RotMatrixWithoutM_To_RotMatrixWithM(nip.RotationMatrix, nip.M);

            for (int row = 0; row < rotMatrixWithMActual.RowCount; row++)
            for (int col = 0; col < rotMatrixWithMActual.ColumnCount; col++)
                Assert.Equal(rotationMatrixExpected[row, col], rotMatrixWithMActual[row, col], 8);
        }

        [Fact]
        private void ConvertMatrix_Convert_RotMatrixWithMToRotMatrixWithoutM_ValidRotMatrixWithoutM()
        {
            var listSrc = new List<Point>();
            var listDest = new List<Point>();
            FillListsCoordinationData(_pathToTxt, ref listSrc, ref listDest);

            var rotationMatrixExpected = ReadControlDataFromFile(_pathToTxt + "\\rotMatrixWithoutM.txt", 3, 3);

            var nip = new NewtonIterationProcess(listSrc, listDest);
            var rotMatrixWithM =
                ConvertMatrix.Convert_RotMatrixWithoutM_To_RotMatrixWithM(nip.RotationMatrix, nip.M);
            var rotMatrixWithoutMActual =
                ConvertMatrix.Convert_RotMatrixWithM_To_RotMatrixWithoutM(rotMatrixWithM);


            for (int row = 0; row < rotMatrixWithoutMActual.RowCount; row++)
            for (int col = 0; col < rotMatrixWithoutMActual.ColumnCount; col++)
                Assert.Equal(rotationMatrixExpected[row, col], rotMatrixWithoutMActual[row, col], 8);
        }
    }
}