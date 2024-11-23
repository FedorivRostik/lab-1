using System;
using System.Numerics;
using System.Security.Cryptography;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("Генерація параметрів протоколу Діффі-Геллмана...\n");

        // Генеруємо безпечне просте число p (p = 2q + 1, де q теж просте)
        var (p, q) = GenerateSafePrime(512); // 512 біт для демонстрації
        Console.WriteLine($"Згенероване безпечне просте число p: {p}");
        Console.WriteLine($"q = (p-1)/2: {q}");

        // Знаходимо примітивний корінь g
        BigInteger g = FindPrimitiveRoot(p, q);
        Console.WriteLine($"\nЗнайдений примітивний корінь g: {g}");

        // Симуляція обміну ключами
        Console.WriteLine("\nСимуляція обміну ключами:");

        // Аліса генерує приватний ключ a
        BigInteger a = GeneratePrivateKey(p);
        Console.WriteLine("\nАліса:");
        Console.WriteLine($"Приватний ключ a: {a}");

        // Аліса обчислює відкритий ключ A = g^a mod p
        BigInteger A = BigInteger.ModPow(g, a, p);
        Console.WriteLine($"Відкритий ключ A = g^a mod p: {A}");

        // Боб генерує приватний ключ b
        BigInteger b = GeneratePrivateKey(p);
        Console.WriteLine("\nБоб:");
        Console.WriteLine($"Приватний ключ b: {b}");

        // Боб обчислює відкритий ключ B = g^b mod p
        BigInteger B = BigInteger.ModPow(g, b, p);
        Console.WriteLine($"Відкритий ключ B = g^b mod p: {B}");

        // Обчислення спільного секретного ключа
        BigInteger secretAlice = BigInteger.ModPow(B, a, p);
        BigInteger secretBob = BigInteger.ModPow(A, b, p);

        Console.WriteLine("\nРезультати обміну:");
        Console.WriteLine($"Секретний ключ Аліси (B^a mod p): {secretAlice}");
        Console.WriteLine($"Секретний ключ Боба (A^b mod p):   {secretBob}");
        Console.WriteLine($"Ключі {(secretAlice == secretBob ? "співпадають" : "не співпадають")}");
    }

    // Генерація безпечного простого числа p = 2q + 1, де q теж просте
    static (BigInteger p, BigInteger q) GenerateSafePrime(int bits)
    {
        while (true)
        {
            // Генеруємо випадкове q
            BigInteger q = GenerateRandomPrime(bits - 1);
            // Обчислюємо p = 2q + 1
            BigInteger p = 2 * q + 1;

            // Перевіряємо, чи p теж просте
            if (MillerRabinTest(p, 50))
            {
                return (p, q);
            }
        }
    }

    // Генерація випадкового простого числа заданої бітової довжини
    static BigInteger GenerateRandomPrime(int bits)
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            while (true)
            {
                byte[] bytes = new byte[bits / 8 + 1];
                rng.GetBytes(bytes);

                // Встановлюємо старший біт
                bytes[^1] &= 0x7F;
                bytes[0] |= 0x01;

                BigInteger number = new BigInteger(bytes);

                // Забезпечуємо потрібну кількість біт
                number |= BigInteger.One << (bits - 1);

                // Перевіряємо на простоту
                if (MillerRabinTest(number, 50))
                {
                    return number;
                }
            }
        }
    }

    // Тест Міллера-Рабіна
    static bool MillerRabinTest(BigInteger n, int k)
    {
        if (n <= 1 || n == 4) return false;
        if (n <= 3) return true;
        if (n % 2 == 0) return false;

        // Представляємо n - 1 як 2^s * d
        BigInteger d = n - 1;
        int s = 0;
        while (d % 2 == 0)
        {
            d /= 2;
            s++;
        }

        using (var rng = RandomNumberGenerator.Create())
        {
            for (int i = 0; i < k; i++)
            {
                BigInteger a = GenerateRandomBigInteger(2, n - 2, rng);
                BigInteger x = BigInteger.ModPow(a, d, n);

                if (x == 1 || x == n - 1)
                    continue;

                bool isProbablyPrime = false;
                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, n);
                    if (x == 1)
                        return false;
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

    // Знаходження примітивного кореня за модулем p
    static BigInteger FindPrimitiveRoot(BigInteger p, BigInteger q)
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            while (true)
            {
                // Генеруємо випадкове число g
                BigInteger g = GenerateRandomBigInteger(2, p - 1, rng);

                // Перевіряємо умови для примітивного кореня
                if (BigInteger.ModPow(g, 2, p) != 1 &&
                    BigInteger.ModPow(g, q, p) != 1 &&
                    BigInteger.ModPow(g, (p - 1) / 2, p) != 1)
                {
                    return g;
                }
            }
        }
    }

    // Генерація випадкового приватного ключа
    static BigInteger GeneratePrivateKey(BigInteger p)
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            return GenerateRandomBigInteger(2, p - 2, rng);
        }
    }

    // Генерація випадкового BigInteger в заданому діапазоні
    static BigInteger GenerateRandomBigInteger(BigInteger min, BigInteger max, RandomNumberGenerator rng)
    {
        if (min > max)
            throw new ArgumentException("min must be less than max");

        BigInteger diff = max - min;
        int byteLength = diff.ToByteArray().Length;
        byte[] bytes = new byte[byteLength];

        while (true)
        {
            rng.GetBytes(bytes);
            bytes[^1] &= 0x7F; // Забезпечуємо додатне число

            BigInteger result = new BigInteger(bytes);
            result = min + (result % (diff + 1));

            if (result >= min && result <= max)
                return result;
        }
    }
}