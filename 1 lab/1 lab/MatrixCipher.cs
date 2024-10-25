using System.Text;
// 1. Матричний шифр
class MatrixCipher
{
    public static string Encrypt(string message, string rowKey, string colKey)
    {
        message = message.Replace(" ", "").ToUpper();
        int rows = rowKey.Length;
        int cols = colKey.Length;
        
        // Доповнюємо повідомлення, якщо потрібно
        while (message.Length % (rows * cols) != 0)
            message += "Х";

        var result = new StringBuilder();
        
        for (int blockStart = 0; blockStart < message.Length; blockStart += rows * cols)
        {
            // Заповнюємо матрицю
            char[,] matrix = new char[rows, cols];
            int messageIndex = blockStart;
            
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = message[messageIndex++];

            // Переставляємо рядки
            char[,] rowPermuted = new char[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                int newRow = int.Parse(rowKey[i].ToString()) - 1;
                for (int j = 0; j < cols; j++)
                    rowPermuted[i, j] = matrix[newRow, j];
            }

            // Переставляємо стовпці
            char[,] colPermuted = new char[rows, cols];
            for (int j = 0; j < cols; j++)
            {
                int newCol = int.Parse(colKey[j].ToString()) - 1;
                for (int i = 0; i < rows; i++)
                    colPermuted[i, j] = rowPermuted[i, newCol];
            }

            // Записуємо результат
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result.Append(colPermuted[i, j]);
        }

        return result.ToString();
    }

    public static string Decrypt(string encrypted, string rowKey, string colKey)
    {
        int rows = rowKey.Length;
        int cols = colKey.Length;
        var result = new StringBuilder();

        for (int blockStart = 0; blockStart < encrypted.Length; blockStart += rows * cols)
        {
            char[,] matrix = new char[rows, cols];
            int encryptedIndex = blockStart;

            // Заповнюємо матрицю
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = encrypted[encryptedIndex++];

            // Відновлюємо стовпці
            char[,] colRestored = new char[rows, cols];
            for (int j = 0; j < cols; j++)
            {
                int originalCol = int.Parse(colKey[j].ToString()) - 1;
                for (int i = 0; i < rows; i++)
                    colRestored[i, originalCol] = matrix[i, j];
            }

            // Відновлюємо рядки
            char[,] rowRestored = new char[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                int originalRow = int.Parse(rowKey[i].ToString()) - 1;
                for (int j = 0; j < cols; j++)
                    rowRestored[originalRow, j] = colRestored[i, j];
            }

            // Записуємо результат
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result.Append(rowRestored[i, j]);
        }

        return result.ToString();
    }
}
