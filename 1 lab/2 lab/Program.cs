namespace _2_lab;

internal class Program
{
    // 1. Ітераційний розширений алгоритм Евкліда для знаходження трійки (d, x, y), де ax + by = d
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

    // 2. Знаходження мультиплікативного оберненого елемента по модулю n, використовуючи GcdEx
    public static int InverseElement(int a, int n)
    {
        var (d, x, _) = GcdEx(a, n);
        if (d != 1)
        {
            throw new ArgumentException($"Немає мультиплікативного оберненого для {a} по модулю {n}");
        }
        return (x % n + n) % n;  // Перевірка на додатне значення
    }

    // 3. Функція Ейлера phi(m)
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

    // 4. Знаходження оберненого елемента по модулю n за допомогою теореми Ейлера або Ферма
    public static int InverseElement2(int a, int n)
    {
        var (d, _, _) = GcdEx(a, n);
        if (d != 1)
        {
            throw new ArgumentException($"Немає мультиплікативного оберненого для {a} по модулю {n}");
        }

        // Перевірка, чи n є простим
        bool isPrime = IsPrime(n);
        int exponent = isPrime ? n - 2 : Phi(n) - 1;

        return ModPow(a, exponent, n);
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

    // Допоміжна функція для перевірки, чи число є простим
    private static bool IsPrime(int number)
    {
        if (number <= 1) return false;
        if (number <= 3) return true;
        if (number % 2 == 0 || number % 3 == 0) return false;
        for (int i = 5; i * i <= number; i += 6)
        {
            if (number % i == 0 || number % (i + 2) == 0)
                return false;
        }
        return true;
    }

    static void Main()
    {
        // Тест для GcdEx
        var (d, x, y) = GcdEx(612, 342);
        Console.WriteLine($"GCD(612, 342) = {d}, x = {x}, y = {y}"); // Очікується, що ax + by = d

        // Тест для InverseElement
        try
        {
            int inverse1 = InverseElement(5, 18);
            Console.WriteLine($"Мультиплікативний обернений елемент для 5 по модулю 18: {inverse1}"); // Очікується 11
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
        }

        // Тест для Phi
        int phi18 = Phi(18);
        Console.WriteLine($"Функція Ейлера для m = 18: {phi18}"); // Очікується 6

        // Тест для InverseElement2 з модулем n = 18 (складене число)
        try
        {
            int inverse2 = InverseElement2(5, 18); // Повинно збігатися з InverseElement
            Console.WriteLine($"Мультиплікативний обернений елемент для 5 по модулю 18 за допомогою теореми Ейлера: {inverse2}");
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

