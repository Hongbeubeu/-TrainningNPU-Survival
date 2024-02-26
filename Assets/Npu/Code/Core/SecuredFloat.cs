using System;
using Random = UnityEngine.Random;
using UnityEngine;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace Npu.Core
{

    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public struct SecuredFloat : IFormattable, IComparable, IComparable<SecuredFloat>, IEquatable<SecuredFloat>
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct Union
        {
            [FieldOffset(0)] public float d;

            [FieldOffset(0)] public long l;
        }

        private static long staticKey = 0x12faba9e7d653e8f;

        [SerializeField, JsonProperty] private long key;
        [SerializeField, JsonProperty] private float masked;

        public SecuredFloat(float value) : this(staticKey, value)
        {
        }

        public SecuredFloat(long key, float value)
        {
            this.key = key;
            Union u = default;
            u.d = value;
            u.l ^= key;
            masked = u.d;
        }

        public float Value
        {
            get
            {
                Union u = default;
                u.d = masked;
                u.l ^= key;
                return u.d;
            }
        }

        // Implicit convert double to SecuredFloat
        public static implicit operator SecuredFloat(float value) => new SecuredFloat(value);

        // Implicit convert SecuredFloat to double
        public static implicit operator float(SecuredFloat value) => value.Value;

        public static SecuredFloat operator +(SecuredFloat a, SecuredFloat b) => new SecuredFloat(a.Value + b.Value);
        public static SecuredFloat operator -(SecuredFloat a, SecuredFloat b) => new SecuredFloat(a.Value - b.Value);
        public static SecuredFloat operator *(SecuredFloat a, SecuredFloat b) => new SecuredFloat(a.Value * b.Value);
        public static SecuredFloat operator /(SecuredFloat a, SecuredFloat b) => new SecuredFloat(a.Value / b.Value);

        public static bool operator >(SecuredFloat a, SecuredFloat b) => a.Value > b.Value;
        public static bool operator >=(SecuredFloat a, SecuredFloat b) => a.Value >= b.Value;
        public static bool operator <(SecuredFloat a, SecuredFloat b) => a.Value < b.Value;
        public static bool operator <=(SecuredFloat a, SecuredFloat b) => a.Value <= b.Value;

        public SecuredFloat Floor => new SecuredFloat((float) Math.Floor(Value));
        public SecuredFloat Ceiling => new SecuredFloat((float) Math.Ceiling(Value));
        public SecuredFloat Round => new SecuredFloat((float) Math.Round(Value));

        public SecuredFloat Pow(float p) => new SecuredFloat(Mathf.Pow(Value, p));
        public static SecuredFloat Max(SecuredFloat a, SecuredFloat b) => a > b ? a : b;
        public static SecuredFloat Min(SecuredFloat a, SecuredFloat b) => a > b ? b : a;

        public static SecuredFloat Clamp(SecuredFloat value, SecuredFloat min, SecuredFloat max) =>
            value < min ? min : (value > max ? max : value);

        public int CompareTo(SecuredFloat other) => Value.CompareTo(other.Value);
        public int CompareTo(object obj) => Value.CompareTo(obj);

        public bool Equals(SecuredFloat other) => Value.Equals(other.Value);

        public override bool Equals(object other)
        {
            if (other == null || GetType() != other.GetType()) return false;
            return Value.Equals(((SecuredFloat) other).Value);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();
        public string ToString(string format) => Value.ToString(format);
        public string ToString(IFormatProvider provider) => Value.ToString(provider);
        public string ToString(string format, IFormatProvider provider) => Value.ToString(format, provider);

        public static void SetNewKey(long key)
        {
            if (key != 0)
            {
                staticKey = key;
            }
        }

#if UNITY_EDITOR
        public float Masked => masked;
        public static float Encrypt(long key, float value)
        {
            Union u = default;
            u.d = value;
            u.l ^= key;
            return u.d;
        }
#endif

        public void ApplyNewKey(long key)
        {
            var value = this.Value;
            this.key = key;
            var u = default(Union);
            u.d = value;
            u.l ^= key;
            masked = u.d;
        }

        public static long RandomKey()
        {
            long key = 0;
            for (var i = 0; i < sizeof(long); i++)
            {
                var b = (long) (Random.value * byte.MaxValue);
                key += b << (i * 8);
            }

            return key;
        }
    }

}