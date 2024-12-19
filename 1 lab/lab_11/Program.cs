using System;

class EllipticCurveElGamal
{
    static void Main()
    {
        // Параметри еліптичної кривої
        int p = 23, a = 1, b = 1; // y^2 = x^3 + x + 1 mod 23
        (int x, int y) G = (17, 20); // Генератор

        // Приватний ключ
        int d = 7; // Випадкове число (секретний ключ)

        // Публічний ключ
        (int x, int y) Q = MultiplyPoint(G, d, a, p);

        Console.WriteLine($"Публічний ключ Q = ({Q.x}, {Q.y})");

        // Повідомлення для шифрування (точка на кривій)
        (int x, int y) M = (12, 4);

        // Шифрування
        int k = 5; // Випадкове число для шифрування
        var (C1, C2) = Encrypt(M, G, Q, k, a, p);

        Console.WriteLine($"Зашифроване повідомлення: C1 = ({C1.x}, {C1.y}), C2 = ({C2.x}, {C2.y})");

        // Розшифрування
        var decryptedMessage = Decrypt(C1, C2, d, a, p);

        Console.WriteLine($"Розшифроване повідомлення: ({decryptedMessage.x}, {decryptedMessage.y})");
    }

    // Функція шифрування
    static ((int x, int y), (int x, int y)) Encrypt((int x, int y) M, (int x, int y) G, (int x, int y) Q, int k, int a, int p)
    {
        var C1 = MultiplyPoint(G, k, a, p);
        var C2 = AddPoints(M, MultiplyPoint(Q, k, a, p), a, p);
        return (C1, C2);
    }

    // Функція розшифрування
    static (int x, int y) Decrypt((int x, int y) C1, (int x, int y) C2, int d, int a, int p)
    {
        var dC1 = MultiplyPoint(C1, d, a, p);
        var neg_dC1 = (dC1.x, (p - dC1.y) % p); // Інверсія точки
        return AddPoints(C2, neg_dC1, a, p);
    }

    // Додавання точок на еліптичній кривій
    static (int x, int y) AddPoints((int x, int y) P, (int x, int y) Q, int a, int p)
    {
        if (P == (0, 0)) return Q;
        if (Q == (0, 0)) return P;

        int lambda;
        if (P.x == Q.x && P.y == Q.y)
        {
            lambda = (3 * ModularPow(P.x, 2, p) + a) * ModularInverse(2 * P.y, p) % p;
        }
        else
        {
            lambda = (Q.y - P.y) * ModularInverse(Q.x - P.x, p) % p;
        }

        if (lambda < 0) lambda += p;

        int x3 = (ModularPow(lambda, 2, p) - P.x - Q.x) % p;
        if (x3 < 0) x3 += p;

        int y3 = (lambda * (P.x - x3) - P.y) % p;
        if (y3 < 0) y3 += p;

        return (x3, y3);
    }

    // Множення точки на скаляр
    static (int x, int y) MultiplyPoint((int x, int y) P, int k, int a, int p)
    {
        (int x, int y) result = (0, 0);
        (int x, int y) addend = P;

        while (k > 0)
        {
            if ((k & 1) == 1)
            {
                result = AddPoints(result, addend, a, p);
            }
            addend = AddPoints(addend, addend, a, p);
            k >>= 1;
        }

        return result;
    }

    // Обчислення оберненого за модулем
    static int ModularInverse(int a, int mod)
    {
        int m0 = mod, t, q;
        int x0 = 0, x1 = 1;

        if (mod == 1) return 0;

        while (a > 1)
        {
            q = a / mod;
            t = mod;
            mod = a % mod;
            a = t;
            t = x0;
            x0 = x1 - q * x0;
            x1 = t;
        }

        if (x1 < 0) x1 += m0;

        return x1;
    }

    // Обчислення (base^exp) mod mod
    static int ModularPow(int baseValue, int exp, int mod)
    {
        int result = 1;
        baseValue = baseValue % mod;

        while (exp > 0)
        {
            if ((exp & 1) == 1)
            {
                result = (result * baseValue) % mod;
            }
            exp >>= 1;
            baseValue = (baseValue * baseValue) % mod;
        }

        return result;
    }
}
