using System;
using System.Collections.Generic;
using SCPT.Helper;
using SCPT.Transformation;
using Xunit;


public class TestsNewtonIterationProcess : BaseTest
{
    private string PathToTestTxt = PathToTest + "\\NewtonIterationProcess";

    [Fact]
    private void NewtonIterationProcess_InitializeCtorNullParameters_ThrowsNullReferenceException()
    {
        var list = new List<Point>();

        Action test1 = () => new NewtonIterationProcess(null, list);
        Action test2 = () => new NewtonIterationProcess(list, null);

        Assert.Throws<NullReferenceException>(test1);
        Assert.Throws<NullReferenceException>(test2);
    }

    [Fact]
    private void NewtonIterationProcess_InitCtorInvalidListCount_ThrowsArgumentException()
    {
        var sourceCoordinates = new List<Point>();
        var destinationCoordinates = new List<Point>();

        Action test1 = () => new NewtonIterationProcess(sourceCoordinates, destinationCoordinates); // empty

        sourceCoordinates.Add(new Point(0, 0, 0));
        destinationCoordinates.Add(new Point(1, 1, 1));
        Action test2 = () => new NewtonIterationProcess(sourceCoordinates, destinationCoordinates); // with 1

        sourceCoordinates.Add(new Point(0, 0, 0));
        destinationCoordinates.Add(new Point(1, 1, 1));
        Action test3 = () => new NewtonIterationProcess(sourceCoordinates, destinationCoordinates); // with 2

        sourceCoordinates.Add(new Point(0, 0, 0));
        destinationCoordinates.Add(new Point(1, 1, 1));
        Action test4 = () => new NewtonIterationProcess(sourceCoordinates, destinationCoordinates); // with 3

        destinationCoordinates.RemoveAt(2);
        Action test5 = () =>
            new NewtonIterationProcess(sourceCoordinates, destinationCoordinates); // count src != dest

        Assert.Throws<ArgumentException>(test1);
        Assert.Throws<ArgumentException>(test2);
        Assert.Throws<ArgumentException>(test3);
        Assert.Throws<ArgumentException>(test4);
        Assert.Throws<ArgumentException>(test5);
    }

    [Fact]
    private void
        NewtonIterationProcess_CheckListGetSourceAndDestinationSystemCoordinate_ValidSourceAndDestinationSystemCoordinate()
    {
        var listSrcExpected = new List<Point>();
        var listDstExpected = new List<Point>();
        FillListsCoordinationData(PathToTestTxt, ref listSrcExpected, ref listDstExpected);

        var a = new NewtonIterationProcess(listSrcExpected, listDstExpected);
        var listSrcActual = a.SourceSystemCoordinates;
        var listDstActual = a.DestinationSystemCoordinates;

        Assert.Equal(listSrcExpected.Count, listSrcActual.Count);
        Assert.Equal(listDstExpected.Count, listDstActual.Count);

        for (int i = 0; i < a.SourceSystemCoordinates.Count; i++)
        {
            Assert.Equal(listSrcExpected[i].X, listSrcActual[i].X);
            Assert.Equal(listSrcExpected[i].Y, listSrcActual[i].Y);
            Assert.Equal(listSrcExpected[i].Z, listSrcActual[i].Z);

            Assert.Equal(listDstExpected[i].X, listDstActual[i].X);
            Assert.Equal(listDstExpected[i].Y, listDstActual[i].Y);
            Assert.Equal(listDstExpected[i].Z, listDstActual[i].Z);
        }
    }

    [Fact]
    private void NewtonIterationProcess_FormingCoordinateMatrix_ValidMatrixCoordinate()
    {
        var listSrc = new List<Point>();
        var listDest = new List<Point>();
        FillListsCoordinationData(PathToTestTxt, ref listSrc, ref listDest);

        var nip = new NewtonIterationProcess(listSrc, listDest);
        var controlSrcMatrix = nip.FormingCoordinateMatrixTst(listSrc);
        var controlDstMatrix = nip.FormingCoordinateMatrixTst(listDest);

        for (int i = 0; i < listSrc.Count; i++)
        {
            Assert.Equal(listSrc[i].X, controlSrcMatrix[i, 0]);
            Assert.Equal(listSrc[i].Y, controlSrcMatrix[i, 1]);
            Assert.Equal(listSrc[i].Z, controlSrcMatrix[i, 2]);

            Assert.Equal(listDest[i].X, controlDstMatrix[i, 0]);
            Assert.Equal(listDest[i].Y, controlDstMatrix[i, 1]);
            Assert.Equal(listDest[i].Z, controlDstMatrix[i, 2]);
        }
    }

