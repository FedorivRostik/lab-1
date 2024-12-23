using System;
using System.Text;

class Program
{
    public static string ComputeMD5(string input)
    {
        byte[] data = Encoding.UTF8.GetBytes(input);

        // Значення зсувів для кожного раунду
        int[] s = {
            7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,
            5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,
            4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,
            6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21
        };

        // Константи k
        uint[] k = new uint[64];
        for (int i = 0; i < 64; i++)
        {
            k[i] = (uint)(Math.Abs(Math.Sin(i + 1)) * 4294967296); // 2^32 = 4294967296
        }

        // Ініціалізація
        uint a0 = 0x67452301;
        uint b0 = 0xefcdab89;
        uint c0 = 0x98badcfe;
        uint d0 = 0x10325476;

        // Додавання заповнення
        int originalLength = data.Length;
        int bitLen = originalLength * 8;
        int padding = (448 - bitLen % 512 + 512) % 512;
        int totalLen = bitLen + padding + 64;

        byte[] paddedData = new byte[totalLen / 8];
        Array.Copy(data, paddedData, data.Length);
        paddedData[data.Length] = 0x80;

        byte[] lengthBytes = BitConverter.GetBytes((ulong)bitLen);
        Array.Copy(lengthBytes, 0, paddedData, paddedData.Length - 8, 8);

        // Обробка кожного блоку (64 байти)
        for (int i = 0; i < paddedData.Length / 64; i++)
        {
            uint[] chunk = new uint[16];
            for (int j = 0; j < 16; j++)
            {
                chunk[j] = BitConverter.ToUInt32(paddedData, (i * 64) + j * 4);
            }

            uint A = a0;
            uint B = b0;
            uint C = c0;
            uint D = d0;

            for (int j = 0; j < 64; j++)
            {
                uint F = 0;
                int g = 0;

                if (j < 16)
                {
                    F = (B & C) | (~B & D);
                    g = j;
                }
                else if (j < 32)
                {
                    F = (D & B) | (~D & C);
                    g = (5 * j + 1) % 16;
                }
                else if (j < 48)
                {
                    F = B ^ C ^ D;
                    g = (3 * j + 5) % 16;
                }
                else
                {
                    F = C ^ (B | ~D);
                    g = (7 * j) % 16;
                }

                uint temp = D;
                D = C;
                C = B;
                B = B + RotateLeft(A + F + k[j] + chunk[g], s[j]);
                A = temp;
            }

            a0 += A;
            b0 += B;
            c0 += C;
            d0 += D;
        }

        return ByteArrayToHexString(BitConverter.GetBytes(a0)) +
               ByteArrayToHexString(BitConverter.GetBytes(b0)) +
               ByteArrayToHexString(BitConverter.GetBytes(c0)) +
               ByteArrayToHexString(BitConverter.GetBytes(d0));
    }

    public static uint RotateLeft(uint x, int n)
    {
        return (x << n) | (x >> (32 - n));
    }

    public static string ByteArrayToHexString(byte[] bytes)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var b in bytes)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }

    static void Main(string[] args)
    {
        Console.WriteLine("Enter the text to hash:");
        string input = Console.ReadLine();

        string md5Hash = ComputeMD5(input);
        Console.WriteLine($"MD5 Hash: {md5Hash}");
    }
}
