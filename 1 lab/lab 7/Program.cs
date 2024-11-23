const int Nb = 4;  // number of columns
 const int Nk = 4;  // number of 32-bit words in key
 const int Nr = 10; // number of rounds


// Тестовий приклад
byte[] plaintext = new byte[] {
            0x00, 0x11, 0x22, 0x33,
            0x44, 0x55, 0x66, 0x77,
            0x88, 0x99, 0xaa, 0xbb,
            0xcc, 0xdd, 0xee, 0xff
        };

byte[] key = new byte[] {
            0x00, 0x01, 0x02, 0x03,
            0x04, 0x05, 0x06, 0x07,
            0x08, 0x09, 0x0a, 0x0b,
            0x0c, 0x0d, 0x0e, 0x0f
        };

Console.WriteLine("Plaintext:");
PrintState(plaintext);
Console.WriteLine("\nKey:");
PrintState(key);

byte[] encrypted = Encrypt(plaintext, key);
Console.WriteLine("\nEncrypted:");
PrintState(encrypted);

byte[] decrypted = Decrypt(encrypted, key);
Console.WriteLine("\nDecrypted:");
PrintState(decrypted);


// Функція шифрування
static byte[] Encrypt(byte[] input, byte[] key)
{
    byte[,] state = new byte[4, Nb];
    ToState(input, state);

    byte[][] roundKeys = KeyExpansion(key);

    // Initial round
    AddRoundKey(state, roundKeys, 0);

    // Main rounds
    for (int round = 1; round < Nr; round++)
    {
        SubBytes(state);
        ShiftRows(state);
        MixColumns(state);
        AddRoundKey(state, roundKeys, round);
    }

    // Final round
    SubBytes(state);
    ShiftRows(state);
    AddRoundKey(state, roundKeys, Nr);

    return ToBytes(state);
}

// Функція дешифрування
static byte[] Decrypt(byte[] input, byte[] key)
{
    byte[,] state = new byte[4, Nb];
    ToState(input, state);

    byte[][] roundKeys = KeyExpansion(key);

    // Initial round
    AddRoundKey(state, roundKeys, Nr);
    InvShiftRows(state);
    InvSubBytes(state);

    // Main rounds
    for (int round = Nr - 1; round > 0; round--)
    {
        AddRoundKey(state, roundKeys, round);
        InvMixColumns(state);
        InvShiftRows(state);
        InvSubBytes(state);
    }

    // Final round
    AddRoundKey(state, roundKeys, 0);

    return ToBytes(state);
}

// SubBytes трансформація
static void SubBytes(byte[,] state)
{
    for (int row = 0; row < 4; row++)
        for (int col = 0; col < Nb; col++)
            state[row, col] = SubByte(state[row, col]);
}

// Математична реалізація S-Box
static byte SubByte(byte b)
{
    if (b == 0) return 0x63;

    // Знаходження мультиплікативного оберненого в GF(2^8)
    byte inv = FindMultiplicativeInverse(b);

    // Афінне перетворення
    byte result = 0;
    byte c = 0x63; // константа афінного перетворення

    for (int i = 0; i < 8; i++)
    {
        result |= Convert.ToByte((
            ((inv >> i) & 0x01) ^
            ((inv >> ((i + 4) % 8)) & 0x01) ^
            ((inv >> ((i + 5) % 8)) & 0x01) ^
            ((inv >> ((i + 6) % 8)) & 0x01) ^
            ((inv >> ((i + 7) % 8)) & 0x01) ^
            ((c >> i) & 0x01)
        ) << i);
    }

    return result;
}

// Знаходження мультиплікативного оберненого в GF(2^8)
static byte FindMultiplicativeInverse(byte b)
{
    if (b == 0) return 0;

    for (int i = 1; i < 256; i++)
    {
        if (MultiplyGF(b, (byte)i) == 1)
            return (byte)i;
    }
    return 0;
}

// Множення в GF(2^8)
static byte MultiplyGF(byte a, byte b)
{
    byte p = 0;
    for (int i = 0; i < 8; i++)
    {
        if ((b & 1) != 0)
            p ^= a;
        bool carry = (a & 0x80) != 0;
        a <<= 1;
        if (carry)
            a ^= 0x1B; // x^8 + x^4 + x^3 + x + 1
        b >>= 1;
    }
    return p;
}

// InvSubBytes трансформація
static void InvSubBytes(byte[,] state)
{
    for (int row = 0; row < 4; row++)
        for (int col = 0; col < Nb; col++)
            state[row, col] = InvSubByte(state[row, col]);
}

// Обернена SubByte трансформація
static byte InvSubByte(byte b)
{
    // Обернене афінне перетворення
    byte result = 0;
    byte c = 0x05; // константа оберненого афінного перетворення

    for (int i = 0; i < 8; i++)
    {
        result |= Convert.ToByte((
            ((b >> ((i + 2) % 8)) & 0x01) ^
            ((b >> ((i + 5) % 8)) & 0x01) ^
            ((b >> ((i + 7) % 8)) & 0x01) ^
            ((c >> i) & 0x01)
        ) << i);
    }

    // Знаходження мультиплікативного оберненого
    return FindMultiplicativeInverse(result);
}

// ShiftRows трансформація
static void ShiftRows(byte[,] state)
{
    for (int row = 1; row < 4; row++)
    {
        byte[] temp = new byte[4];
        for (int col = 0; col < 4; col++)
            temp[col] = state[row, col];

        for (int col = 0; col < 4; col++)
            state[row, col] = temp[(col + row) % 4];
    }
}

