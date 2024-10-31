namespace _2_lab;

internal class Program
{
    // 1. Функція для ітераційного розширеного алгоритму Евкліда
    public static (int d, int x, int y) GcdEx(int a, int b)
    {
        int x0 = 1, y0 = 0;
        int x1 = 0, y1 = 1;

        while (b != 0)
        {
            int q = a / b;
            int r = a % b;
            a = b;
            b = r;

            int tempX = x1;
            x1 = x0 - q * x1;
            x0 = tempX;

            int tempY = y1;
            y1 = y0 - q * y1;
            y0 = tempY;
        }

        return (a, x0, y0);
    }

    // 2. Функція для знаходження мультиплікативного оберненого елемента
    public static int InverseElement(int a, int n)
    {
        var (d, x, _) = GcdEx(a, n);
        if (d != 1)
        {
            throw new ArgumentException($"Немає мультиплікативного оберненого для {a} по модулю {n}");
        }
        return (x % n + n) % n;  // Перевірка на додатне значення
    }

    // 3. Функція для обчислення значення функції Ейлера
    public static int Phi(int m)
    {
        int result = m;
        for (int i = 2; i * i <= m; i++)
        {
            if (m % i == 0)
            {
                while (m % i == 0)
                    m /= i;
                result -= result / i;
            }
        }
        if (m > 1)
            result -= result / m;
        return result;
    }

    // 4. Функція для оберненого елемента з використанням теореми Ферма (для простого n)
    public static int InverseElement2(int a, int p)
    {
        if (GcdEx(a, p).d != 1)
        {
            throw new ArgumentException($"Немає мультиплікативного оберненого для {a} по модулю {p}");
        }
        return ModPow(a, p - 2, p);  // Теорема Ферма: a^(p-1) ≡ 1 (mod p), тому a^(p-2) ≡ a^(-1) (mod p)
    }

    // Допоміжна функція для піднесення до степеня по модулю
    private static int ModPow(int baseValue, int exp, int mod)
    {
        int result = 1;
        baseValue = baseValue % mod;
        while (exp > 0)
        {
            if ((exp & 1) == 1)
                result = (result * baseValue) % mod;
            exp >>= 1;
            baseValue = (baseValue * baseValue) % mod;
        }
        return result;
    }

    static void Main()
    {
        // Тести для GcdEx
        var (d, x, y) = GcdEx(612, 342);
        Console.WriteLine($"GCD(612, 342) = {d}, x = {x}, y = {y}");

        // Тести для InverseElement
        try
        {
            int inverse1 = InverseElement(5, 18);
            Console.WriteLine($"Мультиплікативний обернений елемент для 5 по модулю 18: {inverse1}");
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
        }

        // Тести для Phi
        int phi18 = Phi(18);
        Console.WriteLine($"Функція Ейлера для m = 18: {phi18}");

        // Тести для InverseElement2
        try
        {
            int inverse2 = InverseElement2(5, 18);
            Console.WriteLine($"Мультиплікативний обернений елемент для 5 по модулю 18 за допомогою теореми Ферма: {inverse2}");
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
