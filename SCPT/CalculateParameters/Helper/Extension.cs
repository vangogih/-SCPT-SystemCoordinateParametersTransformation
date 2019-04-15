using System.Collections.Generic;
using Extreme.Mathematics;
using SCPT.Transformation;

namespace SCPT.Helper
{
    // TODO write Helmert transformation  
    public static class Extension
    {
        public static void Helmert(this List<Point> srcList, Matrix<double> transMatrix)
        {
            HelmertTransformation(srcList, transMatrix);
        }

        public static void Helmert(this List<Point> srcList, double scale, Matrix<double> rotateMatrix)
        {
            HelmertTransformation(srcList, Matrix.Multiply(rotateMatrix, 1 + scale));
        }

        private static void HelmertTransformation(List<Point> srcList, Matrix<double> transMatrix)
        {
            
        }
    }
}