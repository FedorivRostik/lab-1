// See https://aka.ms/new-console-template for more information
using System.Reflection;
Console.OutputEncoding = System.Text.Encoding.UTF8;

// Многочлен для модульного ділення (x^8 + x^4 + x^3 + x + 1)
 const byte MODULO = 0x1B;


    // Тестування множення на 02
    byte test1 = 0xD4;
    byte result1 = Mul02(test1);
    Console.WriteLine($"D4 * 02 = {result1:X2} (очікується B3)");

    // Тестування множення на 03
    byte test2 = 0xBF;
    byte result2 = Mul03(test2);
    Console.WriteLine($"BF * 03 = {result2:X2} (очікується DA)");

    // Тестування множення двох довільних байтів
    byte a = 0x57;
    byte b = 0x83;
    byte result3 = MultiplyBytes(a, b);
    Console.WriteLine($"57 * 83 = {result3:X2} (очікується C1)");

    // Додаткова демонстрація проміжних кроків для множення 57 * 83
    Console.WriteLine("\nДемонстрація кроків множення 57 * 83:");
    DemonstrateMultiplication(0x57, 0x83);


// Множення байту на 02 (x)
static byte Mul02(byte b)
{
    // Зсув вліво на 1 біт
    byte result = (byte)(b << 1);

    // Якщо старший біт був 1, виконуємо XOR з поліномом
    if ((b & 0x80) != 0)
    {
        result ^= MODULO;
    }

    return result;
}

// Множення байту на 03 (x + 1)
static byte Mul03(byte b)
{
    // 03 = 02 + 01, тому множимо на 02 і додаємо початкове значення
    return (byte)(Mul02(b) ^ b);
}

// Множення двох довільних байтів
static byte MultiplyBytes(byte a, byte b)
{
    byte result = 0;
    byte temp = a;

    // Перебираємо всі біти другого множника
    for (int i = 0; i < 8; i++)
    {
        // Якщо поточний біт 1, додаємо поточне значення temp до результату
        if ((b & 0x01) != 0)
        {
            result ^= temp;
        }

        // Зсуваємо b вправо для перевірки наступного біту
        b >>= 1;

        // Множимо temp на x (02)
        temp = Mul02(temp);
    }

    return result;
}

// Метод для демонстрації проміжних кроків множення
static void DemonstrateMultiplication(byte a, byte b)
{
    byte result = 0;
    byte temp = a;

    Console.WriteLine($"Множення {a:X2} на {b:X2}:");

    for (int i = 0; i < 8; i++)
    {
        if ((b & 0x01) != 0)
        {
            Console.WriteLine($"Біт {i}: 1, додаємо {temp:X2} до результату");
            result ^= temp;
            Console.WriteLine($"Проміжний результат: {result:X2}");
        }
        else
        {
            Console.WriteLine($"Біт {i}: 0, пропускаємо");
        }

        b >>= 1;
        temp = Mul02(temp);
        Console.WriteLine($"Нове значення temp після множення на x: {temp:X2}");
    }

    Console.WriteLine($"Кінцевий результат: {result:X2}");
}
