using System;
using System.Collections.Generic;
using System.Linq;
using Npu.Helper;
using UnityEngine;

/***
 * @authors: Thanh Le (William)  
 */

namespace Npu.Utilities
{
    public static class Numerics
    {

        #region BASIC NUMERICAL & ENUMS
        public static double Clamp(double value, double min, double max)
        {
            if (value > max) value = max;
            else if (value < min) value = min;
            return value;
        }

        public static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * Clamp(t, 0, 1);
        }

        public static double LerpUnclamped(double a, double b, double t)
        {
            return a + (b - a) * t;
        }

        public static double InverseLerp(double a, double b, double x)
        {
            return Clamp(InverseLerpUnclamped(a, b, x), 0, 1);
        }

        public static double InverseLerpUnclamped(double a, double b, double x)
        {
            return b != a ? (x - a) / (b - a) : float.PositiveInfinity * (x - a);
        }

        public static float InverseLerpUnclamped(float a, float b, float x)
        {
            return b != a ? (x - a) / (b - a) : float.PositiveInfinity * (x - a);
        }

        /// <summary>
        /// return snapped value in a range. e.g. from 1 to 3, value 2.2547f, step 10 => 2.2
        /// </summary>
        public static float StepSnap(float from, float to, float value, int steps)
        {
            var ratio = Mathf.InverseLerp(from, to, value);
            ratio = Mathf.Round(ratio * (steps - 1)) / (steps - 1);
            return Mathf.Lerp(from, to, ratio);
        }

        /// <summary>
        /// count must be >=0, return from 0->count-1
        /// </summary>
        public static int Repeat(int value, int count)
        {
            if (count <= 0) return 0;
            while (value < 0) value += count;
            if (value < count) return value;
            return value % count;
        }

        /// <summary>
        /// pingpong with the ends happening half time, max must be >=0, return steps ...,2,1,0,1,2,...,max-1,max,max-1,...,2,1,0,1,2,...
        /// </summary>
        public static float PingPong(float value, int max)
        {
            if (max <= 0) return 0;
            var group = Mathf.CeilToInt(value / max);
            if (group % 2 == 0)
            {
                return max * group - value;
            }
            else
            {
                return value - max * (group - 1);
            }
        }

        public static double Repeat(double d, double a, double b)
        {
            if (a == b) return a;
            var il = InverseLerpUnclamped(a, b, d);
            var r = Math.Abs(il - Math.Truncate(il));
            if (il < 0) r = 1 - r;
            var o = (b - a) * r;
            return a + o;
        }

        public static double PingPong(double d, double a, double b)
        {
            if (a == b) return a;
            var il = InverseLerpUnclamped(a, b, d);
            var r = Math.Abs(il - Math.Truncate(il));
            if (il < 0) r = 1 - r;
            var o = (b - a) * r;
            var reverse = (long)Math.Abs(Math.Floor(il)) % 2 == 1;
            return reverse ? b - o : a + o;
        }

        public static int PositiveIntPow(int i, int p)
        {
            if (p <= 0) return 1;
            for (var c = 2; c <= p; c++)
            {
                i *= i;
            }
            return i;
        }

        public static float PositiveIntPow(float i, int p)
        {
            if (p <= 0) return 1;
            for (var c = 2; c <= p; c++)
            {
                i *= i;
            }
            return i;
        }

        public static double PositiveIntPow(double i, int p)
        {
            if (p <= 0) return 1;
            for (var c = 2; c <= p; c++)
            {
                i *= i;
            }
            return i;
        }

        public static void Order<T>(ref T t1, ref T t2) where T : struct, IComparable<T>
        {
            if (t2.CompareTo(t1) < 0)
            {
                var t = t2;
                t2 = t1;
                t1 = t;
            }
        }

        public static void ShiftEnum<T>(ref T val, int shift = 1, bool loop = false) where T : struct, IConvertible
        {
            if (ShiftEnum(val, shift, loop, out var val2))
            {
                val = val2;
            }
        }

        public static bool GetNextEnum<T>(T val, bool loop, out T next) where T : struct, IConvertible
        {
            return ShiftEnum(val, 1, loop, out next);
        }

