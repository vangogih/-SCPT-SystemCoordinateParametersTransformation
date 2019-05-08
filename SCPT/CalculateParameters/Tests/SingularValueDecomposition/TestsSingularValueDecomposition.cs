using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using SCPT.Helper;
using SCPT.Transformation;
using Xunit;

public class TestsSingularValueDecomposition : BaseTest
{
    private string PathToTxt = PathToTest + "\\SingularValueDecomposition";

    [Fact]
    private void SVD_CreateEMatrix_ValidEMatrix()
    {
        FillListsCoordinationData(PathToTxt, out var srcCordList, out var dstCordList);
        var eMatrixExpected = ReadControlDataFromFile(PathToTxt + "\\eMatrix.txt", 11, 11);
        var svdInstance = new SingularValueDecomposition(srcCordList, dstCordList);
        Matrix<double> eMatrixActual = svdInstance.CreateEMatrixTst();

        for (int row = 0; row < eMatrixActual.RowCount; row++)
        for (int col = 0; col < eMatrixActual.ColumnCount; col++)
            Assert.Equal(eMatrixExpected[row, col], eMatrixActual[row, col], 8);
    }

    [Fact]
    private void SVD_CalculateTransformationParameters_ValidRotDeltaMatrixM()
    {
        FillListsCoordinationData(PathToTxt, out var srcCordList, out var dstCordList);
        var rotMatrixExpected = ReadControlDataFromFile(PathToTxt + "\\rotMatrix.txt", 3, 3);
        var deltaMatrixExpected = ReadControlDataFromFile(PathToTxt + "\\deltaMatrix.txt", 3, 1).Column(0);
//        var mExpected = -0.0000005476;
        var mExpected = -0.00000290935921;
        var svdInstance = new SingularValueDecomposition(srcCordList, dstCordList);

        var rotMatrixActual = svdInstance.RotationMatrix.Matrix;
        var deltaMatrixActual = svdInstance.DeltaCoordinateMatrix.Vector;
        var mActual = svdInstance.M;

        for (int row = 0; row < rotMatrixActual.RowCount; row++)
        for (int col = 0; col < rotMatrixActual.ColumnCount; col++)
            Assert.Equal(rotMatrixExpected[row, col], rotMatrixActual[row, col], 8);
        for (int row = 0; row < deltaMatrixActual.Count; row++)
            Assert.Equal(deltaMatrixExpected[row], deltaMatrixActual[row], 3); // low precision 10^-3
        Assert.Equal(mExpected, mActual, 8);
    }
}