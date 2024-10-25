static class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

        // Тестування матричного шифру
        string message = "ПРИКЛАД ПОВІДОМЛЕННЯ";
        string rowKey = "31425";
        string colKey = "4213";
        
        Console.WriteLine("\nМатричний шифр:");
        string encrypted = MatrixCipher.Encrypt(message, rowKey, colKey);
        Console.WriteLine($"Зашифровано: {encrypted}");
        string decrypted = MatrixCipher.Decrypt(encrypted, rowKey, colKey);
        Console.WriteLine($"Розшифровано: {decrypted}");

        // Тестування шифру Гронсфельда
        string message2 = "ТЕСТОВЕ ПОВІДОМЛЕННЯ";
        string key = "31415";
        
        Console.WriteLine("\nШифр Гронсфельда:");
        string encrypted2 = GronsfeldCipher.Encrypt(message2, key);
        Console.WriteLine($"Зашифровано: {encrypted2}");
        string decrypted2 = GronsfeldCipher.Decrypt(encrypted2, key);
        Console.WriteLine($"Розшифровано: {decrypted2}");

        // Тестування гомофонічного шифру
        string message3 = "ПРИВІТ СВІТ";
        
        Console.WriteLine("\nГомофонічний шифр:");
        var homophonic = new HomophonicCipher();
        string encrypted3 = homophonic.Encrypt(message3);
        Console.WriteLine($"Зашифровано: {encrypted3}");
        string decrypted3 = homophonic.Decrypt(encrypted3);
        Console.WriteLine($"Розшифровано: {decrypted3}");
    }
}
