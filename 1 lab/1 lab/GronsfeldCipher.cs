using System.Text;
// 2. Шифр Гронсфельда
class GronsfeldCipher
{
    private static readonly string ukrainianAlphabet = "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ";

    public static string Encrypt(string message, string key)
    {
        message = message.Replace(" ", "").ToUpper();
        var result = new StringBuilder();

        for (int i = 0; i < message.Length; i++)
        {
            int shift = int.Parse(key[i % key.Length].ToString());
            int charIndex = ukrainianAlphabet.IndexOf(message[i]);
            
            if (charIndex != -1)
            {
                int newIndex = (charIndex + shift) % ukrainianAlphabet.Length;
                result.Append(ukrainianAlphabet[newIndex]);
            }
            else
            {
                result.Append(message[i]);
            }
        }

        return result.ToString();
    }

    public static string Decrypt(string encrypted, string key)
    {
        var result = new StringBuilder();

        for (int i = 0; i < encrypted.Length; i++)
        {
            int shift = int.Parse(key[i % key.Length].ToString());
            int charIndex = ukrainianAlphabet.IndexOf(encrypted[i]);
            
            if (charIndex != -1)
            {
                int newIndex = (charIndex - shift + ukrainianAlphabet.Length) % ukrainianAlphabet.Length;
                result.Append(ukrainianAlphabet[newIndex]);
            }
            else
            {
                result.Append(encrypted[i]);
            }
        }

        return result.ToString();
    }
}