// Обернена ShiftRows трансформація
static void InvShiftRows(byte[,] state)
{
    for (int row = 1; row < 4; row++)
    {
        byte[] temp = new byte[4];
        for (int col = 0; col < 4; col++)
            temp[col] = state[row, col];

        for (int col = 0; col < 4; col++)
            state[row, col] = temp[(col + 4 - row) % 4];
    }
}

// MixColumns трансформація
static void MixColumns(byte[,] state)
{
    for (int col = 0; col < Nb; col++)
    {
        byte[] temp = new byte[4];
        for (int row = 0; row < 4; row++)
            temp[row] = state[row, col];

        state[0, col] = (byte)(MultiplyGF(0x02, temp[0]) ^ MultiplyGF(0x03, temp[1]) ^ temp[2] ^ temp[3]);
        state[1, col] = (byte)(temp[0] ^ MultiplyGF(0x02, temp[1]) ^ MultiplyGF(0x03, temp[2]) ^ temp[3]);
        state[2, col] = (byte)(temp[0] ^ temp[1] ^ MultiplyGF(0x02, temp[2]) ^ MultiplyGF(0x03, temp[3]));
        state[3, col] = (byte)(MultiplyGF(0x03, temp[0]) ^ temp[1] ^ temp[2] ^ MultiplyGF(0x02, temp[3]));
    }
}

// Обернена MixColumns трансформація
static void InvMixColumns(byte[,] state)
{
    for (int col = 0; col < Nb; col++)
    {
        byte[] temp = new byte[4];
        for (int row = 0; row < 4; row++)
            temp[row] = state[row, col];

        state[0, col] = (byte)(MultiplyGF(0x0e, temp[0]) ^ MultiplyGF(0x0b, temp[1]) ^
                              MultiplyGF(0x0d, temp[2]) ^ MultiplyGF(0x09, temp[3]));
        state[1, col] = (byte)(MultiplyGF(0x09, temp[0]) ^ MultiplyGF(0x0e, temp[1]) ^
                              MultiplyGF(0x0b, temp[2]) ^ MultiplyGF(0x0d, temp[3]));
        state[2, col] = (byte)(MultiplyGF(0x0d, temp[0]) ^ MultiplyGF(0x09, temp[1]) ^
                              MultiplyGF(0x0e, temp[2]) ^ MultiplyGF(0x0b, temp[3]));
        state[3, col] = (byte)(MultiplyGF(0x0b, temp[0]) ^ MultiplyGF(0x0d, temp[1]) ^
                              MultiplyGF(0x09, temp[2]) ^ MultiplyGF(0x0e, temp[3]));
    }
}

// AddRoundKey трансформація
static void AddRoundKey(byte[,] state, byte[][] roundKeys, int round)
{
    for (int col = 0; col < Nb; col++)
    {
        for (int row = 0; row < 4; row++)
        {
            state[row, col] ^= roundKeys[round * Nb + col][row];
        }
    }
}

// Розгортання ключа (Key Expansion)
static byte[][] KeyExpansion(byte[] key)
{
    byte[][] expandedKey = new byte[Nb * (Nr + 1)][];
    for (int i = 0; i < expandedKey.Length; i++)
        expandedKey[i] = new byte[4];

    // Копіювання початкового ключа
    for (int i = 0; i < Nk; i++)
    {
        expandedKey[i][0] = key[4 * i];
        expandedKey[i][1] = key[4 * i + 1];
        expandedKey[i][2] = key[4 * i + 2];
        expandedKey[i][3] = key[4 * i + 3];
    }

    // Генерація решти слів
    for (int i = Nk; i < Nb * (Nr + 1); i++)
    {
        byte[] temp = new byte[4];
        Array.Copy(expandedKey[i - 1], temp, 4);

        if (i % Nk == 0)
        {
            RotWord(temp);
            SubWord(temp);
            temp[0] ^= Rcon(i / Nk);
        }

        expandedKey[i] = new byte[4];
        for (int j = 0; j < 4; j++)
            expandedKey[i][j] = (byte)(expandedKey[i - Nk][j] ^ temp[j]);
    }

    return expandedKey;
}

// Допоміжні функції для розгортання ключа
static void RotWord(byte[] word)
{
    byte temp = word[0];
    word[0] = word[1];
    word[1] = word[2];
    word[2] = word[3];
    word[3] = temp;
}

static void SubWord(byte[] word)
{
    for (int i = 0; i < 4; i++)
        word[i] = SubByte(word[i]);
}

static byte Rcon(int i)
{
    byte rcon = 1;
    for (int j = 1; j < i; j++)
    {
        rcon = (byte)MultiplyGF(rcon, 2);
    }
    return rcon;
}

// Конвертація між форматами
static void ToState(byte[] input, byte[,] state)
{
    for (int row = 0; row < 4; row++)
        for (int col = 0; col < Nb; col++)
            state[row, col] = input[row + 4 * col];
}

static byte[] ToBytes(byte[,] state)
{
    byte[] output = new byte[16];
    for (int row = 0; row < 4; row++)
        for (int col = 0; col < Nb; col++)
            output[row + 4 * col] = state[row, col];
    return output;
}

// Допоміжна функція для виведення стану
static void PrintState(byte[] state)
{
    for (int i = 0; i < state.Length; i++)
    {
        Console.Write($"{state[i]:X2} ");
        if ((i + 1) % 4 == 0) Console.WriteLine();
    }
}