        public static bool GetPrevEnum<T>(T val, bool loop, out T next) where T : struct, IConvertible
        {
            return ShiftEnum(val, -1, loop, out next);
        }

        public static bool ShiftEnum<T>(T val, int shift, bool loop, out T shifted) where T : struct, IConvertible
        {
            var type = typeof(T);
            if (type.IsEnum)
            {
                var enums = Enum.GetValues(type) as T[];
                var index = Array.IndexOf(enums, val);
                if (index >= 0)
                {
                    index += shift;
                    if (loop)
                    {
                        index = Repeat(index, enums.Length);
                    }
                    if (index >= 0 && index < enums.Length)
                    {
                        shifted = enums[index];
                        return true;
                    }
                }
            }
            shifted = val;
            return false;
        }

        #endregion


        #region COUNT, DISTRIBUTE, STATISTICS
        /// <summary>
        /// Think of a simple jump trajectory / bell parabola. Highest point has height of <paramref name="topHeight"/>, jump is normalized from x=0->x=1.
        /// Evaluate the height at any given normalized <paramref name="x"/>
        /// </summary>
        public static float GetNormalizedBellHeight(float topHeight, float x)
        {
            return -topHeight * 4 * Mathf.Pow(x - 0.5f, 2) + topHeight;
        }

        public static void CalculateBasicStatistics(IEnumerable<float> vals, out float mean, out float variance, out float stdDev)
        {
            var lmean = mean = vals.Average();
            variance = vals.Select(v => (v - lmean) * (v - lmean)).Average();
            stdDev = Mathf.Sqrt(variance);
        }

        /// <summary>
        /// Calculate the right count and size for row cells from min&max size. 
        /// Return true if defaultCount is within min-max calculation
        /// </summary>
        public static bool CalculateIdealCount(float availableSpace, float minSize, float maxSize, int defaultCount, out int count, out float size)
        {
            var minCount = Mathf.Max(1, Mathf.FloorToInt(availableSpace / maxSize));
            var maxCount = Mathf.Max(1, Mathf.FloorToInt(availableSpace / minSize));
            var goodness = defaultCount >= minCount && defaultCount <= maxCount;
            count = Mathf.Clamp(defaultCount, minCount, maxCount);
            size = availableSpace / count;
            return goodness;
        }

        /// <summary>
        /// Calculate ideal cell counts of <paramref name="elementCount"/> elements in a grid with min&max crRatio. 
        /// crRatio: column / row ratio
        /// </summary>
        public static Vector2Int CalculateIdealCount(int elementCount, float minCRRatio, float maxCRRatio)
        {
            if (elementCount <= 1 || minCRRatio <= 0.001f || maxCRRatio <= 0.001f || maxCRRatio < minCRRatio) return Vector2Int.one;
            float crr(int c, int r) => (float)c / r;

            int? lastCols = null, lastRows = null;
            var safeCounter = 10000;
            var cols = Mathf.CeilToInt(Mathf.Sqrt(elementCount));
            var rows = cols;
            float crRatio() => crr(cols, rows);
            while (crRatio() < minCRRatio || crRatio() > maxCRRatio)
            {
                if (crRatio() < minCRRatio)
                {
                    if (cols * (rows - 1) >= elementCount) rows--;
                    else cols++;
                }
                else if (crRatio() > maxCRRatio)
                {
                    if ((cols - 1) * rows >= elementCount) cols--;
                    else rows++;
                }

                if (safeCounter-- <= 0) break;
                if (lastCols.HasValue && lastRows.HasValue && cols == lastRows && rows == lastCols)
                {
                    break;
                }
                lastCols = cols;
                lastRows = rows;
            }
            return new Vector2Int(cols, rows);
        }

        public static Dictionary<T, int> OrderedDistribute<T>(IEnumerable<T> ts, int count, System.Func<T, int> getMaxCount)
        {
            var dict = ts.ToDictionary(b => b, b => 0);
            var remain = count;
            while (remain > 0 && dict.Count > 0)
            {
                var addedAny = false;
                foreach (var b in ts)
                {
                    if (remain <= 0) break;
                    if (getMaxCount == null || dict[b] < getMaxCount(b))
                    {
                        dict[b]++;
                        addedAny = true;
                        remain--;
                    }
                }
                if (!addedAny) break;
            }
            return dict;
        }

