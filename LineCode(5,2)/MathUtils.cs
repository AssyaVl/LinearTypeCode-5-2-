namespace LineCodeLibrary
{
    /// <summary>
    /// Статический класс с утилитами для операций в поле GF(2).
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// Сложение в GF(2)
        /// </summary>
        /// <param name="a">Первое число (0 или 1).</param>
        /// <param name="b">Второе число (0 или 1).</param>
        /// <returns>Результат сложения: 0+0=0, 1+0=1, 0+1=1, 1+1=0.</returns>
        public static int AddGF2(int a, int b) => a ^ b;

        /// <summary>
        /// Умножение в GF(2)
        /// </summary>
        /// <param name="a">Первое число (0 или 1).</param>
        /// <param name="b">Второе число (0 или 1).</param>
        /// <returns>Результат умножения: 0*0=0, 0*1=0, 1*0=0, 1*1=1.</returns>
        public static int MultiplyGF2(int a, int b) => a & b;

        /// <summary>
        /// Умножение вектора на матрицу в GF(2) (для кодирования: c = u * G).
        /// </summary>
        /// <param name="vector">Входной вектор (строка).</param>
        /// <param name="matrix">Матрица.</param>
        /// <param name="rows">Число строк матрицы.</param>
        /// <param name="cols">Число столбцов матрицы.</param>
        /// <returns>Результат-вектор.</returns>
        public static int[] MatrixMultiply(int[] vector, int[,] matrix, int rows, int cols)
        {
            int[] result = new int[cols];
            for (int j = 0; j < cols; j++)
            {
                int sum = 0;
                for (int i = 0; i < rows; i++)
                {
                    // Суммируем произведения vector[i] * matrix[i,j]
                    sum = AddGF2(sum, MultiplyGF2(vector[i], matrix[i, j]));
                }
                result[j] = sum;
            }
            return result;
        }
        /// <summary>
        /// Умножение матрицы на вектор в GF(2) (для синдрома: s = H * r^T).
        /// </summary>
        /// <param name="matrix">Матрица.</param>
        /// <param name="vector">Входной вектор (столбец).</param>
        /// <param name="rows">Число строк матрицы.</param>
        /// <param name="cols">Число столбцов матрицы.</param>
        /// <returns>Результат-вектор (синдром).</returns>
        public static int[] VectorMultiply(int[,] matrix, int[] vector, int rows, int cols)
        {
            int[] result = new int[rows];
            for (int i = 0; i < rows; i++)
            {
                int sum = 0;
                for (int j = 0; j < cols; j++)
                {
                    // Суммируем произведения matrix[i,j] * vector[j]
                    sum = AddGF2(sum, MultiplyGF2(matrix[i, j], vector[j]));
                }
                result[i] = sum;
            }
            return result;
        }
    }
}
