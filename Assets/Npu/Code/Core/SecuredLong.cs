using System;
using UnityEngine;
using Newtonsoft.Json;

namespace Npu.Core
{

    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public struct SecuredLong : IEquatable<SecuredLong>, IComparable<SecuredLong>, IComparable, IFormattable
    {
        private static long staticKey = 246357;

        [SerializeField, JsonProperty] private long key;
        [SerializeField, JsonProperty] private long masked;

        public SecuredLong(long value = 0) : this(staticKey, value)
        {
        }

        public SecuredLong(long key, long value)
        {
            this.key = key;
            this.masked = value ^ key;
        }

        public long Value => masked ^ key;

        // Implicit convert long to SecuredLong
        public static implicit operator SecuredLong(long value) => new SecuredLong(value);

        // Implicit convert SecuredLong to long
        public static implicit operator long(SecuredLong value) => value.Value;

        // Implicit convert int to SecuredLong
        public static implicit operator SecuredLong(int value) => new SecuredLong(value);

        // Implicit convert SecuredInt to SecuredLong
        public static implicit operator SecuredLong(SecuredInt value) => new SecuredLong(value.Value);

        public static SecuredLong operator ++(SecuredLong number) => new SecuredLong(number.Value + 1);
        public static SecuredLong operator --(SecuredLong number) => new SecuredLong(number.Value - 1);
        public static SecuredLong operator +(SecuredLong a, SecuredLong b) => new SecuredLong(a.Value + b.Value);
        public static SecuredLong operator -(SecuredLong a, SecuredLong b) => new SecuredLong(a.Value - b.Value);
        public static SecuredLong operator *(SecuredLong a, SecuredLong b) => new SecuredLong(a.Value * b.Value);
        public static SecuredLong operator /(SecuredLong a, SecuredLong b) => new SecuredLong(a.Value / b.Value);

        public static bool operator >(SecuredLong a, SecuredLong b) => a.Value > b.Value;
        public static bool operator >=(SecuredLong a, SecuredLong b) => a.Value >= b.Value;
        public static bool operator <(SecuredLong a, SecuredLong b) => a.Value < b.Value;
        public static bool operator <=(SecuredLong a, SecuredLong b) => a.Value <= b.Value;
        public static bool operator ==(SecuredLong a, SecuredLong b) => a.Value == b.Value;
        public static bool operator !=(SecuredLong a, SecuredLong b) => a.Value != b.Value;

        public int CompareTo(SecuredLong other) => Value.CompareTo(other.Value);
        public int CompareTo(object other) => Value.CompareTo(other);

        public override bool Equals(object other)
        {
            if (other == null || GetType() != other.GetType()) return false;
            return Value == ((SecuredLong) other).Value;
        }

        public bool Equals(SecuredLong other) => this.Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();
        public string ToString(string format) => Value.ToString(format);
        public string ToString(IFormatProvider provider) => Value.ToString(provider);
        public string ToString(string format, IFormatProvider provider) => Value.ToString(format, provider);

        public static SecuredLong Max(SecuredLong a, SecuredLong b) => a > b ? a : b;
        public static SecuredLong Min(SecuredLong a, SecuredLong b) => a > b ? b : a;

        public static SecuredLong Clamp(SecuredLong value, SecuredLong min, SecuredLong max)
        {
            return value < min ? min : (value > max ? max : value);
        }

        public void ApplyNewKey(long key)
        {
            var value = Value;
            this.key = key;
            this.masked = value ^ key;
        }

#if UNITY_EDITOR
        public long Masked => masked;
        public long Key => key;
        public static long Encrypt(long key, long value) => value ^ key;
#endif

        public static void SetNewKey(long key)
        {
            if (key != 0)
            {
                staticKey = key;
            }
        }
    }

}