    [Fact]
    private void NewtonIterationProcess_FormingAMatrix_ValidAMatrix()
    {
        var listSrc = new List<Point>();
        var listDest = new List<Point>();
        FillListsCoordinationData(PathToTestTxt, ref listSrc, ref listDest);

        var test = ReadControlDataFromFile(PathToTestTxt + "\\aMatrix.txt", 30, 7);

        var nip = new NewtonIterationProcess(listSrc, listDest);
        var controlMatrixSrc = nip.FormingAMatrixTst();

        for (int row = 0; row < test.RowCount; row++)
        {
            for (int col = 0; col < test.ColumnCount; col++)
            {
                // excessive accuracy for correct pMatrix forming
                Assert.Equal(test[row, col], controlMatrixSrc[row, col], 8);
            }
        }
    }

    [Fact]
    private void NewtonIterationProcess_FormingYMatrix_ValidYMatrix()
    {
        var listSrc = new List<Point>();
        var listDest = new List<Point>();
        FillListsCoordinationData(PathToTestTxt, ref listSrc, ref listDest);

        var yMatrixExpected = ReadControlDataFromFile(PathToTestTxt + "\\yMatrix.txt", 30, 1);

        var nip = new NewtonIterationProcess(listSrc, listDest);
        var yMatrixActual = nip.FormingYMatrixTst();

        for (int row = 0; row < yMatrixExpected.RowCount; row++)
        {
            for (int col = 0; col < yMatrixExpected.ColumnCount; col++)
            {
                // excessive accuracy for correct pMatrix forming
                Assert.Equal(yMatrixExpected[row, col], yMatrixActual[row, col], 8);
            }
        }
    }

    [Fact]
    private void NewtonIterationProcess_GetVectorWithTransformParameters_ValidPMatrix()
    {
        var listSrc = new List<Point>();
        var listDest = new List<Point>();
        FillListsCoordinationData(PathToTestTxt, ref listSrc, ref listDest);

        var aMatrix = ReadControlDataFromFile(PathToTestTxt + "\\aMatrix.txt", 30, 7);
        var yMatrix = ReadControlDataFromFile(PathToTestTxt + "\\yMatrix.txt", 30, 1);
        var pMatrixExpected = ReadControlDataFromFile(PathToTestTxt + "\\pMatrix.txt", 7, 1);


        var nip = new NewtonIterationProcess(listSrc, listDest);
        var pMatrixActual = nip.GetVectorWithTransformParametersTst(aMatrix, yMatrix);

        for (int row = 0; row < pMatrixExpected.RowCount; row++)
        {
            for (int col = 0; col < pMatrixExpected.ColumnCount; col++)
            {
                // 10^-8 radians or 10^-3 seconds 
                Assert.Equal(pMatrixExpected[row, col], pMatrixActual[row, col], 8);
            }
        }
    }

    [Fact]
    private void NewtonIterationProcess_GetVMatrix_ValidVMatrix()
    {
        var listSrc = new List<Point>();
        var listDest = new List<Point>();
        FillListsCoordinationData(PathToTestTxt, ref listSrc, ref listDest);

        var vMatrixExpected = ReadControlDataFromFile(PathToTestTxt + "\\vMatrix.txt", 30, 1);

        var nip = new NewtonIterationProcess(listSrc, listDest);
        var aMatrix = nip.FormingAMatrixTst();
        var yMatrix = nip.FormingYMatrixTst();
        var vecParamsMatrix = nip.GetVectorWithTransformParametersTst(aMatrix, yMatrix);
        var vMatrixActual = nip.GetVMatrixTst(aMatrix, yMatrix, vecParamsMatrix);

        for (int row = 0; row < vMatrixExpected.RowCount; row++)
        for (int col = 0; col < vMatrixExpected.ColumnCount; col++)
            Assert.Equal(vMatrixExpected[row, col], vMatrixActual[row, col], 6); // 10^-6 meters
    }

    [Fact]
    private void NewtonIterationProcess_CalculateFCoefficient_ValidFCoefficient()
    {
        var listSrc = new List<Point>();
        var listDest = new List<Point>();
        FillListsCoordinationData(PathToTestTxt, ref listSrc, ref listDest);

        var nip = new NewtonIterationProcess(listSrc, listDest);
        var aMatrix = nip.FormingAMatrixTst();
        var yMatrix = nip.FormingYMatrixTst();
        var paramsTransformMatrix = nip.GetVectorWithTransformParametersTst(aMatrix, yMatrix);
        var vMatrix = nip.GetVMatrixTst(aMatrix, yMatrix, paramsTransformMatrix);

        var fCoefficientActual = nip.CalculateFCoefficientTst(vMatrix);
        var fCoefficientExpected = 3.68548085E-07;

        Assert.Equal(fCoefficientExpected, fCoefficientActual, 12); // 10^-12 m^2
    }

