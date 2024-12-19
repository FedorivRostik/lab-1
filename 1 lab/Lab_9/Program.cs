using System.Numerics;


Random rng = new Random();

// Генерація p за тестом Рабіна-Міллера
BigInteger p;
do
{
    p = ElGamal.RandomBigInteger(1000, 10000, rng);
} while (!ElGamal.IsPrime(p, 40));

Console.WriteLine($"Generated prime p: {p}");

// Вибір первісного кореня g
BigInteger g;
do
{
    g = ElGamal.RandomBigInteger(2, p - 1, rng);
} while (!ElGamal.IsPrimitiveRoot(g, p));

Console.WriteLine($"Primitive root g: {g}");

// Генерація приватного та публічного ключів
BigInteger x = ElGamal.RandomBigInteger(2, p - 2, rng); // Приватний ключ
BigInteger y = BigInteger.ModPow(g, x, p);       // Публічний ключ

Console.WriteLine($"Private key x: {x}");
Console.WriteLine($"Public key y: {y}");

// Шифрування
Console.Write("Enter message to encrypt (as integer): ");
BigInteger m = BigInteger.Parse(Console.ReadLine());

BigInteger k = ElGamal.RandomBigInteger(2, p - 2, rng);
BigInteger c1 = BigInteger.ModPow(g, k, p);
BigInteger c2 = (m * BigInteger.ModPow(y, k, p)) % p;

Console.WriteLine($"Encrypted message: c1 = {c1}, c2 = {c2}");

// Дешифрування
BigInteger decrypted = (c2 * BigInteger.ModPow(c1, p - 1 - x, p)) % p;
Console.WriteLine($"Decrypted message: {decrypted}");
public class ElGamal
{
    // Перевірка числа на простоту за тестом Рабіна-Міллера
    public static bool IsPrime(BigInteger n, int k)
    {
        if (n < 2) return false;
        if (n != 2 && n % 2 == 0) return false;

        BigInteger s = n - 1;
        while (s % 2 == 0) s /= 2;

        Random rng = new Random();
        for (int i = 0; i < k; i++)
        {
            BigInteger a = RandomBigInteger(2, n - 1, rng);
            BigInteger temp = s;
            BigInteger mod = BigInteger.ModPow(a, temp, n);
            while (temp != n - 1 && mod != 1 && mod != n - 1)
            {
                mod = BigInteger.ModPow(mod, 2, n);
                temp *= 2;
            }
            if (mod != n - 1 && temp % 2 == 0) return false;
        }
        return true;
    }

    // Генерація випадкового числа у вказаному діапазоні
    public static BigInteger RandomBigInteger(BigInteger min, BigInteger max, Random rng)
    {
        byte[] bytes = max.ToByteArray();
        BigInteger result;
        do
        {
            rng.NextBytes(bytes);
            bytes[^1] &= 0x7F; // Зробити число додатним
            result = new BigInteger(bytes);
        } while (result < min || result >= max);
        return result;
    }

    // Перевірка, чи є g первісним коренем за модулем p
    public static bool IsPrimitiveRoot(BigInteger g, BigInteger p)
    {
        BigInteger phi = p - 1;
        for (BigInteger i = 2; i * i <= phi; i++)
        {
            if (phi % i == 0)
            {
                if (BigInteger.ModPow(g, i, p) == 1 || BigInteger.ModPow(g, phi / i, p) == 1)
                    return false;
            }
        }
        return true;
    }
}