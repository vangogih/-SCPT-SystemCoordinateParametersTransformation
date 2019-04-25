using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using SCPT.Helper;
using SCPT.Transformation;
using Xunit;


public class TestsLinearProcedure : BaseTest
{
    private string PathToTxt = PathToTest + "\\LinearProcedure";

    [Fact]
    private void LinearProcedure_InitializeCtorNullParameters_ThrowNullReferenceException()
    {
        var srcList = new List<Point>();
        var dstList = new List<Point>();

        Action test1 = () => new LinearProcedure(null, dstList);
        Action test2 = () => new LinearProcedure(srcList, null);

        Assert.Throws<NullReferenceException>(test1);
        Assert.Throws<NullReferenceException>(test2);
    }

    [Fact]
    private void LinearProcedure_FormingMatrixQ_ValidMatrixQ()
    {
        var srcList = new List<Point>();
        var dstList = new List<Point>();
        FillListsCoordinationData(PathToTxt, ref srcList, ref dstList);

        var qMatrixExpected = ReadControlDataFromFile(PathToTxt + "\\qMatrix.txt", 10, 4);
        var lp = new LinearProcedure(srcList, dstList);
        Matrix<double> qMatrixActual = lp.FormingQMatrixTst();

        for (int row = 0; row < qMatrixActual.RowCount; row++)
        for (int col = 0; col < qMatrixActual.ColumnCount; col++)
            Assert.Equal(qMatrixExpected[row, col], qMatrixActual[row, col], 8);
    }

    [Fact]
    private void LinearProcedure_FormingLxVector_ValidVxVector()
    {
        var srcList = new List<Point>();
        var dstList = new List<Point>();
        FillListsCoordinationData(PathToTxt, ref srcList, ref dstList);

        var lxVectorExpected = ReadControlDataFromFile(PathToTxt + "\\LxMatrix.txt", 10, 1).Column(0);
        var lp = new LinearProcedure(srcList, dstList);
        Vector<double> lxVectorActual = lp.FormingLxVectorTst();

        Assert.Equal(lxVectorActual.Count, lxVectorExpected.Count);

        for (int row = 0; row < lxVectorActual.Count; row++)
            Assert.Equal(lxVectorActual[row], lxVectorExpected[row], 8);
    }

    [Fact]
    private void LinearProcedure_FormingLyVector_ValidLyVector()
    {
        var srcList = new List<Point>();
        var dstList = new List<Point>();
        FillListsCoordinationData(PathToTxt, ref srcList, ref dstList);

        var lyVectorExpected = ReadControlDataFromFile(PathToTxt + "\\LyMatrix.txt", 10, 1).Column(0);
        var lp = new LinearProcedure(srcList, dstList);
        Vector<double> lyVectorActual = lp.FormingLyVectorTst();

        Assert.Equal(lyVectorActual.Count, lyVectorExpected.Count);

        for (int row = 0; row < lyVectorActual.Count; row++)
            Assert.Equal(lyVectorActual[row], lyVectorExpected[row], 8);
    }

    [Fact]
    private void LinearProcedure_FormingLzVector_ValidLzVector()
    {
        var srcList = new List<Point>();
        var dstList = new List<Point>();
        FillListsCoordinationData(PathToTxt, ref srcList, ref dstList);

        var lzVectorExpected = ReadControlDataFromFile(PathToTxt + "\\LzMatrix.txt", 10, 1).Column(0);
        var lp = new LinearProcedure(srcList, dstList);
        Vector<double> lzVectorActual = lp.FormingLzVectorTst();

        Assert.Equal(lzVectorActual.Count, lzVectorExpected.Count);

        for (int row = 0; row < lzVectorActual.Count; row++)
            Assert.Equal(lzVectorActual[row], lzVectorExpected[row], 8);
    }

    [Fact]
    private void LinearProcedure_CalculateDxVector_ValidDxVector()
    {
        var srcList = new List<Point>();
        var dstList = new List<Point>();
        FillListsCoordinationData(PathToTxt, ref srcList, ref dstList);

        var dxVectorExpected = ReadControlDataFromFile(PathToTxt + "\\DxVector.txt", 4, 1).Column(0);
        var lp = new LinearProcedure(srcList, dstList);
        var qMatrix = lp.FormingQMatrixTst();
        var lxVector = lp.FormingLxVectorTst();
        Vector<double> dxVectorActual = lp.CalculateDVectorsTst(qMatrix, lxVector);

        Assert.Equal(dxVectorActual.Count, dxVectorExpected.Count);

        for (int row = 0; row < dxVectorActual.Count; row++)
            Assert.Equal(dxVectorActual[row], dxVectorExpected[row], 8);
    }

