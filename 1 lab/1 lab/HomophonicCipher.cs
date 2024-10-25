using System.Text;
// 3. Гомофонічний шифр
class HomophonicCipher
{
    private readonly Dictionary<char, List<string>> encryptionMap;
    private readonly Dictionary<string, char> decryptionMap;
    private readonly Random random;

    public HomophonicCipher()
    {
        random = new Random();
        encryptionMap = new Dictionary<char, List<string>>();
        decryptionMap = new Dictionary<string, char>();

        // Ініціалізація карти шифрування для української мови
        // Кожній літері присвоюємо кілька можливих кодів
        InitializeHomophoneMap();
    }

    private void InitializeHomophoneMap()
    {
        string alphabet = "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ";
        int codeStart = 100;

        foreach (char letter in alphabet)
        {
            var codes = new List<string>();
            // Створюємо 2-3 гомофони для кожної літери
            int homophoneCount = random.Next(2, 4);
            
            for (int i = 0; i < homophoneCount; i++)
            {
                string code = (codeStart++).ToString();
                codes.Add(code);
                decryptionMap[code] = letter;
            }
            
            encryptionMap[letter] = codes;
        }
    }

    public string Encrypt(string message)
    {
        message = message.Replace(" ", "").ToUpper();
        var result = new StringBuilder();

        foreach (char c in message)
        {
            if (encryptionMap.ContainsKey(c))
            {
                var possibleCodes = encryptionMap[c];
                string selectedCode = possibleCodes[random.Next(possibleCodes.Count)];
                result.Append(selectedCode);
                result.Append(" ");
            }
        }

        return result.ToString().Trim();
    }

    public string Decrypt(string encrypted)
    {
        var result = new StringBuilder();
        string[] codes = encrypted.Split(' ');

        foreach (string code in codes)
        {
            if (decryptionMap.ContainsKey(code))
            {
                result.Append(decryptionMap[code]);
            }
        }

        return result.ToString();
    }
}