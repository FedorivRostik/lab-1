using System;
using System.Text;

class DES
{
    // Initial Permutation Table
    private static readonly int[] IP = {
        58, 50, 42, 34, 26, 18, 10, 2,
        60, 52, 44, 36, 28, 20, 12, 4,
        62, 54, 46, 38, 30, 22, 14, 6,
        64, 56, 48, 40, 32, 24, 16, 8,
        57, 49, 41, 33, 25, 17, 9, 1,
        59, 51, 43, 35, 27, 19, 11, 3,
        61, 53, 45, 37, 29, 21, 13, 5,
        63, 55, 47, 39, 31, 23, 15, 7
    };

    // Final Permutation Table
    private static readonly int[] FP = {
        40, 8, 48, 16, 56, 24, 64, 32,
        39, 7, 47, 15, 55, 23, 63, 31,
        38, 6, 46, 14, 54, 22, 62, 30,
        37, 5, 45, 13, 53, 21, 61, 29,
        36, 4, 44, 12, 52, 20, 60, 28,
        35, 3, 43, 11, 51, 19, 59, 27,
        34, 2, 42, 10, 50, 18, 58, 26,
        33, 1, 41, 9, 49, 17, 57, 25
    };

    // Expansion Table
    private static readonly int[] E = {
        32, 1, 2, 3, 4, 5,
        4, 5, 6, 7, 8, 9,
        8, 9, 10, 11, 12, 13,
        12, 13, 14, 15, 16, 17,
        16, 17, 18, 19, 20, 21,
        20, 21, 22, 23, 24, 25,
        24, 25, 26, 27, 28, 29,
        28, 29, 30, 31, 32, 1
    };

    // Permutation Table
    private static readonly int[] P = {
        16, 7, 20, 21, 29, 12, 28, 17,
        1, 15, 23, 26, 5, 18, 31, 10,
        2, 8, 24, 14, 32, 27, 3, 9,
        19, 13, 30, 6, 22, 11, 4, 25
    };

    // S-boxes (всі 8 S-блоків)
    private static readonly int[,,] SBox = {
        { // S1
            {14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7},
            {0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8},
            {4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0},
            {15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13}
        },
        { // S2
            {15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10},
            {3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5},
            {0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15},
            {13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9}
        },
        { // S3
            {10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8},
            {13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1},
            {13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7},
            {1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12}
        },
        { // S4
            {7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15},
            {13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9},
            {10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4},
            {3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14}
        },
        { // S5
            {2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9},
            {14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6},
            {4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14},
            {11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3}
        },
        { // S6
            {12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11},
            {10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8},
            {9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6},
            {4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13}
        },
        { // S7
            {4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1},
            {13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6},
            {1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2},
            {6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12}
        },
        { // S8
            {13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7},
            {1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2},
            {7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8},
            {2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11}
        }
    };

    // PC-1 Table
    private static readonly int[] PC1 = {
        57, 49, 41, 33, 25, 17, 9,
        1, 58, 50, 42, 34, 26, 18,
        10, 2, 59, 51, 43, 35, 27,
        19, 11, 3, 60, 52, 44, 36,
        63, 55, 47, 39, 31, 23, 15,
        7, 62, 54, 46, 38, 30, 22,
        14, 6, 61, 53, 45, 37, 29,
        21, 13, 5, 28, 20, 12, 4
    };

    // PC-2 Table
    private static readonly int[] PC2 = {
        14, 17, 11, 24, 1, 5,
        3, 28, 15, 6, 21, 10,
        23, 19, 12, 4, 26, 8,
        16, 7, 27, 20, 13, 2,
        41, 52, 31, 37, 47, 55,
        30, 40, 51, 45, 33, 48,
        44, 49, 39, 56, 34, 53,
        46, 42, 50, 36, 29, 32
    };

    private static readonly int[] ShiftBits = { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };
    private ulong[] roundKeys = new ulong[16];