    [Fact]
    private void LinearProcedure_CalculateDyVector_ValidDyVector()
    {
        var srcList = new List<Point>();
        var dstList = new List<Point>();
        FillListsCoordinationData(PathToTxt, ref srcList, ref dstList);

        var dyVectorExpected = ReadControlDataFromFile(PathToTxt + "\\DyVector.txt", 4, 1).Column(0);
        var lp = new LinearProcedure(srcList, dstList);
        var qMatrix = lp.FormingQMatrixTst();
        var lyVector = lp.FormingLyVectorTst();
        Vector<double> dyVectorActual = lp.CalculateDVectorsTst(qMatrix, lyVector);

        Assert.Equal(dyVectorActual.Count, dyVectorExpected.Count);

        for (int row = 0; row < dyVectorActual.Count; row++)
            Assert.Equal(dyVectorActual[row], dyVectorExpected[row], 8);
    }

    [Fact]
    private void LinearProcedure_CalculateDzVector_ValidDzVector()
    {
        var srcList = new List<Point>();
        var dstList = new List<Point>();
        FillListsCoordinationData(PathToTxt, ref srcList, ref dstList);

        var dzVectorExpected = ReadControlDataFromFile(PathToTxt + "\\DzVector.txt", 4, 1).Column(0);
        var lp = new LinearProcedure(srcList, dstList);
        var qMatrix = lp.FormingQMatrixTst();
        var lzVector = lp.FormingLzVectorTst();
        Vector<double> dzVectorActual = lp.CalculateDVectorsTst(qMatrix, lzVector);

        Assert.Equal(dzVectorActual.Count, dzVectorExpected.Count);

        for (int row = 0; row < dzVectorActual.Count; row++)
            Assert.Equal(dzVectorActual[row], dzVectorExpected[row], 8);
    }

    [Fact]
    private void LinearProcedure_FormingRotationMatrix_ValidRotationMatrix()
    {
        var srcList = new List<Point>();
        var dstList = new List<Point>();
        FillListsCoordinationData(PathToTxt, ref srcList, ref dstList);

        var rotMatrixActual = ReadControlDataFromFile(PathToTxt + "\\rotationMatrixMultiplyM.txt", 3, 3);
        var lp = new LinearProcedure(srcList, dstList);
        var qMatrix = lp.FormingQMatrixTst();
        var lxVector = lp.FormingLxVectorTst();
        var lyVector = lp.FormingLyVectorTst();
        var lzVector = lp.FormingLzVectorTst();
        var dxVector = lp.CalculateDVectorsTst(qMatrix, lxVector);
        var dyVector = lp.CalculateDVectorsTst(qMatrix, lyVector);
        var dzVector = lp.CalculateDVectorsTst(qMatrix, lzVector);

        Matrix<double> rotMatrixExpected = lp.FormingRotationMatrixTst(dxVector, dyVector, dzVector);

        for (int row = 0; row < rotMatrixExpected.RowCount; row++)
        for (int col = 0; col < rotMatrixExpected.ColumnCount; col++)
            Assert.Equal(rotMatrixActual[row, col], rotMatrixExpected[row, col], 8);
    }

    [Fact]
    private void LinearProcedure_CheckAllCalculations_ValidRotMatrixDeltaVectorM()
    {
        var srcList = new List<Point>();
        var dstList = new List<Point>();
        FillListsCoordinationData(PathToTxt, ref srcList, ref dstList);

        var rotMatrixActual = ReadControlDataFromFile(PathToTxt + "\\rotationMatrixWithoutM.txt", 3, 3);
        var deltaVectorActual = ReadControlDataFromFile(PathToTxt + "\\resultDeltaVector.txt", 3, 1).Column(0);
        var mActual = -0.00000290935921;

        var lp = new LinearProcedure(srcList, dstList);
        var rotMatrixExpected = lp.RotationMatrix;
        var deltaVectorExpected = lp.DeltaCoordinateMatrix;

        for (int row = 0; row < rotMatrixExpected.RowCount; row++)
        for (int col = 0; col < rotMatrixExpected.ColumnCount; col++)
            Assert.Equal(rotMatrixActual[row, col], rotMatrixExpected[row, col], 8);

        for (int row = 0; row < deltaVectorExpected.Count; row++)
            Assert.Equal(deltaVectorActual[row], deltaVectorExpected[row], 8);

        Assert.Equal(mActual, lp.M, 9);
    }
}