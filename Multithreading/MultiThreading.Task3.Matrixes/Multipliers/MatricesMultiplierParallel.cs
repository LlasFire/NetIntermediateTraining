using MultiThreading.Task3.MatrixMultiplier.Matrices;
using System;
using System.Threading.Tasks;

namespace MultiThreading.Task3.MatrixMultiplier.Multipliers
{
    public class MatricesMultiplierParallel : IMatricesMultiplier
    {
        public IMatrix Multiply(IMatrix m1, IMatrix m2)
        {
            var resultMatrix = new Matrix(m1.RowCount, m2.ColCount);

            Parallel.For(0, (int)m1.RowCount, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (state) => MultipleFirstMatrixRows(state, m1, m2, resultMatrix));

            return resultMatrix;
        }

        private void MultipleFirstMatrixRows(int firstMatrixRowIndex,
                                             IMatrix m1,
                                             IMatrix m2,
                                             IMatrix resultMatrix)
        {
            Parallel.For(0, (int)m2.ColCount, new ParallelOptions { MaxDegreeOfParallelism = 8},(state) => MultipleSecondMartixColumn(state, firstMatrixRowIndex, m1, m2, resultMatrix));
        }

        private void MultipleSecondMartixColumn(int secondMatrixColumnIndex,
                                                int firstMatrixRowIndex,
                                                IMatrix m1,
                                                IMatrix m2,
                                                IMatrix resultMatrix)
        {
            long sum = 0;

            Parallel.For(0, (int)m1.ColCount, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (state) => CalculateResultForMatrix(state, m1, m2, firstMatrixRowIndex, secondMatrixColumnIndex, ref sum));

            resultMatrix.SetElement(firstMatrixRowIndex, secondMatrixColumnIndex, sum);
        }

        private void CalculateResultForMatrix(int firstMatrixColumnNumber,
                                              IMatrix m1,
                                              IMatrix m2,
                                              int firstMatrixRowIndex,
                                              int secondMatrixColumnIndex,
                                              ref long sum)
        {
            sum += m1.GetElement(firstMatrixRowIndex, firstMatrixColumnNumber) * m2.GetElement(firstMatrixColumnNumber, secondMatrixColumnIndex);
        }
    }
}
