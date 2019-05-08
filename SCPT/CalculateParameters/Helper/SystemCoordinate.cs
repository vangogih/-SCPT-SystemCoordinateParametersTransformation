using System.Collections.Generic;
using System.Security.Permissions;
using MathNet.Numerics.LinearAlgebra;

namespace SCPT.Helper
{
    public class SystemCoordinate
    {
        public List<Point> List { get; }
        public Matrix<double> Matrix { get; private set; }
        public Vector<double> Vector { get; private set; }

        public SystemCoordinate(List<Point> coordList)
        {
            List = coordList;

            FormingMatrix();
            FormingVector();
        }

        private void FormingVector()
        {
            var vec = Vector<double>.Build.Dense(List.Count * 3);
            for (int matrixRow = 0, listRow = 0; listRow < List.Count; matrixRow += 3, listRow++)
            {
                vec[matrixRow] = List[listRow].X;
                vec[matrixRow + 1] = List[listRow].Y;
                vec[matrixRow + 2] = List[listRow].Z;
            }

            Vector = vec;
        }

        private void FormingMatrix()
        {
            var mat = Matrix<double>.Build.Dense(List.Count, 3);
            for (int i = 0; i < List.Count; i++)
            {
                mat[i, 0] = List[i].X;
                mat[i, 1] = List[i].Y;
                mat[i, 2] = List[i].Z;
            }

            Matrix = mat;
        }
    }
}