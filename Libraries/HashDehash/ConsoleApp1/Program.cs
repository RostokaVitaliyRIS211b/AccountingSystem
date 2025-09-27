string original = "Секретное сообщение";
string encrypted = StringCipher.Encrypt(original);
string decrypted = StringCipher.Decrypt(encrypted);

Console.WriteLine($"Исходная: {original}");
Console.WriteLine($"Зашифровано: {encrypted}");
Console.WriteLine($"Расшифровано: {decrypted}");
// Вывод:
// Исходная: Секретное сообщение
// Зашифровано: 7gH9kL2mN4pQ6sT8vX0zB3cE5fG7iJ9lM=
// Расшифровано: Секретное сообщение