    // Генерація раундових ключів
    private void GenerateRoundKeys(ulong key)
    {
        ulong permutedChoice1 = PermuteBits(key, PC1, 64, 56);

        ulong c = (permutedChoice1 >> 28) & 0x0FFFFFFF;
        ulong d = permutedChoice1 & 0x0FFFFFFF;

        for (int i = 0; i < 16; i++)
        {
            // Циклічний зсув вліво
            c = ((c << ShiftBits[i]) | (c >> (28 - ShiftBits[i]))) & 0x0FFFFFFF;
            d = ((d << ShiftBits[i]) | (d >> (28 - ShiftBits[i]))) & 0x0FFFFFFF;

            // Об'єднання і застосування PC-2
            ulong cd = (c << 28) | d;
            roundKeys[i] = PermuteBits(cd, PC2, 56, 48);
        }
    }

    private uint F(uint right, ulong roundKey)
    {
        // Розширення до 48 біт
        ulong expanded = PermuteBits(right, E, 32, 48);

        // XOR з ключем раунду
        expanded ^= roundKey;

        // S-box підстановки
        uint output = 0;
        for (int i = 0; i < 8; i++)
        {
            // Виділяємо 6 бітів для поточного S-блоку
            int sixBits = (int)((expanded >> (42 - i * 6)) & 0x3F);

            // Обчислюємо рядок і стовпець для S-блоку
            int row = ((sixBits & 0x20) >> 4) | (sixBits & 0x01); // Перший і останній біти
            int col = (sixBits >> 1) & 0x0F; // Середні 4 біти

            // Отримуємо значення з S-блоку і додаємо до результату
            output = (output << 4) | (uint)SBox[i, row, col];
        }

        // P-box перестановка
        return (uint)PermuteBits(output, P, 32, 32);
    }

    // Оновлена версія PermuteBits для кращої обробки бітів
    private ulong PermuteBits(ulong input, int[] table, int inputLength, int outputLength)
    {
        ulong output = 0;
        for (int i = 0; i < table.Length; i++)
        {
            int sourcePos = table[i] - 1; // Віднімаємо 1, бо таблиці використовують нумерацію з 1
            ulong bit = (input >> (inputLength - 1 - sourcePos)) & 1;
            output |= bit << (outputLength - 1 - i);
        }
        return output;
    }

    // Оновлена версія PermuteBits для роботи з uint
    private ulong PermuteBits(uint input, int[] table, int inputLength, int outputLength)
    {
        return PermuteBits((ulong)input, table, inputLength, outputLength);
    }

    public ulong EncryptBlock(ulong block, ulong key)
    {
        GenerateRoundKeys(key);

        // Початкова перестановка
        ulong state = PermuteBits(block, IP, 64, 64);

        uint left = (uint)(state >> 32);
        uint right = (uint)(state & 0xFFFFFFFF);

        // 16 раундів
        for (int i = 0; i < 16; i++)
        {
            uint temp = right;
            right = left ^ F(right, roundKeys[i]);
            left = temp;
        }

        // Фінальна перестановка
        state = ((ulong)right << 32) | left;
        return PermuteBits(state, FP, 64, 64);
    }

    public ulong DecryptBlock(ulong block, ulong key)
    {
        GenerateRoundKeys(key);

        // Початкова перестановка
        ulong state = PermuteBits(block, IP, 64, 64);

        uint left = (uint)(state >> 32);
        uint right = (uint)(state & 0xFFFFFFFF);

        // 16 раундів у зворотному порядку
        for (int i = 15; i >= 0; i--)
        {
            uint temp = right;
            right = left ^ F(right, roundKeys[i]);
            left = temp;
        }

        // Фінальна перестановка
        state = ((ulong)right << 32) | left;
        return PermuteBits(state, FP, 64, 64);
    }

