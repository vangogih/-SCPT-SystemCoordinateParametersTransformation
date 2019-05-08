using System;
using System.Collections.Generic;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using SCPT.Helper;

public abstract class BaseTest
{
    protected static string PathToTest =
        "D:\\RiderProjects\\-SCPT-SystemCoordinateParametersTransformation\\SCPT\\CalculateParameters\\Tests";

    protected Matrix<double> ReadControlDataFromFile(string path, int row, int col)
    {
        var matrixFromData = Matrix<double>.Build.Dense(row, col);
        var data = File.ReadAllLines(path);
        for (int textRow = 0; textRow < data.Length; textRow++)
        {
            var split = data[textRow].Split(' ');
            for (int textCol = 0; textCol < split.Length; textCol++)
                matrixFromData[textRow, textCol] = Convert.ToDouble(split[textCol]);
        }

        return matrixFromData;
    }

    protected void FillListsCoordinationData(string path, out SystemCoordinate listSrc, out SystemCoordinate listDest)
    {
        var srcMatrix = ReadControlDataFromFile(path + "\\testpoints_src.txt", 10, 3);
        var dstMatrix = ReadControlDataFromFile(path + "\\testpoints_dest.txt", 10, 3);
        var listSrc1 = new List<Point>();
        var listDst1 = new List<Point>();
        for (int row = 0; row < srcMatrix.RowCount; row++)
        {
            listSrc1.Add(new Point(srcMatrix[row, 0], srcMatrix[row, 1], srcMatrix[row, 2]));
            listDst1.Add(new Point(dstMatrix[row, 0], dstMatrix[row, 1], dstMatrix[row, 2]));
        }
        listSrc = new SystemCoordinate(listSrc1);
        listDest = new SystemCoordinate(listDst1);
    }
}