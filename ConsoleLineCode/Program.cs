using LineCodeLibrary;
namespace ConsoleLineCode
{
    internal class Program
    {
        /// <summary>
        /// Точка входа в приложение.
        /// </summary>
        static void Main(string[] args)
        {
            // Устанавливаем белый фон для консоли
            Console.BackgroundColor = ConsoleColor.White;
            Console.Clear();
            // Создаём объект линейного кода
            LineCoder code = new LineCoder();

            // Основной цикл меню
            while (true)
            {
                DisplayMenu();
                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            EncodeMessage(code);
                            break;
                        case "2":
                            IntroduceErrorAndCorrect(code);
                            break;
                        case "3":
                            DecodeMessage(code);
                            break;
                        case "4":
                            AnalyzeWord(code);
                            break;
                        case "5":
                            Console.WriteLine("Выход...");
                            return;
                        default:
                            PrintResponse("Неверный выбор. Попробуйте снова.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    PrintResponse($"Ошибка: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Отображает меню.
        /// </summary>
        static void DisplayMenu()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n=== Меню линейного кода (5,2) ===");
            Console.WriteLine("1. Закодировать 2-битное сообщение");
            Console.WriteLine("2. Ввести ошибку и исправить");
            Console.WriteLine("3. Декодировать 5-битное слово");
            Console.WriteLine("4. Анализировать 5-битное слово");
            Console.WriteLine("5. Выход");
            Console.Write("Выберите опцию: ");

            Console.ForegroundColor = ConsoleColor.Black;
        }

        /// <summary>
        /// Выводит сообщение в розовом цвете.
        /// </summary>
        /// <param name="message">Сообщение для вывода.</param>
        static void PrintResponse(string message)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(message);

            Console.ForegroundColor = ConsoleColor.Black;
        }

        /// <summary>
        /// Считывает двоичный вектор заданной длины от пользователя.
        /// </summary>
        /// <param name="length">Ожидаемая длина вектора.</param>
        /// <param name="prompt">Приглашение для ввода.</param>
        /// <returns>Двоичный вектор как массив int.</returns>
        /// <exception cref="ArgumentException">Если ввод некорректен.</exception>
        static int[] ReadBinaryVector(int length, string prompt)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            // Проверяем, что длина и символы корректны
            if (input.Length != length || !input.All(c => c == '0' || c == '1'))
                throw new ArgumentException($"Ввод должен быть {length}-битной двоичной строкой.");
            // Преобразуем строку в массив чисел
            return input.Select(c => c - '0').ToArray();
        }

        /// <summary>
        /// Кодирует 2-битное сообщение и выводит результат.
        /// </summary>
        /// <param name="code">Объект линейного кода.</param>
        static void EncodeMessage(LineCoder  code)
        {
            // Считываем 2-битное сообщение
            int[] message = ReadBinaryVector(2, "Введите 2-битное сообщение: ");
            // Кодируем сообщение
            int[] codeword = code.Encode(message);
            // Выводим результат
            PrintResponse($"Закодированное слово: {string.Join("", codeword)}");
        }

        /// <summary>
        /// Выполняет полный цикл: кодирование, ввод слова с ошибкой,
        /// вычисление синдрома, исправление и декодирование.
        /// </summary>
        /// <param name="code">Объект линейного кода.</param>
        static void IntroduceErrorAndCorrect(LineCoder code)
        {
            // Считываем 2-битное сообщение
            int[] message = ReadBinaryVector(2, "Введите 2-битное сообщение: ");
            // Кодируем сообщение
            int[] codeword = code.Encode(message);
            PrintResponse($"Закодированное слово: {string.Join("", codeword)}");

            // Считываем полученное слово с одной ошибкой
            int[] received = ReadBinaryVector(5, "Введите 5-битное слово с одной ошибкой: ");
            // Вычисляем синдром
            int[] syndrome = code.CalculateSyndrome(received);
            PrintResponse($"Синдром: {string.Join("", syndrome)}");

            // Получаем вектор ошибки
            int[] errorVector = code.GetErrorVector(received);
            if (errorVector == null)
            {
                PrintResponse("Невозможно исправить: обнаружено несколько ошибок.");
                return;
            }
            PrintResponse($"Вектор ошибки: {string.Join("", errorVector)}");

            // Исправляем ошибку
            int[] corrected = code.CorrectError(received);
            PrintResponse($"Исправленное слово: {string.Join("", corrected)}");

            // Декодируем исправленное слово
            int[] decoded = code.Decode(corrected);
            PrintResponse($"Декодированное сообщение: {string.Join("", decoded)}");
        }

        /// <summary>
        /// Декодирует 5-битное слово, извлекая 2-битное сообщение.
        /// </summary>
        /// <param name="code">Объект линейного кода.</param>
        static void DecodeMessage(LineCoder code)
        {
            // Считываем 5-битное слово
            int[] codeword = ReadBinaryVector(5, "Введите 5-битное слово: ");
            // Декодируем слово
            int[] decoded = code.Decode(codeword);
            // Выводим результат
            PrintResponse($"Декодированное сообщение: {string.Join("", decoded)}");
        }

        /// <summary>
        /// Анализирует 5-битное слово: вычисляет синдром, определяет вектор ошибки
        /// и пытается исправить слово, если ошибка одиночная.
        /// </summary>
        /// <param name="code">Объект линейного кода.</param>
        static void AnalyzeWord(LineCoder code)
        {
            // Считываем 5-битное слово
            int[] received = ReadBinaryVector(5, "Введите 5-битное слово: ");
            // Вычисляем синдром
            int[] syndrome = code.CalculateSyndrome(received);
            PrintResponse($"Синдром: {string.Join("", syndrome)}");
            // Проверяем, является ли слово кодовым
            if (syndrome.All(x => x == 0))
            {
                PrintResponse("Слово является кодовым (ошибок нет или не обнаружено).");
            }
            else
            {
                // Пытаемся определить вектор ошибки
                int[] errorVector = code.GetErrorVector(received);
                if (errorVector == null)
                {
                    PrintResponse("Невозможно исправить: обнаружено несколько ошибок.");
                    return;
                }
                PrintResponse($"Вектор ошибки: {string.Join("", errorVector)}");

                // Исправляем слово
                int[] corrected = code.CorrectError(received);
                PrintResponse($"Исправленное слово: {string.Join("", corrected)}");
            }

            // Декодируем слово (даже если оно не исправлялось)
            int[] decoded = code.Decode(received);
            PrintResponse($"Декодированное сообщение: {string.Join("", decoded)}");
        }
    }
}
