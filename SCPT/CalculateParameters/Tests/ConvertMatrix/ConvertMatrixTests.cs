using System;
using System.Collections.Generic;
using System.IO;
using Extreme.Mathematics;
using Extreme.Mathematics.LinearAlgebra;
using SCPT.Helper;
using SCPT.Transformation;
using Xunit;

namespace SCPT.Tests.ConvertMatrix
{
    public class ConvertMatrixTests
    {
        private const string Path =
            "D:\\RiderProjects\\-SCPT-SystemCoordinateParametersTransformation\\SCPT\\CalculateParameters\\Tests\\ConvertMatrix";

        [Fact]
        private void ConvertMatrix_ConvertRotMatrixWithoutMToRotMatrixWithM_ThrowsNullReferenceException()
        {
            Action test = () => Helper.ConvertMatrix.Convert_RotMatrixWithoutM_To_RotMatrixWithM(null, 0);

            Assert.Throws<NullReferenceException>(test);
        }

        [Fact]
        private void ConvertMatrix_ConvertRotMatrixWithMToRotMatrixWithoutM_ThrowsNullReferenceException()
        {
            Action test = () => Helper.ConvertMatrix.Convert_RotMatrixWithM_To_RotMatrixWithoutM(null);

            Assert.Throws<NullReferenceException>(test);
        }

        [Fact]
        private void ConvertMatrix_ConvertRotMatrixWithoutMToRotMatrixWithM_ThrowsArgumentException()
        {
            var testMatrix1x1 = Matrix.Create<double>(1, 1);
            var testMatrix3x4 = Matrix.Create<double>(3, 4);
            Action test1 = () => Helper.ConvertMatrix.Convert_RotMatrixWithoutM_To_RotMatrixWithM(testMatrix1x1, 0);
            Action test2 = () => Helper.ConvertMatrix.Convert_RotMatrixWithoutM_To_RotMatrixWithM(testMatrix3x4, 0);

            Assert.Throws<ArgumentException>(test1);
            Assert.Throws<ArgumentException>(test2);
        }

        [Fact]
        private void ConvertMatrix_ConvertRotMatrixWithMToRotMatrixWithoutM_ThrowsArgumentException()
        {
            var testMatrix1x1 = Matrix.Create<double>(1, 1);
            var testMatrix3x4 = Matrix.Create<double>(3, 4);
            Action test1 = () => Helper.ConvertMatrix.Convert_RotMatrixWithM_To_RotMatrixWithoutM(testMatrix1x1);
            Action test2 = () => Helper.ConvertMatrix.Convert_RotMatrixWithM_To_RotMatrixWithoutM(testMatrix3x4);

            Assert.Throws<ArgumentException>(test1);
            Assert.Throws<ArgumentException>(test2);
        }

        [Fact]
        private void ConvertMatrix_PassMisNaN_ThrowsArgumentException()
        {
            var testMatrix3x3 = Matrix.Create<double>(3, 3);

            Action test = () =>
                Helper.ConvertMatrix.Convert_RotMatrixWithoutM_To_RotMatrixWithM(testMatrix3x3, double.NaN);

            Assert.Throws<ArgumentException>(test);
        }

        [Fact]
        private void ConvertMatrix_PassMisInfinity_ThrowsArgumentException()
        {
            var testMatrix3x3 = Matrix.Create<double>(3, 3);

            Action test1 = () =>
                Helper.ConvertMatrix.Convert_RotMatrixWithoutM_To_RotMatrixWithM(testMatrix3x3,
                    double.NegativeInfinity);
            Action test2 = () =>
                Helper.ConvertMatrix.Convert_RotMatrixWithoutM_To_RotMatrixWithM(testMatrix3x3,
                    double.PositiveInfinity);

            Assert.Throws<ArgumentException>(test1);
            Assert.Throws<ArgumentException>(test2);
        }

        [Fact]
        private void ConvertMatrix_ConvertRotMatrixWithoutMToRotMatrixWithM_ValidRotMatrixWithM()
        {
            var listSrc = new List<Point>();
            var listDest = new List<Point>();

            var rotationMatrixExpected = ReadControlDataFromFile(Path + "\\rotMatrixWithM.txt", 3, 3);
            var srcMatrix = ReadControlDataFromFile(Path + "\\testpoints_src.txt", 10, 3);
            var dstMatrix = ReadControlDataFromFile(Path + "\\testpoints_dest.txt", 10, 3);
            for (int row = 0; row < srcMatrix.RowCount; row++)
            {
                listSrc.Add(new Point(srcMatrix[row, 0], srcMatrix[row, 1], srcMatrix[row, 2]));
                listDest.Add(new Point(dstMatrix[row, 0], dstMatrix[row, 1], dstMatrix[row, 2]));
            }

            var nip = new NewtonIterationProcess(listSrc, listDest);
            var rotMatrixWithMActual =
                Helper.ConvertMatrix.Convert_RotMatrixWithoutM_To_RotMatrixWithM(nip.RotationMatrix, nip.M);

            for (int row = 0; row < rotMatrixWithMActual.RowCount; row++)
            for (int col = 0; col < rotMatrixWithMActual.ColumnCount; col++)
                Assert.Equal(rotationMatrixExpected[row, col], rotMatrixWithMActual[row, col], 8);
        }

        [Fact]
        private void ConvertMatrix_Convert_RotMatrixWithMToRotMatrixWithoutM_ValidRotMatrixWithoutM()
        {
            var listSrc = new List<Point>();
            var listDest = new List<Point>();

            var rotationMatrixExpected = ReadControlDataFromFile(Path + "\\rotMatrixWithoutM.txt", 3, 3);
            var srcMatrix = ReadControlDataFromFile(Path + "\\testpoints_src.txt", 10, 3);
            var dstMatrix = ReadControlDataFromFile(Path + "\\testpoints_dest.txt", 10, 3);
            for (int row = 0; row < srcMatrix.RowCount; row++)
            {
                listSrc.Add(new Point(srcMatrix[row, 0], srcMatrix[row, 1], srcMatrix[row, 2]));
                listDest.Add(new Point(dstMatrix[row, 0], dstMatrix[row, 1], dstMatrix[row, 2]));
            }

            var nip = new NewtonIterationProcess(listSrc, listDest);
            var rotMatrixWithM =
                Helper.ConvertMatrix.Convert_RotMatrixWithoutM_To_RotMatrixWithM(nip.RotationMatrix, nip.M);
            var rotMatrixWithoutMActual =
                Helper.ConvertMatrix.Convert_RotMatrixWithM_To_RotMatrixWithoutM(rotMatrixWithM);


            for (int row = 0; row < rotMatrixWithoutMActual.RowCount; row++)
            for (int col = 0; col < rotMatrixWithoutMActual.ColumnCount; col++)
                Assert.Equal(rotationMatrixExpected[row, col], rotMatrixWithoutMActual[row, col], 8);
        }

        private DenseMatrix<double> ReadControlDataFromFile(string path, int row, int col)
        {
            var matrixFromData = Matrix.Create<double>(row, col);
            var data = File.ReadAllLines(path);
            for (int textRow = 0; textRow < data.Length; textRow++)
            {
                var split = data[textRow].Split(' ');
                for (int textCol = 0; textCol < split.Length; textCol++)
                    matrixFromData[textRow, textCol] = Convert.ToDouble(split[textCol]);
            }

            return matrixFromData;
        }
    }
}