using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Lockd.Core;

public class Encryption
{
	private static readonly byte[] Key = Encoding.UTF8.GetBytes("12345678901234567890123456789012"); // 32-byte key
    private static readonly byte[] IV = new byte[16]; // 16-byte IV

	public static string Encrypt(string plainText)
	{
		using Aes aes = Aes.Create();
		aes.Key = Key;
		aes.IV = IV;

		using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
		byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
		byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

		return Convert.ToBase64String(encryptedBytes);
	}

	public static string Decrypt(string encryptedText)
	{
		using Aes aes = Aes.Create();
		aes.Key = Key;
		aes.IV = IV;

		using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
		byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
		byte[] plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

		return Encoding.UTF8.GetString(plainBytes);
	}
}
