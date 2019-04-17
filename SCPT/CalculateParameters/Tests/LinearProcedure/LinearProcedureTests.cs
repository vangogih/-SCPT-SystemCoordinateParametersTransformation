using System;
using System.Collections.Generic;
using System.IO;
using Extreme.Mathematics;
using Extreme.Mathematics.LinearAlgebra;
using SCPT.Helper;
using Xunit;

namespace SCPT.Tests.LinearProcedure
{
    public class LinearProcedureTests : BaseTest
    {
        private string PathToTxt = PathToTest + "\\LinearProcedure";
        
        [Fact]
        private void LinearProcedure_InitializeCtorNullParameters_ThrowNullReferenceException()
        {
            var srcList = new List<Point>();
            var dstList = new List<Point>();
            
            Action test  = () => new Transformation.LinearProcedure(null,null);
            
            Assert.Throws<NullReferenceException>(test);
        }

        [Fact]
        private void LinearProcedure_FormingMatrixQ_ValidMatrixQ()
        {
            Assert.False(true);
        }
        
    }
}