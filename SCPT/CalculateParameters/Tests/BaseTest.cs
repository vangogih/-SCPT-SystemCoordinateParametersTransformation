using System;
using System.Collections.Generic;
using System.IO;
using Extreme.Mathematics;
using Extreme.Mathematics.LinearAlgebra;
using SCPT.Helper;


internal partial class Incapsulation
{
    public abstract class BaseTest
    {
        protected static string PathToTest =
            "D:\\RiderProjects\\-SCPT-SystemCoordinateParametersTransformation\\SCPT\\CalculateParameters\\Tests";

        protected DenseMatrix<double> ReadControlDataFromFile(string path, int row, int col)
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

        protected void FillListsCoordinationData(string path, ref List<Point> listSrc, ref List<Point> listDest)
        {
            var srcMatrix = ReadControlDataFromFile(path + "\\testpoints_src.txt", 10, 3);
            var dstMatrix = ReadControlDataFromFile(path + "\\testpoints_dest.txt", 10, 3);
            for (int row = 0; row < srcMatrix.RowCount; row++)
            {
                listSrc.Add(new Point(srcMatrix[row, 0], srcMatrix[row, 1], srcMatrix[row, 2]));
                listDest.Add(new Point(dstMatrix[row, 0], dstMatrix[row, 1], dstMatrix[row, 2]));
            }
        }
    }
}