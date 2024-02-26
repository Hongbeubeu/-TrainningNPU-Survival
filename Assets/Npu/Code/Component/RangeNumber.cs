using UnityEngine;
using Npu.Utilities;

namespace Npu.Common
{

    [System.Serializable]
    public struct RangeDouble
    {
        public double a;
        public double b;

        public RangeDouble(RangeDouble rf) : this(rf.a, rf.b)
        {

        }

        public RangeDouble(double aa, double bb)
        {
            a = aa;
            b = bb;
        }

        public double Min => System.Math.Min(a, b);

        public double Max => System.Math.Max(a, b);

        public double Rnd => a + (b - a) * Random.value;

        public double Avg => (a + b) / 2f;

        public double Lerp(double t, bool unclamped = false)
        {
            return unclamped ? Numerics.LerpUnclamped(a, b, t) : Numerics.Lerp(a, b, t);
        }

        public double InverseLerp(double x, bool unclamped = false)
        {
            return unclamped ? Numerics.InverseLerpUnclamped(a, b, x) : Numerics.InverseLerp(a, b, x);
        }

        public double Clamp(double x)
        {
            return Numerics.Clamp(x, Min, Max);
        }

        public string ToString(int decs)
        {
            var temp = string.Format("{{0:f{0}}} - {{1:f{0}}}", decs);
            return string.Format(temp, a, b);
        }

        public override string ToString()
        {
            return ToString(2);
        }
    }


    [System.Serializable]
    public struct RangeFloat
    {
        public float a;
        public float b;

        public RangeFloat(RangeFloat rf) : this(rf.a, rf.b)
        {

        }

        public RangeFloat(float aa, float bb)
        {
            a = aa;
            b = bb;
        }

        public float Min => Mathf.Min(a, b);

        public float Max => Mathf.Max(a, b);

        public float Rnd => Random.Range(Min, Max);

        public float Avg => (a + b) / 2f;

        public float Lerp(float t, bool unclamped = false)
        {
            return unclamped ? Mathf.LerpUnclamped(a, b, t) : Mathf.Lerp(a, b, t);
        }

        public float InverseLerp(float x, bool unclamped = false)
        {
            return unclamped ? Numerics.InverseLerpUnclamped(a, b, x) : Mathf.InverseLerp(a, b, x);
        }

        public float Clamp(float x)
        {
            return Mathf.Clamp(x, Min, Max);
        }

        public string ToString(int decs)
        {
            var temp = string.Format("{{0:f{0}}} - {{1:f{0}}}", decs);
            return string.Format(temp, a, b);
        }

        public override string ToString()
        {
            return ToString(2);
        }
    }

    [System.Serializable]
    public struct RangeInteger
    {
        public int a; // inclusive
        public int b; // inclusive

        public RangeInteger(int aa, int bb)
        {
            a = aa;
            b = bb;
        }

        public int Min => Mathf.Min(a, b);

        public int Max => Mathf.Max(a, b);

        public int Rnd => Random.Range(Min, Max + 1);

        public float Lerp(float t)
        {
            var e = Mathf.Lerp(a, b, t);
            return Mathf.Clamp(Mathf.RoundToInt(e), a, b);
        }

        public float InverseLerp(float x)
        {
            return Mathf.InverseLerp(a, b, x);
        }

        public float Clamp(float x)
        {
            return Mathf.Clamp(x, Min, Max);
        }

        public override string ToString()
        {
            return $"{a} - {b}";
        }
    }
}
