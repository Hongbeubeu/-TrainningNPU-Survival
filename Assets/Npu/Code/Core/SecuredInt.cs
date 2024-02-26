using System;
using Random = UnityEngine.Random;
using UnityEngine;
using Newtonsoft.Json;

namespace Npu.Core
{

    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public struct SecuredInt : IEquatable<SecuredInt>, IComparable, IComparable<SecuredLong>, IFormattable
    {
        private static int staticKey = 43098;

        [SerializeField, JsonProperty] private int key;
        [SerializeField, JsonProperty] private int masked;

        public SecuredInt(int value = 0) : this(staticKey, value)
        {
        }

        public SecuredInt(int key, int value)
        {
            this.key = key;
            this.masked = value ^ key;
        }

        public SecuredInt(string serializedString)
        {
            try
            {
                var cs = serializedString.Split('@');
                this.key = int.Parse(cs[0]);
                this.masked = int.Parse(cs[1]);
            }
            catch (Exception ex)
            {
                this.key = staticKey;
                this.masked = 0 ^ this.key;
            }
        }

        public int Value => masked ^ key;

        // Implicit convert int to SecuredInt
        public static implicit operator SecuredInt(int value) => new SecuredInt(staticKey, value);

        // Implicit convert SecuredInt to int
        public static implicit operator int(SecuredInt value) => value.Value;

        public static SecuredInt operator ++(SecuredInt number) => new SecuredInt(number.Value + 1);
        public static SecuredInt operator --(SecuredInt number) => new SecuredInt(number.Value - 1);
        public static SecuredInt operator +(SecuredInt a, SecuredInt b) => new SecuredInt(a.Value + b.Value);
        public static SecuredInt operator -(SecuredInt a, SecuredInt b) => new SecuredInt(a.Value - b.Value);
        public static SecuredInt operator *(SecuredInt a, SecuredInt b) => new SecuredInt(a.Value * b.Value);
        public static SecuredInt operator /(SecuredInt a, SecuredInt b) => new SecuredInt(a.Value / b.Value);

        public static bool operator >(SecuredInt a, SecuredInt b) => a.Value > b.Value;
        public static bool operator >=(SecuredInt a, SecuredInt b) => a.Value >= b.Value;
        public static bool operator <(SecuredInt a, SecuredInt b) => a.Value < b.Value;
        public static bool operator <=(SecuredInt a, SecuredInt b) => a.Value <= b.Value;
        public static bool operator ==(SecuredInt a, SecuredInt b) => a.Value == b.Value;
        public static bool operator !=(SecuredInt a, SecuredInt b) => a.Value != b.Value;

        public override bool Equals(object other)
        {
            if (other == null || GetType() != other.GetType()) return false;
            return Value == ((SecuredInt) other).Value;
        }

        public bool Equals(SecuredInt other) => Value == other.Value;

        public int CompareTo(SecuredLong other) => Value.CompareTo(other.Value);
        public int CompareTo(object other) => Value.CompareTo(other);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();
        public string ToString(string format) => Value.ToString(format);
        public string ToString(IFormatProvider provider) => Value.ToString(provider);
        public string ToString(string format, IFormatProvider provider) => Value.ToString(format, provider);

        public string ToSerializedString() => key.ToString() + "@" + masked.ToString();

        public static SecuredInt Max(SecuredInt a, SecuredInt b) => a > b ? a : b;
        public static SecuredInt Min(SecuredInt a, SecuredInt b) => a > b ? b : a;

        public static SecuredInt Clamp(SecuredInt value, SecuredInt min, SecuredInt max) =>
            value < min ? min : (value > max ? max : value);

        public void ApplyNewKey(int key)
        {
            var value = Value;
            this.key = key;
            this.masked = value ^ key;
        }

#if UNITY_EDITOR
        public int Masked => masked;
        public int Key => key;
        public static int Encrypt(int key, int value) => value ^ key;

#endif

        public static void SetNewKey(int key)
        {
            if (key != 0)
            {
                staticKey = key;
            }
        }

        public static int RandomKey()
        {
            var key = 0;
            for (var i = 0; i < sizeof(int); i++)
            {
                var b = (int) (Random.value * byte.MaxValue);
                key += b << (i * 8);
            }

            return key;
        }
    }

}