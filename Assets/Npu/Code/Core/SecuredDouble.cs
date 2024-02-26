using System;
using Random = UnityEngine.Random;
using UnityEngine;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace Npu.Core
{


    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public partial struct SecuredDouble : IFormattable, IComparable<SecuredDouble>, IEquatable<SecuredDouble>,
        IComparable
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct Union
        {
            [FieldOffset(0)] public double d;

            [FieldOffset(0)] public long l;
        }

        private static long _staticKey = 0x12faba9e7d653e8f;
        private static Func<double, Unit> _unitFinder;

        static SecuredDouble()
        {
            _unitFinder = Unit0.Find;
        }

        [SerializeField, JsonProperty] private long key;
        [SerializeField, JsonProperty] private double masked;

        public SecuredDouble(double value = 0) : this(_staticKey, value)
        {
        }

        public SecuredDouble(long key, double value = 0)
        {
            this.key = key;
            Union u = default;
            u.d = value;
            u.l ^= key;
            masked = u.d;
        }

        public double Value
        {
            get
            {
                Union u = default;
                u.d = masked;
                u.l ^= key;
                return u.d;
            }
        }

        // Implicit convert double to SecuredDouble
        public static implicit operator SecuredDouble(double value) => new SecuredDouble(value);

        // Implicit convert SecuredDouble to double
        public static implicit operator double(SecuredDouble value) => value.Value;

        public static SecuredDouble operator +(SecuredDouble a, SecuredDouble b) =>
            new SecuredDouble(a.Value + b.Value);

        public static SecuredDouble operator -(SecuredDouble a, SecuredDouble b) =>
            new SecuredDouble(a.Value - b.Value);

        public static SecuredDouble operator *(SecuredDouble a, SecuredDouble b) =>
            new SecuredDouble(a.Value * b.Value);

        public static SecuredDouble operator /(SecuredDouble a, SecuredDouble b) =>
            new SecuredDouble(a.Value / b.Value);

        public static bool operator >(SecuredDouble a, SecuredDouble b) => a.Value > b.Value;
        public static bool operator >=(SecuredDouble a, SecuredDouble b) => a.Value >= b.Value;
        public static bool operator <(SecuredDouble a, SecuredDouble b) => a.Value < b.Value;
        public static bool operator <=(SecuredDouble a, SecuredDouble b) => a.Value <= b.Value;

        public SecuredDouble Floor => new SecuredDouble(Math.Floor(Value));
        public SecuredDouble Ceiling => new SecuredDouble(Math.Ceiling(Value));
        public SecuredDouble Round => new SecuredDouble(Math.Round(Value));

        public float AsFloat() => (float) Value;
        public bool AsBool(float eps = 0.3f) => Value > eps;
        public int AsInt() => (int) Value;
        public long AsLong() => (long) Value;
        public bool True => AsBool();

        public SecuredDouble Pow(double p) => new SecuredDouble(Math.Pow(Value, p));
        public static SecuredDouble Max(SecuredDouble a, SecuredDouble b) => a > b ? a : b;
        public static SecuredDouble Min(SecuredDouble a, SecuredDouble b) => a > b ? b : a;

        public static SecuredDouble Clamp(SecuredDouble value, SecuredDouble min, SecuredDouble max) =>
            value < min ? min : (value > max ? max : value);

        public int CompareTo(SecuredDouble other) => Value.CompareTo(other.Value);
        public int CompareTo(object obj) => Value.CompareTo(obj);

        public bool Equals(SecuredDouble other) => Value.Equals(other.Value);

        public override bool Equals(object other)
        {
            if (other == null || GetType() != other.GetType()) return false;
            return Value.Equals(((SecuredDouble) other).Value);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => ToString(FindUnit(Value), "0.#");
        public string ToString(string format) => ToString(FindUnit(Value), format);
        public string ToString(IFormatProvider provider) => Value.ToString(provider);
        public string ToString(string format, IFormatProvider provider) => ToString(FindUnit(Value), format, provider);

        private string ToString(Unit unit, string format = "0.##")
        {
            if (double.IsInfinity(Value) || double.IsNaN(Value))
            {
                return "Infinity or NaN";
            }

            return (Value / Math.Pow(10, unit.exponent)).ToString(format) + unit.name;
        }

        private string ToString(Unit unit, string format, IFormatProvider provider)
        {
            if (double.IsInfinity(Value) || double.IsNaN(Value))
            {
                return "Infinity or NaN";
            }

            return (Value / Math.Pow(10, unit.exponent)).ToString(format, provider) + unit.name;
        }

        public void ApplyNewKey(long key)
        {
            var value = this.Value;
            this.key = key;
            Union u = default;
            u.d = value;
            u.l ^= key;
            masked = u.d;
        }

#if UNITY_EDITOR
        public double Masked => masked;

        public static double Encrypt(long key, double value)
        {
            Union u = default;
            u.d = value;
            u.l ^= key;
            return u.d;
        }

        public static double Pack(long key, double masked)
        {
            Union u = default;
            u.d = masked;
            u.l ^= key;
            return u.d;
        }
#endif

        public static void SetUnit(int u)
        {
            if (u == 0)
            {
                _unitFinder = Unit0.Find;
            }
            else if (u == 1)
            {
                _unitFinder = Unit1.Find;
            }
            else
            {
                _unitFinder = Unit2.Find;
            }
        }

        public static void SetNewKey(long key)
        {
            if (key != 0)
            {
                _staticKey = key;
            }
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