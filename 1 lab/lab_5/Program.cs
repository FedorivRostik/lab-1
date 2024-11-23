// See https://aka.ms/new-console-template for more information
using System.Numerics;
using System.Security.Cryptography;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Демонстрація тесту Міллера-Рабіна
Console.WriteLine("Тест Міллера-Рабіна:");
BigInteger numberToTest = 104729;
int k = 10; // кількість раундів
bool isPrime = MillerRabinTest(numberToTest, k);
Console.WriteLine($"Число {numberToTest} є {(isPrime ? "простим" : "складеним")}");
if (isPrime)
{
    double probability = 1 - Math.Pow(4, -k);
    Console.WriteLine($"Ймовірність простоти: {probability * 100}%");
}

Console.WriteLine("\nГенерація ключів RSA...");
// Генерація великих простих чисел для RSA
BigInteger p = GenerateLargePrime(1024);
BigInteger q = GenerateLargePrime(1024);

// Обчислення параметрів RSA
BigInteger n = p * q;
BigInteger phi = (p - 1) * (q - 1);

// Вибір відкритого експоненти e
BigInteger e = 65537; // Стандартне значення для e

// Обчислення закритого ключа d
BigInteger d = ModInverse(e, phi);

Console.WriteLine("Параметри RSA:");
Console.WriteLine($"p = {p}");
Console.WriteLine($"q = {q}");
Console.WriteLine($"n = {n}");
Console.WriteLine($"e = {e}");
Console.WriteLine($"d = {d}");

// Демонстрація шифрування/розшифрування
string message = "Hello, World!";
Console.WriteLine($"\nПовідомлення для шифрування: {message}");

// Конвертація повідомлення в число
byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
BigInteger m = new BigInteger(messageBytes.Concat(new byte[] { 0 }).ToArray());

// Шифрування
BigInteger encrypted = BigInteger.ModPow(m, e, n);
Console.WriteLine($"Зашифроване повідомлення (число): {encrypted}");

// Розшифрування
BigInteger decrypted = BigInteger.ModPow(encrypted, d, n);
byte[] decryptedBytes = decrypted.ToByteArray();
Array.Resize(ref decryptedBytes, decryptedBytes.Length - 1);
string decryptedMessage = System.Text.Encoding.UTF8.GetString(decryptedBytes);
Console.WriteLine($"Розшифроване повідомлення: {decryptedMessage}");
    

    // Тест Міллера-Рабіна
    static bool MillerRabinTest(BigInteger n, int k)
{
    if (n <= 1 || n == 4) return false;
    if (n <= 3) return true;
    if (n % 2 == 0) return false;

    // Представлення n - 1 як 2^r * d
    BigInteger d = n - 1;
    int r = 0;
    while (d % 2 == 0)
    {
        d /= 2;
        r++;
    }

    // Використовуємо криптографічний генератор випадкових чисел
    using (var rng = RandomNumberGenerator.Create())
    {
        // k раундів тестування
        for (int i = 0; i < k; i++)
        {
            BigInteger a = GenerateRandomBigInteger(2, n - 2, rng);
            BigInteger x = BigInteger.ModPow(a, d, n);

            if (x == 1 || x == n - 1)
                continue;

            bool isProbablyPrime = false;
            for (int j = 0; j < r - 1; j++)
            {
                x = BigInteger.ModPow(x, 2, n);
                if (x == n - 1)
                {
                    isProbablyPrime = true;
                    break;
                }
            }

            if (!isProbablyPrime)
                return false;
        }
    }

    return true;
}

// Генерація випадкового великого числа
static BigInteger GenerateRandomBigInteger(BigInteger min, BigInteger max, RandomNumberGenerator rng)
{
    if (min > max)
        throw new ArgumentException("min must be less than max");

    BigInteger diff = max - min;
    byte[] bytes = diff.ToByteArray();
    byte[] result = new byte[bytes.Length];

    rng.GetBytes(result);
    BigInteger randomNumber = new BigInteger(result);
    randomNumber = BigInteger.Abs(randomNumber % diff + min);

    return randomNumber;
}

// Генерація великого простого числа заданої бітової довжини
static BigInteger GenerateLargePrime(int bits)
{
    using (var rng = RandomNumberGenerator.Create())
    {
        while (true)
        {
            byte[] bytes = new byte[bits / 8];
            rng.GetBytes(bytes);
            bytes[bytes.Length - 1] |= 0x01; // Робимо число непарним
            bytes[0] |= 0x80; // Встановлюємо старший біт

            BigInteger number = new BigInteger(bytes.Concat(new byte[] { 0 }).ToArray());
            if (MillerRabinTest(number, 50)) // 50 раундів для надійності
                return number;
        }
    }
}

// Розширений алгоритм Евкліда для знаходження мультиплікативного оберненого
static BigInteger ModInverse(BigInteger e, BigInteger phi)
{
    BigInteger old_r = e;
    BigInteger r = phi;
    BigInteger old_s = 1;
    BigInteger s = 0;
    BigInteger old_t = 0;
    BigInteger t = 1;

    while (r != 0)
    {
        BigInteger quotient = old_r / r;
        BigInteger temp = r;
        r = old_r - quotient * r;
        old_r = temp;

        temp = s;
        s = old_s - quotient * s;
        old_s = temp;

        temp = t;
        t = old_t - quotient * t;
        old_t = temp;
    }

    if (old_s < 0)
        old_s += phi;

    return old_s;
}