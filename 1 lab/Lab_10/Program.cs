using System;
using System.Text;

class Program
{
    // MD5 Hash Function Implementation
    public static string ComputeMD5(string input)
    {
        byte[] data = Encoding.UTF8.GetBytes(input);
        uint[] s = { 7, 12, 17, 22, 5, 9, 14, 20, 4, 11, 16, 23, 6, 10, 15, 21 };
        uint[] k = new uint[64];

        for (int i = 0; i < 64; i++)
        {
            k[i] = (uint)(Math.Abs(Math.Sin(i + 1)) * Math.Pow(2, 32));
        }

        uint a0 = 0x67452301;
        uint b0 = 0xefcdab89;
        uint c0 = 0x98badcfe;
        uint d0 = 0x10325476;

        // Padding
        int bitLen = data.Length * 8;
        int padding = (448 - bitLen % 512 + 512) % 512;
        int totalLen = bitLen + padding + 64;

        byte[] paddedData = new byte[totalLen / 8];
        Array.Copy(data, paddedData, data.Length);
        paddedData[data.Length] = 0x80;
        byte[] lengthBytes = BitConverter.GetBytes((ulong)bitLen);
        Array.Copy(lengthBytes, 0, paddedData, paddedData.Length - 8, 8);

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

                F = F + A + k[j] + chunk[g];
                A = D;
                D = C;
                C = B;
                B = B + RotateLeft(F, (int)s[j % 4]);
            }

            a0 += A;
            b0 += B;
            c0 += C;
            d0 += D;
        }

        return $"{a0:x8}{b0:x8}{c0:x8}{d0:x8}";
    }

    public static uint RotateLeft(uint x, int n)
    {
        return (x << n) | (x >> (32 - n));
    }

    static void Main(string[] args)
    {
        Console.WriteLine("Enter the text to hash:");
        string input = Console.ReadLine();

        string md5Hash = ComputeMD5(input);
        Console.WriteLine($"MD5 Hash: {md5Hash}");

        // Implementations for SHA-1, SHA-256, and SHA-3 are not shown here due to length constraints.
        // For full SHA-2 or SHA-3, you would follow a similar approach using the specifications for each algorithm.
    }
}
