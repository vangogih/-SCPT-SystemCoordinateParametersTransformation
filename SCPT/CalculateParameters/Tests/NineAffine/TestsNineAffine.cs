using System;
using MathNet.Numerics.LinearAlgebra;
using SCPT.Transformation;
using Xunit;
using Xunit.Abstractions;


public class TestsNineAffine : BaseTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private string PathToTxt = PathToTest + "\\NineAffine";

    public TestsNineAffine(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    private void NineAffine_FormingEMatrix_ValidEMatrix()
    {
        FillListsCoordinationData(PathToTxt, out var srcSC, out var dstSC);
        var eMatrixExpected = ReadControlDataFromFile(PathToTxt + "\\eMatrix.txt", 3, 3);
        var a9Instance = new NineAffine(srcSC, dstSC);
        Matrix<double> eMatrixActual = a9Instance.CreateEMatrixTst();

        for (int row = 0; row < eMatrixActual.RowCount; row++)
        for (int col = 0; col < eMatrixExpected.ColumnCount; col++)
            Assert.Equal(eMatrixExpected[row, col], eMatrixActual[row, col], 8);
    }

    [Fact]
    private void NineAffine_FormingFMatrix_ValidFMatrix()
    {
        FillListsCoordinationData(PathToTxt, out var srcSC, out var dstSC);
        var fMatrixExpected = ReadControlDataFromFile(PathToTxt + "\\fMatrix.txt", 3, 3);
        var a9Instance = new NineAffine(srcSC, dstSC);
        Matrix<double> fMatrixActual = a9Instance.CreateFMatrixTst();

        for (int row = 0; row < fMatrixActual.RowCount; row++)
        for (int col = 0; col < fMatrixExpected.ColumnCount; col++)
            Assert.Equal(fMatrixExpected[row, col], fMatrixActual[row, col], 8);
    }

    [Fact]
    private void NineAffine_FormingGMatrix_ValidGMatrix()
    {
        FillListsCoordinationData(PathToTxt, out var srcSC, out var dstSC);
        var gMatrixExpected = ReadControlDataFromFile(PathToTxt + "\\gMatrix.txt", 3, 3);
        var a9Instance = new NineAffine(srcSC, dstSC);
        Matrix<double> gMatrixActual = a9Instance.CreateGMatrixTst();

        for (int row = 0; row < gMatrixActual.RowCount; row++)
        for (int col = 0; col < gMatrixExpected.ColumnCount; col++)
            Assert.Equal(gMatrixExpected[row, col], gMatrixActual[row, col], 8);
    }

    [Fact]
    private void NineAffine_CalculateRotationMatrix_ValidRotationMatrix()
    {
        FillListsCoordinationData(PathToTxt, out var srcSC, out var dstSC);
        var rotMatrixExpected = ReadControlDataFromFile(PathToTxt + "\\baseRotMatrix.txt", 3, 3);
        var a9Instance = new NineAffine(srcSC, dstSC);

        var eMatrix = a9Instance.CreateEMatrixTst();
        var fMatrix = a9Instance.CreateFMatrixTst();
        var gMatrix = a9Instance.CreateGMatrixTst();
        Matrix<double> rotMatrixActual = a9Instance.CalculateRotationMatrixTst(eMatrix, fMatrix, gMatrix);

        for (int row = 0; row < rotMatrixActual.RowCount; row++)
        for (int col = 0; col < rotMatrixExpected.ColumnCount; col++)
            Assert.Equal(rotMatrixExpected[row, col], rotMatrixActual[row, col], 5);
    }

    [Fact]
    private void NineAffine_FormingMMatrix_ValidMMatrix()
    {
        FillListsCoordinationData(PathToTxt, out var srcSC, out var dstSC);
        var mMatrixExpected = ReadControlDataFromFile(PathToTxt + "\\mMatrix.txt", 3, 3);
        var a9Instance = new NineAffine(srcSC, dstSC);
        Matrix<double> mMatrixActual = a9Instance.CreateMMatrixTst();

        for (int row = 0; row < mMatrixActual.RowCount; row++)
        for (int col = 0; col < mMatrixExpected.ColumnCount; col++)
            Assert.Equal(mMatrixExpected[row, col], mMatrixActual[row, col], 8);
    }

    [Fact]
    private void NineAffine_FormingNMatrix_ValidNMatrix()
    {
        FillListsCoordinationData(PathToTxt, out var srcSC, out var dstSC);
        var nMatrixExpected = ReadControlDataFromFile(PathToTxt + "\\nMatrix.txt", 3, 3);
        var a9Instance = new NineAffine(srcSC, dstSC);
        Matrix<double> nMatrixActual = a9Instance.CreateNMatrixTst();

        for (int row = 0; row < nMatrixActual.RowCount; row++)
        for (int col = 0; col < nMatrixExpected.ColumnCount; col++)
            Assert.Equal(nMatrixExpected[row, col], nMatrixActual[row, col], 6);
    }

    [Fact]
    private void NineAffine_FormingPMatrix_ValidPMatrix()
    {
        FillListsCoordinationData(PathToTxt, out var srcSC, out var dstSC);
        var pMatrixExpected = ReadControlDataFromFile(PathToTxt + "\\pMatrix.txt", 3, 3);
        var a9Instance = new NineAffine(srcSC, dstSC);
        Matrix<double> pMatrixActual = a9Instance.CreatePMatrixTst();

        for (int row = 0; row < pMatrixActual.RowCount; row++)
        for (int col = 0; col < pMatrixExpected.ColumnCount; col++)
            Assert.Equal(pMatrixExpected[row, col], pMatrixActual[row, col], 6);
    }

    [Fact]
    private void NineAffine_CalculateScaleMatrix_ValidScaleMatrix()
    {
        FillListsCoordinationData(PathToTxt, out var srcSC, out var dstSC);
        var scaleMatrixExpected = ReadControlDataFromFile(PathToTxt + "\\baseScaleMatrix.txt", 3, 3);
        var a9Instance = new NineAffine(srcSC, dstSC);
        var mMatrixTst = a9Instance.CreateMMatrixTst();
        var nMatrixTst = a9Instance.CreateNMatrixTst();
        var pMatrixTst = a9Instance.CreatePMatrixTst();
        Matrix<double> rotMatrixActual = a9Instance.CalculateScaleMatrixTst(mMatrixTst, nMatrixTst, pMatrixTst);

        for (int row = 0; row < rotMatrixActual.RowCount; row++)
        for (int col = 0; col < scaleMatrixExpected.ColumnCount; col++)
            Assert.Equal(scaleMatrixExpected[row, col], rotMatrixActual[row, col], 4);
    }

    [Fact]
    private void NineAffine_CreateAMatrix_ValidAMatrix()
    {
        FillListsCoordinationData(PathToTxt, out var srcSC, out var dstSC);
        var aMatrixExpected = ReadControlDataFromFile(PathToTxt + "\\aMatrix.txt", 30, 9);
        var a9Instance = new NineAffine(srcSC, dstSC);

        var eMatrix = a9Instance.CreateEMatrixTst();
        var fMatrix = a9Instance.CreateFMatrixTst();
        var gMatrix = a9Instance.CreateGMatrixTst();
        var rotMatrix = a9Instance.CalculateRotationMatrixTst(eMatrix, fMatrix, gMatrix);

        var mMatrixTst = a9Instance.CreateMMatrixTst();
        var nMatrixTst = a9Instance.CreateNMatrixTst();
        var pMatrixTst = a9Instance.CreatePMatrixTst();
        var scaleMatrix = a9Instance.CalculateScaleMatrixTst(mMatrixTst, nMatrixTst, pMatrixTst);

        var aMatrixActual = a9Instance.CreateAMatrixTst(rotMatrix, scaleMatrix);

        for (int row = 0; row < aMatrixActual.RowCount; row++)
        for (int col = 0; col < aMatrixExpected.ColumnCount; col++)
            Assert.Equal(aMatrixExpected[row, col], aMatrixActual[row, col], 3);
    }

    [Fact]
    private void NineAffine_CreateLMatrix_ValidLMatrix()
    {
        FillListsCoordinationData(PathToTxt, out var srcSC, out var dstSC);
        var lVectorExpected = ReadControlDataFromFile(PathToTxt + "\\lVector.txt", 30, 1).Column(0);
        var a9Instance = new NineAffine(srcSC, dstSC);

        var rotMatrix = Matrix<double>.Build.DenseDiagonal(3, 3, 1);
        var scaleMatrix = Matrix<double>.Build.DenseDiagonal(3, 3, 1);

        var lVectorActual = a9Instance.CreateLVectorTst(rotMatrix, scaleMatrix);

        for (int row = 0; row < lVectorActual.Count; row++)
            Assert.Equal(lVectorExpected[row], lVectorActual[row], 8);
    }

    [Fact]
    private void NineAffine_CalculateDxVector_ValidDxVector()
    {
        FillListsCoordinationData(PathToTxt, out var srcSC, out var dstSC);
        var dxVectorExpected = ReadControlDataFromFile(PathToTxt + "\\dxVector.txt", 9, 1).Column(0);
        var a9Instance = new NineAffine(srcSC, dstSC);

        var eMatrix = a9Instance.CreateEMatrixTst();
        var fMatrix = a9Instance.CreateFMatrixTst();
        var gMatrix = a9Instance.CreateGMatrixTst();
        var rotMatrix = a9Instance.CalculateRotationMatrixTst(eMatrix, fMatrix, gMatrix);

        var mMatrixTst = a9Instance.CreateMMatrixTst();
        var nMatrixTst = a9Instance.CreateNMatrixTst();
        var pMatrixTst = a9Instance.CreatePMatrixTst();
        var scaleMatrix = a9Instance.CalculateScaleMatrixTst(mMatrixTst, nMatrixTst, pMatrixTst);

        var aMatrix = a9Instance.CreateAMatrixTst(rotMatrix, scaleMatrix);
        
        var diagonalRot = Matrix<double>.Build.DenseDiagonal(3, 3, 1);
        var diagonalScale = Matrix<double>.Build.DenseDiagonal(3, 3, 1);
        var lVector = a9Instance.CreateLVectorTst(diagonalRot, diagonalScale);

        var dxVectorActual = a9Instance.CreateDxVectorTst(aMatrix, lVector);

        for (int row = 0; row < dxVectorActual.Count; row++)
            Assert.Equal(dxVectorExpected[row], dxVectorActual[row], 8);
    }

    [Fact]
    private void NineAffine_Iteration_ValidIteration()
    {
        FillListsCoordinationData(PathToTxt, out var srcSC, out var dstSC);
        var a9Instance = new NineAffine(srcSC, dstSC);

        _testOutputHelper.WriteLine("DC\n");
        foreach (var t in a9Instance.DeltaCoordinateMatrix.Vector)
            _testOutputHelper.WriteLine(t.ToString());
        _testOutputHelper.WriteLine("rot\n");
        for (int row = 0; row < a9Instance.RotationMatrix.Matrix.RowCount; row++)
        for (int col = 0; col < a9Instance.RotationMatrix.Matrix.ColumnCount; col++)
            _testOutputHelper.WriteLine(a9Instance.RotationMatrix.Matrix[row, col].ToString());
        _testOutputHelper.WriteLine("scale\n");
        _testOutputHelper.WriteLine(a9Instance.M.ToString());
    }
}