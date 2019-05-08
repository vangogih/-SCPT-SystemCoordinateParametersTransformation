using System;
using System.Collections.Generic;
using SCPT.Helper;
using SCPT.Transformation;
using Xunit;


public class TestsNewtonIterationProcess : BaseTest
{
    private string PathToTestTxt = PathToTest + "\\NewtonIterationProcess";

    [Fact]
    private void
        NewtonIterationProcess_CheckListGetSourceAndDestinationSystemCoordinate_ValidSourceAndDestinationSystemCoordinate()
    {
        FillListsCoordinationData(PathToTestTxt, out var listSrcExpected, out var listDstExpected);

        var a = new NewtonIterationProcess(listSrcExpected, listDstExpected);
        var listSrcActual = a.SourceSystemCoordinates;
        var listDstActual = a.DestinationSystemCoordinates;

        Assert.Equal(listSrcExpected.List.Count, listSrcActual.List.Count);
        Assert.Equal(listDstExpected.List.Count, listDstActual.List.Count);

        for (int i = 0; i < a.SourceSystemCoordinates.List.Count; i++)
        {
            Assert.Equal(listSrcExpected.List[i].X, listSrcActual.List[i].X);
            Assert.Equal(listSrcExpected.List[i].Y, listSrcActual.List[i].Y);
            Assert.Equal(listSrcExpected.List[i].Z, listSrcActual.List[i].Z);

            Assert.Equal(listDstExpected.List[i].X, listDstActual.List[i].X);
            Assert.Equal(listDstExpected.List[i].Y, listDstActual.List[i].Y);
            Assert.Equal(listDstExpected.List[i].Z, listDstActual.List[i].Z);
        }
    }

    [Fact]
    private void NewtonIterationProcess_FormingAMatrix_ValidAMatrix()
    {
        FillListsCoordinationData(PathToTestTxt, out var listSrc, out var listDest);

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
        FillListsCoordinationData(PathToTestTxt, out var listSrc, out var listDest);

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
        FillListsCoordinationData(PathToTestTxt, out var listSrc, out var listDest);

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
        FillListsCoordinationData(PathToTestTxt, out var listSrc, out var listDest);

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
        FillListsCoordinationData(PathToTestTxt, out var listSrc, out var listDest);

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
        FillListsCoordinationData(PathToTestTxt, out var listSrc, out var listDest);

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
        FillListsCoordinationData(PathToTestTxt, out var listSrc, out var listDest);

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
        FillListsCoordinationData(PathToTestTxt, out var listSrc, out var listDest);

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
        FillListsCoordinationData(PathToTestTxt, out var listSrc, out var listDest);

        var mceMatrixActual = ReadControlDataFromFile(PathToTestTxt + "\\mceMatrix.txt", 7, 1);
        var pMatrixActual = ReadControlDataFromFile(PathToTestTxt + "\\rotationMatrix.txt", 3, 3);

        var nip = new NewtonIterationProcess(listSrc, listDest);

        for (int row = 0; row < nip.RotationMatrix.Matrix.RowCount; row++)
        for (int col = 0; col < nip.RotationMatrix.Matrix.ColumnCount; col++)
            Assert.Equal(nip.RotationMatrix.Matrix[row, col], pMatrixActual[row, col], 8);

        for (int row = 0; row < nip.MeanSquareErrorsMatrix.Count; row++)
            Assert.Equal(nip.MeanSquareErrorsMatrix[row], mceMatrixActual[row, 0], 8);
    }
}