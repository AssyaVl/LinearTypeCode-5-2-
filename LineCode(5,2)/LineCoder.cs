using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineCodeLibrary
{
    /// <summary>
    /// Класс для работы с линейным кодом (5,2).
    /// Поддерживает кодирование, вычисление синдрома, исправление ошибок и декодирование.
    /// </summary>
    public class LineCoder
    {
        // Порождающая матрица G (2x5) для кодирования
        private readonly int[,] G = new int[,] { { 1, 0, 1, 1, 1 }, { 0, 1, 0, 1, 1 } };

        // Проверочная матрица H (3x5) для проверки и исправления ошибок
        private readonly int[,] H = new int[,] { { 1, 0, 1, 0, 0 }, { 1, 1, 0, 1, 0 }, { 1, 1, 0, 0, 1 } };

        // Таблица синдромов: сопоставляет синдромы с векторами ошибок (лидерами классов смежности)
        private readonly Dictionary<string, int[]> syndromeTable;//ключ - синдром (как строка),
                                                                 //значение - вектор ошибки

        /// <summary>
        /// Конструктор. Инициализирует таблицу синдромов для одиночных ошибок.
        /// </summary>
        public LineCoder()
        {
            syndromeTable = new Dictionary<string, int[]>();
            InitializeSyndromeTable();
        }

        /// <summary>
        /// Инициализирует таблицу синдромов.
        /// Для каждой позиции i создаётся вектор ошибки (единица в i-й позиции),
        /// вычисляется синдром s = H * e^T и сохраняется соответствие.
        /// </summary>
        private void InitializeSyndromeTable()
        {
            // Для каждой из 5 позиций создаём вектор ошибки с одной 1
            for (int i = 0; i < 5; i++)
            {
                int[] errorVector = new int[5];
                errorVector[i] = 1;
                // Вычисляем синдром для вектора ошибки
                int[] syndrome = MathUtils.VectorMultiply(H, errorVector, 3, 5);
                // Ключ — строка из синдрома, значение - вектор ошибки
                string syndromeKey = string.Join("", syndrome);
                syndromeTable[syndromeKey] = errorVector;
            }
            // Синдром 000 соответствует отсутствию ошибок
            syndromeTable["000"] = new int[5]; //пустой массив 
        }

        /// <summary>
        /// Кодирует 2-битное сообщение в 5-битное кодовое слово (c = u * G).
        /// </summary>
        /// <param name="message">Входное сообщение (2 бита).</param>
        /// <returns>Кодовое слово (5 бит).</returns>
        /// <exception cref="ArgumentException">Если вход не 2-битный или не двоичный.</exception>
        public int[] Encode(int[] message)
        {
            // Проверяем, что вход — 2 бита и только 0 или 1
            if (message.Length != 2 || message.Any(x => x != 0 && x != 1))
                throw new ArgumentException("Сообщение должно быть 2-битным и двоичным.");
            // Умножаем вектор сообщения на матрицу G
            return MathUtils.MatrixMultiply(message, G, 2, 5);//возвращает закодированное слово
        }

        /// <summary>
        /// Вычисляет синдром для полученного слова (s = H * r^T).
        /// </summary>
        /// <param name="received">Полученное слово (5 бит).</param>
        /// <returns>Синдром (3 бита).</returns>
        /// <exception cref="ArgumentException">Если вход не 5-битный или не двоичный.</exception>
        public int[] CalculateSyndrome(int[] received)
        {
            // Проверяем, что вход — 5 бит и только 0 или 1
            if (received.Length != 5 || received.Any(x => x != 0 && x != 1))
                throw new ArgumentException("Полученное слово должно быть 5-битным и двоичным.");
            // Умножаем матрицу H на вектор r
            return MathUtils.VectorMultiply(H, received, 3, 5); //возвращает синдром
        }

        /// <summary>
        /// Исправляет одиночную ошибку в полученном слове, используя синдром.
        /// </summary>
        /// <param name="received">Полученное слово (5 бит).</param>
        /// <returns>Исправленное кодовое слово.</returns>
        /// <exception cref="InvalidOperationException">Если ошибка не одиночная.</exception>
        public int[] CorrectError(int[] received)
        {
            // Вычисляем синдром
            int[] syndrome = CalculateSyndrome(received);
            string syndromeKey = string.Join("", syndrome);//записываем синдром как строку
            // Проверяем, есть ли синдром в словаре
            if (!syndromeTable.ContainsKey(syndromeKey))//если его там нет, значит ошибка не одна
                throw new InvalidOperationException("Невозможно исправить более одной ошибки.");
            // Получаем вектор ошибки
            int[] errorVector = syndromeTable[syndromeKey];
            // Исправляем слово: c = r + e 
            int[] corrected = new int[5];
            for (int i = 0; i < 5; i++)//записываем исправленное слово посимвольно
                corrected[i] = MathUtils.AddGF2(received[i], errorVector[i]);//складываем векторы
            return corrected;//возвращаем исправленное слово
        }

        /// <summary>
        /// Декодирует кодовое слово, извлекая исходное сообщение (первые два бита)
        /// </summary>
        /// <param name="codeword">Кодовое слово (5 бит).</param>
        /// <returns>Исходное сообщение (2 бита).</returns>
        /// <exception cref="ArgumentException">Если вход не 5-битный или не двоичный.</exception>
        public int[] Decode(int[] codeword)
        {
            // Проверяем, что вход — 5 бит и только 0 или 1
            if (codeword.Length != 5 || codeword.Any(x => x != 0 && x != 1))
                throw new ArgumentException("Кодовое слово должно быть 5-битным и двоичным.");
            // Возвращаем первые 2 бита
            return new int[] { codeword[0], codeword[1] };
        }

        /// <summary>
        /// Возвращает вектор ошибки для полученного слова по синдрому.
        /// </summary>
        /// <param name="received">Полученное слово (5 бит).</param>
        /// <returns>Вектор ошибки или null, если ошибка не одиночная.</returns>
        public int[] GetErrorVector(int[] received)
        {
            // Вычисляем синдром
            int[] syndrome = CalculateSyndrome(received);
            string syndromeKey = string.Join("", syndrome);//переделываем синдром в строку
            // Возвращаем вектор ошибки, если синдром есть в таблице
            return syndromeTable.ContainsKey(syndromeKey) ? syndromeTable[syndromeKey] : null;
        }
    }
}
