using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Npu.Helper
{
    public class StandardCryptor : AbstractCryptor
        {
            private RijndaelManaged provider = null;
            public StandardCryptor(string key) : base(key)
            {
                provider = new RijndaelManaged();
                provider.Key = Key;
            }

            public override byte[] Encrypt(byte[] data)
            {
                using (var encryptor = provider.CreateEncryptor())
                {
                    return encryptor.TransformFinalBlock(data, 0, data.Length);
                }
            }

            public override byte[] Decrypt(byte[] data)
            {
                using (var decryptor = provider.CreateDecryptor())
                {
                    return decryptor.TransformFinalBlock(data, 0, data.Length);
                }
            }
        }

        public class SimpleCryptor : AbstractCryptor
        {
            public SimpleCryptor(string key) : base(key) {}

            public override byte[] Encrypt(byte[] data)
            {
                var encrypted = new byte[data.Length];
                var keyLength = Key.Length;
                for (var i = 0; i < encrypted.Length; i++)
                {
                    encrypted[i] = (byte) (data[i] ^ Key[i % keyLength]);
                }

                return encrypted;
            }

            public override byte[] Decrypt(byte[] data)
            {
                return Encrypt(data);
            }
        }

        public abstract class AbstractCryptor
        {
            protected byte[] Key;

            public AbstractCryptor(string key)
            {
                Key = Encoding.ASCII.GetBytes(key);
            }


            public abstract byte[] Encrypt(byte[] data);
            public abstract byte[] Decrypt(byte[] data);
            
            public string Encrypt(string data)
            {
                var bytes = Encrypt(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(bytes);
            }

            public string Encrypt(int data)
            {
                var bytes = Encrypt(BitConverter.GetBytes(data));
                return Convert.ToBase64String(bytes);
            }

            public string Encrypt(long data)
            {
                var bytes = Encrypt(BitConverter.GetBytes(data));
                return Convert.ToBase64String(bytes);
            }

            public string Encrypt(float data)
            {
                var bytes = Encrypt(BitConverter.GetBytes(data));
                return Convert.ToBase64String(bytes);
            }

            public string Encrypt(double data)
            {
                var bytes = Encrypt(BitConverter.GetBytes(data));
                return Convert.ToBase64String(bytes);
            }

            public string Encrypt(bool data)
            {
                var bytes = Encrypt(BitConverter.GetBytes(data));
                return Convert.ToBase64String(bytes);
            }
            
            public string Decrypt(string data)
            {
                var bytes = Decrypt(Convert.FromBase64String(data));
                return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            }

            public int DecryptInt(string data)
            {
                var bytes = Decrypt(Convert.FromBase64String(data));
                return BitConverter.ToInt32(bytes, 0);
            }

            public long DecryptLong(string data)
            {
                var bytes = Decrypt(Convert.FromBase64String(data));
                return BitConverter.ToInt64(bytes, 0);
            }

            public float DecryptFloat(string data)
            {
                var bytes = Decrypt(Convert.FromBase64String(data));
                return BitConverter.ToSingle(bytes, 0);
            }

            public double DecryptDouble(string data)
            {
                var bytes = Decrypt(Convert.FromBase64String(data));
                return BitConverter.ToDouble(bytes, 0);
            }

            public bool DecryptBool(string data)
            {
                var bytes = Decrypt(Convert.FromBase64String(data));
                return BitConverter.ToBoolean(bytes, 0);
            }
        }

    public static class CryptoUtils
    {
        static MD5 _MD5 = System.Security.Cryptography.MD5.Create();

        public static string Base64Encode(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        public static string Base64Decode(string text)
        {
            var bytes = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string Sha256Hash(string text)
        {
            using (var sha256Hash = SHA256.Create())
            {
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(text));

                var builder = new StringBuilder();
                for (var i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public static string MD5(Stream stream)
        {
            return Convert.ToBase64String(_MD5.ComputeHash(stream));
        }

        #region SSL

        public static string Sign(byte[] key, byte[] message)
        {
            var hm = new HMACSHA1(key);
            var stream = new MemoryStream(message);
            return Convert.ToBase64String(hm.ComputeHash(stream));
        }

        public static string Sign(byte[] message)
        {
            return Sign(SessionKeyBytes, message);
        }

        public static string Sign(string message)
        {
            return Sign(SessionKeyBytes, Encoding.UTF8.GetBytes(message));
        }

        public static string PublicKey { get; set; }
        public static string SessionKey { get; private set; }
        public static byte[] SessionKeyBytes { get; private set; }

        public static void CreateSessionKey()
        {
            SessionKey = GenerateRandomString(16);
            SessionKeyBytes = Encoding.UTF8.GetBytes(SessionKey);
            Logger._Log("CryptoUtils", SessionKey);
        }

        public static byte[] EncryptRSA(byte[] message)
        {
            using (var RSA = new RSACryptoServiceProvider())
            {
                RSA.FromXmlString(PublicKey);
                var encryptedData = RSA.Encrypt(message, true);
                return encryptedData;
            }
        }

        public static string GenerateRandomString(int length)
        {
            if (length <= 0)
            {
                throw new Exception("Expected nonce to have positive length");
            }

            const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._";
            var cryptographicallySecureRandomNumberGenerator = new RNGCryptoServiceProvider();
            var result = string.Empty;
            var remainingLength = length;

            var randomNumberHolder = new byte[1];
            while (remainingLength > 0)
            {
                var randomNumbers = new List<int>(16);
                for (var randomNumberCount = 0; randomNumberCount < 16; randomNumberCount++)
                {
                    cryptographicallySecureRandomNumberGenerator.GetBytes(randomNumberHolder);
                    randomNumbers.Add(randomNumberHolder[0]);
                }

                for (var randomNumberIndex = 0; randomNumberIndex < randomNumbers.Count; randomNumberIndex++)
                {
                    if (remainingLength == 0)
                    {
                        break;
                    }

                    var randomNumber = randomNumbers[randomNumberIndex];
                    if (randomNumber < charset.Length)
                    {
                        result += charset[randomNumber];
                        remainingLength--;
                    }
                }
            }

            return result;
        }

        #endregion
    }
}