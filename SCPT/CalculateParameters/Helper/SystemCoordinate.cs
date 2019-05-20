using System.Collections.Generic;
using System.Security.Permissions;
using MathNet.Numerics.LinearAlgebra;

namespace SCPT.Helper
{
    /// <summary>
    /// System coordinate on different represent type
    /// </summary>
    public class SystemCoordinate
    {
        /// <summary>
        /// Coordinates represent List type.
        /// </summary>
        public List<Point> List { get; }
        
        /// <summary>
        /// Coordinates represent Matrix type.
        /// <example>
        /// <code>
        /// |X1 Y1 Z1|
        /// |X2 Y2 Z2|
        /// |.  .  . |
        /// |Xn Yn Zn|
        /// </code>
        /// </example>
        /// </summary>
        public Matrix<double> Matrix { get; private set; }
        
        /// <summary>
        /// Coordinates represent Vector type
        /// </summary>
        /// <example>
        /// <code>
        /// |X1|
        /// |Y1|
        /// |Z1|
        /// |X2|
        /// |Y2|
        /// |Z2|
        /// |..|
        /// |Xn|
        /// |Yn|
        /// |Zn|
        /// </code>
        /// </example>
        public Vector<double> Vector { get; private set; }

        /// <inheritdoc cref="SystemCoordinate"/>
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