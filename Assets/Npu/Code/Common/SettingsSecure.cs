using System.Security.Cryptography;
using System.Text;
using System;
using Npu.Helper;
using UnityEngine;

namespace Npu
{
    public static partial class Settings
    {
        private static string Key = "9v{qsm<)T@*HVY5?";
        public static AbstractCryptor Cryptor = new SimpleCryptor(Key);

        public static bool SecuredPrefsHasKey(string key)
        {
            return PlayerPrefs.HasKey(Cryptor.Encrypt(key));
        }
        
        public static void SecuredPrefsDeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(Cryptor.Encrypt(key));
        }
        
        public static int SecuredPrefsGetInt(string key, int defaultValue=0)
        {
            key = Cryptor.Encrypt(key);
            var value = PlayerPrefs.GetString(key);
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return Cryptor.DecryptInt(value);
        }
        
        public static void SecuredPrefsSetInt(string key, int value)
        {
            PlayerPrefs.SetString(Cryptor.Encrypt(key), Cryptor.Encrypt(value));
        }
        
        public static long SecuredPrefsGetLong(string key, long defaultValue=0)
        {
            key = Cryptor.Encrypt(key);
            var value = PlayerPrefs.GetString(key);
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return Cryptor.DecryptLong(value);
        }
        
        public static void SecuredPrefsSetLong(string key, long value)
        {
            PlayerPrefs.SetString(Cryptor.Encrypt(key), Cryptor.Encrypt(value));
        }
        
        public static float SecuredPrefsGetFloat(string key, float defaultValue=0)
        {
            key = Cryptor.Encrypt(key);
            var value = PlayerPrefs.GetString(key);
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return Cryptor.DecryptFloat(value);
        }
        
        public static void SecuredPrefsSetFloat(string key, float value)
        {
            PlayerPrefs.SetString(Cryptor.Encrypt(key), Cryptor.Encrypt(value));
        }
        
        public static double SecuredPrefsGetDouble(string key, double defaultValue=0)
        {
            key = Cryptor.Encrypt(key);
            var value = PlayerPrefs.GetString(key);
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return Cryptor.DecryptDouble(value);
        }
        
        public static void SecuredPrefsSetDouble(string key, double value)
        {
            PlayerPrefs.SetString(Cryptor.Encrypt(key), Cryptor.Encrypt(value));
        }
        
        public static bool SecuredPrefsGetBool(string key, bool defaultValue=false)
        {
            key = Cryptor.Encrypt(key);
            var value = PlayerPrefs.GetString(key);
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return Cryptor.DecryptBool(value);
        }
        
        public static void SecuredPrefsSetBool(string key, bool value)
        {
            PlayerPrefs.SetString(Cryptor.Encrypt(key), Cryptor.Encrypt(value));
        }
        
        public static string SecuredPrefsGetString(string key, string defaultValue=null)
        {
            key = Cryptor.Encrypt(key);
            var value = PlayerPrefs.GetString(key);
            return string.IsNullOrEmpty(value) ? defaultValue : Cryptor.Decrypt(value);
        }
        
        public static void SecuredPrefsSetString(string key, string value)
        {
            PlayerPrefs.SetString(Cryptor.Encrypt(key), Cryptor.Encrypt(value));
        }

        

#region Tests
        public static void TestSecuredPrefs()
        {
            var tests = 10000;

            var key = "a_bool";
            SecuredPrefsSetBool(key, true);
            if (SecuredPrefsGetBool(key) != true)
            {
                Debug.LogErrorFormat(key);
            }

            key = "a_int";
            for (var k = 0; k < tests; k++)
            {

                var i = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                SecuredPrefsSetInt(key, i);
                var i_actual = SecuredPrefsGetInt(key);
                if (i_actual != i)
                {
                    Debug.LogErrorFormat("{0} {1} vs {2}", key, i, i_actual);
                }
            }
            
            key = "a_float";
            for (var k = 0; k < tests; k++)
            {
                var i = UnityEngine.Random.Range(float.MinValue, float.MaxValue);
                SecuredPrefsSetFloat(key, i);
                var i_actual = SecuredPrefsGetFloat(key);
                if (i_actual != i)
                {
                    Debug.LogErrorFormat("{0} {1} vs {2}", key, i, i_actual);
                }
            }
            
            key = "a_long";
            for (var k = 0; k < tests; k++)
            {
                var i = (long)((UnityEngine.Random.value - 0.5f) * long.MaxValue * 2);
                SecuredPrefsSetLong(key, i);
                var i_actual = SecuredPrefsGetLong(key);
                if (i_actual != i)
                {
                    Debug.LogErrorFormat("{0} {1} vs {2}", key, i, i_actual);
                }
            }
            
            key = "a_double";
            for (var k = 0; k < tests; k++)
            {
                var i = ((UnityEngine.Random.value - 0.5) * double.MaxValue) * 2;
                SecuredPrefsSetDouble(key, i);
                var i_actual = SecuredPrefsGetDouble(key);
                if (i_actual != i)
                {
                    Debug.LogErrorFormat("{0} {1} vs {2}", key, i, i_actual);
                }
            }

            key = "a_string";
            for (var k = 0; k < tests; k++)
            {
                var i = (((UnityEngine.Random.value - 0.5) * 1000000) * 2).ToString("0.00000000");
                SecuredPrefsSetString(key, i);
                var i_actual = SecuredPrefsGetString(key);
                if (i_actual != i)
                {
                    Debug.LogErrorFormat("{0} {1} vs {2}", key, i, i_actual);
                }
            }
            
            PlayerPrefs.Save();
            Debug.Log("Finish Testing");
        }

        public static void PerformanceTests()
        {
            var Key = "a6f34dvnr2pg4yvn";
            var standard = new StandardCryptor(Key);
            var simple = new SimpleCryptor(Key);

            var steps = 10000;
            var dataLength = 100;

            byte[] Random()
            {
                var bytes = new byte[dataLength];
                for (var i = 0; i < dataLength; i++)
                {
                    bytes[i] = (byte)((UnityEngine.Random.value - 0.5f) * byte.MaxValue * 2);
                }

                return bytes;
            }

            long Stress(AbstractCryptor cryptor, byte[] bytes)
            {
                var ts = TimeUtils.CurrentTicks;
                for (var i = 0; i < steps; i++)
                {
                    var crypted = cryptor.Encrypt(bytes);
                }

                return TimeUtils.CurrentTicks - ts;
            }

            void PrintResult(string tag, long ticks)
            {
                Debug.LogFormat("{0}: {1}/{2}", tag, ticks, TimeUtils.TicksToSeconds(ticks));
            }

            var data = Random();
            var t1 = Stress(standard, data);
            var t2 = Stress(simple, data);

            PrintResult("Standard", t1);
            PrintResult("Simple", t2);
        }
#endregion
    }
}