        /// <summary>
        /// Distribute amount so that all T(s) will have the same (or as equal as possible). Return the amount to add.
        /// </summary>
        public static Dictionary<T, float> EquityDistribute<T>(float amount, Dictionary<T, float> having)
        {
            if (having.Count < 0 || amount <= 0) return new Dictionary<T, float>();

            var sortedTargets = having.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
            var remaining = amount;

            var immaginaryHaving = having.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var sharing = new List<T>();

            for (var i = 0; i < sortedTargets.Count; i++)
            {
                var curr = sortedTargets[i];
                var next = ArrayExtensions.TryGet(sortedTargets, i + 1);

                sharing.Add(curr);
                var sharingCount = sharing.Count;

                var addableToEach = remaining / sharingCount;
                var toAddEach = addableToEach;

                if (next != null)
                {
                    var toBeEqual = immaginaryHaving[next] - immaginaryHaving[curr];
                    toAddEach = Mathf.Min(toAddEach, toBeEqual);
                }

                foreach (var s in sharing)
                {
                    immaginaryHaving[s] += toAddEach;
                    remaining -= toAddEach;
                }
            }

            foreach (var s in sortedTargets)
            {
                immaginaryHaving[s] -= having[s];
            }

            return immaginaryHaving;
        }
        #endregion


        #region COORDS & INDEXES
        public static Vector2Int IndexToCoords(int i, int w)
        {
            var y = i / w;
            var x = i - y * w;
            return new Vector2Int(x, y);
        }

        public static int CoordsToIndex(Vector2Int v, int w)
        {
            return CoordsToIndex(v.x, v.y, w);
        }

        public static int CoordsToIndex(int x, int y, int w)
        {
            return x + y * w;
        }
        
        /// <summary>
        /// (i) => (ia)
        /// </summary>
        public static void Recoords1D1D(int width, RectInt area, System.Action<int, int> action)
        {
            var aw = area.width;
            var ah = area.height;
            for (var xa = 0; xa < aw; xa++)
            {
                var x = xa + area.xMin;
                for (var ya = 0; ya < ah; ya++)
                {
                    var y = ya + area.yMin;
                    var i = CoordsToIndex(x, y, width);
                    var ia = CoordsToIndex(xa, ya, aw);
                    action?.Invoke(i, ia);
                }
            }
        }

        /// <summary>
        /// (i) => (xa,ya)
        /// </summary>
        public static void Recoords1D2D(int width, RectInt area, System.Action<int, int, int> action)
        {
            var aw = area.width;
            var ah = area.height;
            for (var xa = 0; xa < aw; xa++)
            {
                var x = xa + area.xMin;
                for (var ya = 0; ya < ah; ya++)
                {
                    var y = ya + area.yMin;
                    var i = CoordsToIndex(x, y, width);
                    action?.Invoke(i, xa, ya);
                }
            }
        }

        /// <summary>
        /// (x,y) => (xa,ya)
        /// </summary>
        public static void Recoords2D2D(RectInt area, System.Action<int, int, int, int> action)
        {
            var aw = area.width;
            var ah = area.height;
            for (var xa = 0; xa < aw; xa++)
            {
                var x = xa + area.xMin;
                for (var ya = 0; ya < ah; ya++)
                {
                    var y = ya + area.yMin;
                    action?.Invoke(x, y, xa, ya);
                }
            }
        }

        /// <summary>
        /// (x,y) => (ia)
        /// </summary>
        public static void Recoords2D1D(RectInt area, System.Action<int, int, int> action)
        {
            var aw = area.width;
            var ah = area.height;
            for (var xa = 0; xa < aw; xa++)
            {
                var x = xa + area.xMin;
                for (var ya = 0; ya < ah; ya++)
                {
                    var y = ya + area.yMin;
                    var ia = CoordsToIndex(xa, ya, aw);
                    action?.Invoke(x, y, ia);
                }
            }
        }
        #endregion

    }
}