    [Fact]
    private void NewtonIterationProcess_CalculateMCoefficient_ValidMCoefficient()
    {
        var listSrc = new List<Point>();
        var listDest = new List<Point>();
        FillListsCoordinationData(PathToTestTxt, ref listSrc, ref listDest);

        var nip = new NewtonIterationProcess(listSrc, listDest);
        var aMatrix = nip.FormingAMatrixTst();
        var yMatrix = nip.FormingYMatrixTst();
        var paramsTransformMatrix = nip.GetVectorWithTransformParametersTst(aMatrix, yMatrix);
        var vMatrix = nip.GetVMatrixTst(aMatrix, yMatrix, paramsTransformMatrix);
        var fCoefficient = nip.CalculateFCoefficientTst(vMatrix);

        var mCoefficientActual = nip.CalculateMCoefficientTst(fCoefficient);
        var mCoefficientExpected = 0.000126585;
        Assert.Equal(mCoefficientExpected, mCoefficientActual, 6); // 10^-6 m
    }

    [Fact]
    private void NewtonIterationProcess_GetQMatrix_ValidQMatrix()
    {
        var listSrc = new List<Point>();
        var listDest = new List<Point>();
        FillListsCoordinationData(PathToTestTxt, ref listSrc, ref listDest);

        var qMatrixActual = ReadControlDataFromFile(PathToTestTxt + "\\qMatrix.txt", 7, 7);

        var nip = new NewtonIterationProcess(listSrc, listDest);
        var aMatrix = nip.FormingAMatrixTst();
        var qMatrixExpected = nip.GetQMatrixTst(aMatrix);

        for (int row = 0; row < qMatrixExpected.RowCount; row++)
        {
            for (int col = 0; col < qMatrixExpected.ColumnCount; col++)
            {
                Assert.Equal(qMatrixExpected[row, col], qMatrixActual[row, col], 5);
            }
        }
    }

    [Fact]
    private void NewtonIterationProcess_GetMeanSquareErrorsMatrix_ValidMeanSquareErrorsMatrix()
    {
        var listSrc = new List<Point>();
        var listDest = new List<Point>();
        FillListsCoordinationData(PathToTestTxt, ref listSrc, ref listDest);

        var mceMatrixActual = ReadControlDataFromFile(PathToTestTxt + "\\mceMatrix.txt", 7, 1);

        var nip = new NewtonIterationProcess(listSrc, listDest);
        var aMatrix = nip.FormingAMatrixTst();
        var yMatrix = nip.FormingYMatrixTst();
        var paramsTransformMatrix = nip.GetVectorWithTransformParametersTst(aMatrix, yMatrix);
        var vMatrix = nip.GetVMatrixTst(aMatrix, yMatrix, paramsTransformMatrix);
        var fCoefficient = nip.CalculateFCoefficientTst(vMatrix);
        var mCoefficient = nip.CalculateMCoefficientTst(fCoefficient);
        var qMatrix = nip.GetQMatrixTst(aMatrix);

        var mceMatrixExpected = nip.GetMeanSquareErrorsMatrixTst(qMatrix, mCoefficient);

        for (int row = 0; row < mceMatrixExpected.Count; row++)
            Assert.Equal(mceMatrixExpected[row], mceMatrixActual[row, 0], 8);
    }

    [Fact]
    private void NewtonIterationProcess_CheckSquareErrorsMatrixAndTransformParametersMatrix_Valid()
    {
        var listSrc = new List<Point>();
        var listDest = new List<Point>();
        FillListsCoordinationData(PathToTestTxt, ref listSrc, ref listDest);

        var mceMatrixActual = ReadControlDataFromFile(PathToTestTxt + "\\mceMatrix.txt", 7, 1);
        var pMatrixActual = ReadControlDataFromFile(PathToTestTxt + "\\rotationMatrix.txt", 3, 3);

        var nip = new NewtonIterationProcess(listSrc, listDest);

        for (int row = 0; row < nip.RotationMatrix.RowCount; row++)
        for (int col = 0; col < nip.RotationMatrix.ColumnCount; col++)
            Assert.Equal(nip.RotationMatrix[row, col], pMatrixActual[row, col], 8);

        for (int row = 0; row < nip.MeanSquareErrorsMatrix.Count; row++)
            Assert.Equal(nip.MeanSquareErrorsMatrix[row], mceMatrixActual[row, 0], 8);
    }
}