    // Допоміжні методи для зручності використання
    // Допоміжні методи для зручності використання
    public string EncryptText(string text, ulong key)
    {
        Console.WriteLine($"Початковий текст: {text}");

        // Перетворення тексту в байти з використанням UTF8
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        Console.WriteLine($"Байти UTF8 (довжина: {bytes.Length}): {BitConverter.ToString(bytes)}");

        // Доповнення до кратності 8 байт (PKCS7 padding)
        int paddingLength = 8 - (bytes.Length % 8);
        if (paddingLength == 0) paddingLength = 8;
        Console.WriteLine($"Довжина паддінгу: {paddingLength}");

        // Створюємо новий масив з врахуванням паддінгу
        byte[] paddedBytes = new byte[bytes.Length + paddingLength];
        Console.WriteLine($"Розмір масиву з паддінгом: {paddedBytes.Length}");

        // Копіюємо оригінальні байти
        Buffer.BlockCopy(bytes, 0, paddedBytes, 0, bytes.Length);

        // Додаємо padding
        for (int i = bytes.Length; i < paddedBytes.Length; i++)
        {
            paddedBytes[i] = (byte)paddingLength;
        }
        Console.WriteLine($"Байти з паддінгом: {BitConverter.ToString(paddedBytes)}");

        // Шифрування кожного блоку
        byte[] encryptedBytes = new byte[paddedBytes.Length];

        try
        {
            for (int i = 0; i < paddedBytes.Length; i += 8)
            {
                Console.WriteLine($"\nОбробка блоку {i / 8 + 1}:");

                // Створюємо тимчасовий буфер для блоку
                byte[] block = new byte[8];
                Buffer.BlockCopy(paddedBytes, i, block, 0, 8);
                Console.WriteLine($"Блок для шифрування: {BitConverter.ToString(block)}");

                // Конвертація байтів у ulong
                ulong blockValue = 0;
                for (int j = 0; j < 8; j++)
                {
                    blockValue = (blockValue << 8) | block[j];
                }
                Console.WriteLine($"Значення блоку (hex): {blockValue:X16}");

                // Шифрування блоку
                ulong encryptedBlock = EncryptBlock(blockValue, key);
                Console.WriteLine($"Зашифрований блок (hex): {encryptedBlock:X16}");

                // Конвертація назад у байти
                for (int j = 7; j >= 0; j--)
                {
                    block[j] = (byte)(encryptedBlock & 0xFF);
                    encryptedBlock >>= 8;
                }

                // Копіюємо зашифрований блок назад
                Buffer.BlockCopy(block, 0, encryptedBytes, i, 8);
                Console.WriteLine($"Зашифровані байти блоку: {BitConverter.ToString(block)}");
            }

            // Кодування результату в Base64
            string result = Convert.ToBase64String(encryptedBytes);
            Console.WriteLine($"\nФінальний результат (Base64): {result}");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nВиникла помилка під час шифрування: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public string DecryptText(string encryptedText, ulong key)
    {
        try
        {
            Console.WriteLine($"Початковий Base64 текст: {encryptedText}");

            // Декодування з Base64
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            Console.WriteLine($"Декодовані байти: {BitConverter.ToString(encryptedBytes)}");

            if (encryptedBytes.Length % 8 != 0)
            {
                throw new ArgumentException($"Зашифровані дані пошкоджені. Довжина: {encryptedBytes.Length}");
            }

            byte[] decryptedBytes = new byte[encryptedBytes.Length];

            for (int i = 0; i < encryptedBytes.Length; i += 8)
            {
                Console.WriteLine($"\nРозшифрування блоку {i / 8 + 1}:");

                byte[] block = new byte[8];
                Buffer.BlockCopy(encryptedBytes, i, block, 0, 8);
                Console.WriteLine($"Блок для розшифрування: {BitConverter.ToString(block)}");

                ulong blockValue = 0;
                for (int j = 0; j < 8; j++)
                {
                    blockValue = (blockValue << 8) | block[j];
                }
                Console.WriteLine($"Значення блоку (hex): {blockValue:X16}");

                ulong decryptedBlock = DecryptBlock(blockValue, key);
                Console.WriteLine($"Розшифрований блок (hex): {decryptedBlock:X16}");

                for (int j = 7; j >= 0; j--)
                {
                    block[j] = (byte)(decryptedBlock & 0xFF);
                    decryptedBlock >>= 8;
                }

                Buffer.BlockCopy(block, 0, decryptedBytes, i, 8);
                Console.WriteLine($"Розшифровані байти блоку: {BitConverter.ToString(block)}");
            }

            int paddingLength = decryptedBytes[decryptedBytes.Length - 1];
            Console.WriteLine($"\nДовжина виявленого паддінгу: {paddingLength}");

            if (paddingLength < 1 || paddingLength > 8)
            {
                throw new ArgumentException($"Неправильний padding: {paddingLength}");
            }

            // Перевірка правильності padding
            for (int i = decryptedBytes.Length - paddingLength; i < decryptedBytes.Length; i++)
            {
                if (decryptedBytes[i] != paddingLength)
                {
                    throw new ArgumentException($"Неправильний padding на позиції {i}");
                }
            }

            string result = Encoding.UTF8.GetString(decryptedBytes, 0, decryptedBytes.Length - paddingLength);
            Console.WriteLine($"Фінальний результат: {result}");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nВиникла помилка під час розшифрування: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw new Exception("Помилка розшифрування: " + ex.Message);
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        DES des = new DES();

        while (true)
        {
            Console.WriteLine("\nВиберіть операцію:");
            Console.WriteLine("1. Зашифрувати текст");
            Console.WriteLine("2. Розшифрувати текст");
            Console.WriteLine("3. Зашифрувати 64-бітний блок (hex)");
            Console.WriteLine("4. Розшифрувати 64-бітний блок (hex)");
            Console.WriteLine("5. Вийти");

            string choice = Console.ReadLine();
            if (choice == "5") break;

            Console.Write("Введіть ключ (16 символів hex, наприклад 133457799BBCDFF1): ");
            string keyStr = Console.ReadLine();
            ulong key = Convert.ToUInt64(keyStr, 16);

            switch (choice)
            {
                case "1":
                    Console.Write("Введіть текст для шифрування: ");
                    string plaintext = Console.ReadLine();
                    try
                    {
                        string encrypted = des.EncryptText(plaintext, key);
                        Console.WriteLine($"Зашифрований текст (Base64): {encrypted}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Помилка шифрування: {ex.Message}");
                    }
                    break;

                case "2":
                    Console.Write("Введіть зашифрований текст (Base64): ");
                    string encryptedText = Console.ReadLine();
                    try
                    {
                        string decrypted = des.DecryptText(encryptedText, key);
                        Console.WriteLine($"Розшифрований текст: {decrypted}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Помилка розшифрування: {ex.Message}");
                    }
                    break;

                case "3":
                    Console.Write("Введіть 64-бітний блок (hex): ");
                    string blockStr = Console.ReadLine();
                    try
                    {
                        ulong block = Convert.ToUInt64(blockStr, 16);
                        ulong encryptedBlock = des.EncryptBlock(block, key);
                        Console.WriteLine($"Зашифрований блок: {encryptedBlock:X16}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Помилка: {ex.Message}");
                    }
                    break;

                case "4":
                    Console.Write("Введіть зашифрований 64-бітний блок (hex): ");
                    string encryptedBlockStr = Console.ReadLine();
                    try
                    {
                        ulong encryptedBlock = Convert.ToUInt64(encryptedBlockStr, 16);
                        ulong decryptedBlock = des.DecryptBlock(encryptedBlock, key);
                        Console.WriteLine($"Розшифрований блок: {decryptedBlock:X16}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Помилка: {ex.Message}");
                    }
                    break;

                default:
                    Console.WriteLine("Невірний вибір!");
                    break;
            }
        }
    }
}