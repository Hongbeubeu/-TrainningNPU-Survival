using System.Collections.Generic;
using System.Linq;
using Npu.Common;
using Npu.Utilities;
using UnityEngine;
using BigInteger = System.Numerics.BigInteger;

/***
 * @authors: Thanh Le (William)  
 */

namespace Npu.Helper
{
    public static class MathExtensions
    {
        #region Matrix
        public static Vector3 Position(this Matrix4x4 m)
        {
            return m.GetColumn(3);
        }

        public static Vector3 Scale(this Matrix4x4 m)
        {
            return new Vector3(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude);
        }
        #endregion


        #region Vector3
        public static float Average(this Vector3 v)
        {
            return (v.x + v.y + v.z) / 3f;
        }

        public static bool IsZero(this Vector3 v)
        {
            return v == Vector3.zero;
        }

        public static Vector3 Abs(this Vector3 v)
        {
            v.x = v.x < 0 ? -v.x : v.x;
            v.y = v.y < 0 ? -v.y : v.y;
            v.z = v.z < 0 ? -v.z : v.z;
            return v;
        }

        public static Vector3 Clamp(this Vector3 v, Vector3 min, Vector3 max)
        {
            return new Vector3(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y), Mathf.Clamp(v.z, min.z, max.z));
        }

        public static Vector3 Flat(this Vector3 v)
        {
            return new Vector3(v.x, 0, v.z);
        }

        public static Vector3 Set(this Vector3 v, Axis axis, float val)
        {
            if (axis.Contains(Axis.X)) v.x = val;
            if (axis.Contains(Axis.Y)) v.y = val;
            if (axis.Contains(Axis.Z)) v.z = val;
            return v;
        }

        public static Vector3 Mult(this Vector3 v, float x, float y, float z)
        {
            return v.Mult(new Vector3(x, y, z));
        }

        public static Vector3 Mult(this Vector3 v, Vector3 other)
        {
            return Vector3.Scale(v, other);
        }

        public static Vector3 Divide(this Vector3 v, Vector3 other)
        {
            return Vector3.Scale(v, new Vector3(1 / other.x, 1 / other.y, 1 / other.z));
        }

        public static Vector3Int Rounded(this Vector3 v)
        {
            return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
        }

        public static Vector3Int Floored(this Vector3 v)
        {
            return new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));
        }

        public static Vector3Int Ceiled(this Vector3 v)
        {
            return new Vector3Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y), Mathf.CeilToInt(v.z));
        }
        
        public static float ManhattanLength(this Vector3 v)
        {
            return Mathf.Abs(v.x) + Mathf.Abs(v.y) + Mathf.Abs(v.z);
        }

        public static IEnumerable<float> Dimensions(this Vector3 v)
        {
            yield return v.x;
            yield return v.y;
            yield return v.z;
        }
        #endregion


        #region Vector2

        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            var sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            var cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            var tx = v.x;
            var ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }

        public static float Average(this Vector2 v)
        {
            return (v.x + v.y) / 3f;
        }

        public static bool IsZero(this Vector2 v)
        {
            return v == Vector2.zero;
        }

        public static Vector2 Set(this Vector2 v, Axis axis, float val)
        {
            if (axis.Contains(Axis.X)) v.x = val;
            if (axis.Contains(Axis.Y)) v.y = val;
            return v;
        }

        public static Vector2 Abs(this Vector2 v)
        {
            v.x = v.x < 0 ? -v.x : v.x;
            v.y = v.y < 0 ? -v.y : v.y;
            return v;
        }

        public static Vector2 Clamp(this Vector2 v, Vector2 min, Vector2 max)
        {
            return new Vector2(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y));
        }

        public static Vector2 Mult(this Vector2 v, float x, float y)
        {
            v.x *= x;
            v.y *= y;
            return v;
        }

        public static Vector2 Mult(this Vector2 v, Vector2 other)
        {
            v.x *= other.x;
            v.y *= other.y;
            return v;
        }

        public static Vector2 Divide(this Vector2 v, float x, float y)
        {
            v.x /= x;
            v.y /= y;
            return v;
        }

        public static Vector2 Divide(this Vector2 v, Vector2 other)
        {
            v.x /= other.x;
            v.y /= other.y;
            return v;
        }

        public static Vector3 ToVector3(this Vector2 v)
        {
            return new Vector3(v.x, v.y, 0);
        }

        public static Vector2Int Rounded(this Vector2 v)
        {
            return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }

        public static Vector2Int Floored(this Vector2 v)
        {
            return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
        }

        public static Vector2Int Ceiled(this Vector2 v)
        {
            return new Vector2Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));
        }

        #endregion


        #region RectInt
        public static bool Overlaps(this RectInt r1, RectInt r2, bool includeTouch)
        {
            if (includeTouch)
            {
                if (r1.xMin > r2.xMax || r2.xMin > r1.xMax) return false;
                if (r1.yMin > r2.yMax || r2.yMin > r1.yMax) return false;
                return true;
            }
            else
            {
                if (r1.xMin >= r2.xMax || r2.xMin >= r1.xMax) return false;
                if (r1.yMin >= r2.yMax || r2.yMin >= r1.yMax) return false;
                return true;
            }
        }

        public static RectInt Encapsulate(this RectInt r, RectInt r2)
        {
            return r.Encapsulate(r2.xMin, r2.yMin).Encapsulate(r2.xMax, r2.yMax);
        }

        public static RectInt Encapsulate(this RectInt r, Vector2Int v)
        {
            return r.Encapsulate(v.x, v.y);
        }

        public static RectInt Encapsulate(this RectInt r, int x, int y)
        {
            if (x > r.xMax) r.xMax = x;
            if (y > r.yMax) r.yMax = y;
            if (x < r.xMin) r.xMin = x;
            if (y < r.yMin) r.yMin = y;
            return r;
        }

        #endregion


        #region Rect
        // 1 , 2
        // 0 , 3
        public static Vector2[] GetCorners(this Rect r)
        {
            var c1 = new Vector2(r.xMin, r.yMax);
            var c2 = new Vector2(r.xMax, r.yMin);
            return new Vector2[] { r.min, c1, r.max, c2 };
        }
        
        public static bool Contains(this Rect r, Rect smaller)
        {
            return r.Contains(smaller.min) && r.Contains(smaller.max);
        }

        public static Rect Encapsulate(this Rect r, Rect r2)
        {
            return r.Encapsulate(r2.min).Encapsulate(r2.max);
        }

        public static Rect Encapsulate(this Rect r, Vector2 v)
        {
            if (v.x > r.xMax) r.xMax = v.x;
            if (v.y > r.yMax) r.yMax = v.y;
            if (v.x < r.xMin) r.xMin = v.x;
            if (v.y < r.yMin) r.yMin = v.y;
            return r;
        }

        public static Vector2 Clamp(this Rect r, Vector2 v)
        {
            v.x = Mathf.Clamp(v.x, r.xMin, r.xMax);
            v.y = Mathf.Clamp(v.y, r.yMin, r.yMax);
            return v;
        }

        public static Vector2 InnerLerp(this Rect r, Vector2 rv)
        {
            return r.InnerLerp(rv.x, rv.y);
        }

        public static Vector2 InnerLerp(this Rect r, float rx, float ry)
        {
            return new Vector2(r.x + r.width * rx, r.y + r.height * ry);
        }

        public static Rect InnerLerp(this Rect r, float xMin, float yMin, float xMax, float yMax)
        {
            var min = r.InnerLerp(xMin, yMin);
            var max = r.InnerLerp(xMax, yMax);
            return new Rect(min, max - min);
        }

        public static Vector2 InnerInverseLerp(this Rect r, Vector2 v)
        {
            return r.InnerInverseLerp(v.x, v.y);
        }

        public static Vector2 InnerInverseLerp(this Rect r, float x, float y)
        {
            return new Vector2(Mathf.InverseLerp(r.x, r.x + r.width, x), Mathf.InverseLerp(r.y, r.y + r.height, y));
        }

        public static Vector2 RandomPosition(this Rect r)
        {
            return r.InnerLerp(Random.value, Random.value);
        }

        public static Rect FromCenter(this Rect r, Vector2 center, Vector2 size)
        {
            return new Rect(center - size / 2f, size);
        }

        public static Rect Union(this Rect r, Rect other)
        {
            var min = Vector2.Min(r.min, other.min);
            var max = Vector2.Max(r.max, other.max);
            return new Rect(min, max - min);
        }

        public static Rect ToPositive(this Rect r)
        {
            if (r.width < 0)
            {
                var t = r.xMin;
                r.xMin = r.xMax;
                r.xMax = t;
            }
            if (r.height < 0)
            {
                var t = r.yMin;
                r.yMin = r.yMax;
                r.yMax = t;
            }
            return r;
        }

        public static Rect Offset(this Rect r, Vector2 v)
        {
            var r2 = new Rect(r);
            r2.position += v;
            return r2;
        }

        public static Rect Offset(this Rect r, float vx, float vy)
        {
            return r.Offset(new Vector2(vx, vy));
        }

        public static Rect Scale(this Rect r, float f)
        {
            return r.Scale(f, f);
        }

        public static Rect Scale(this Rect r, float fx, float fy)
        {
            return r.FromCenter(r.center, Vector2.Scale(r.size, new Vector2(fx, fy)));
        }

        public static Rect Normalized(this Rect r, float fx, float fy)
        {
            return new Rect(r.x / fx, r.y / fy, r.width / fx, r.height / fy);
        }

        public static Rect Grown(this Rect r, float f)
        {
            return r.Grown(Vector2.one * f);
        }

        public static Rect Grown(this Rect r, float x, float y)
        {
            return r.Grown(new Vector2(x, y));
        }

        public static Rect Grown(this Rect r, Vector2 half)
        {
            var min = r.min - half;
            var max = r.max + half;
            return new Rect(min, max - min);
        }

        public static Rect Grown(this Rect r, float left, float right, float top, float bottom)
        {
            var min = r.min - new Vector2(left, bottom);
            var max = r.max + new Vector2(right, top);
            return new Rect(min, max - min);
        }

        // left to right, bottom to top
        public static Rect[] Split(this Rect rect, int cols = 1, int rows = 1)
        {
            rows = Mathf.Max(1, rows);
            cols = Mathf.Max(1, cols);

            var size = new Vector2(rect.width / cols, rect.height / rows);
            var rs = new Rect[rows * cols];

            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < cols; x++)
                {
                    var cx = rect.position.x + (x + 0.5f) * size.x;
                    var cy = rect.position.y + (y + 0.5f) * size.y;

                    var index = y * cols + x;
                    rs[index] = rect.FromCenter(new Vector2(cx, cy), size);
                }
            }
            return rs;
        }

        // left to right, bottom to top
        public static Rect[,] Split2D(this Rect rect, int cols = 1, int rows = 1)
        {
            rows = Mathf.Max(1, rows);
            cols = Mathf.Max(1, cols);

            var size = new Vector2(rect.width / cols, rect.height / rows);
            var rs = new Rect[cols, rows];

            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < cols; x++)
                {
                    var cx = rect.position.x + (x + 0.5f) * size.x;
                    var cy = rect.position.y + (y + 0.5f) * size.y;

                    var index = y * cols + x;
                    rs[x, y] = rect.FromCenter(new Vector2(cx, cy), size);
                }
            }
            return rs;
        }


        #endregion


        #region Bounds

        /// <summary> Expand bounds so that it contains vector </summary>
        public static void Encapsulate(this ref BoundsInt b, Vector3Int v)
        {
            var min = Vector3Int.Min(v, b.min);
            var max = Vector3Int.Min(v, b.max);
            b.SetMinMax(min, max);
        }

        public static Bounds Grown(this Bounds bounds, float h)
        {
            return bounds.Grown(Vector3.one * h);
        }

        public static Bounds Grown(this Bounds bounds, Vector3 half)
        {
            bounds.extents += half;
            return bounds;
        }

        public static Bounds Offset(this Bounds bounds, Vector3 v)
        {
            bounds.center += v;
            return bounds;
        }

        /// <summary> Extract a new bounds from normalized positions of this bounds </summary>
        public static Bounds Extract(this Bounds bounds, Vector3 from, Vector3 to, bool clamped)
        {
            Vector3 lerp(Vector3 n) => clamped ? bounds.InnerLerp(n.x, n.y, n.z) : bounds.InnerLerpUnclamped(n.x, n.y, n.z);
            var b2 = new Bounds();
            b2.SetMinMax(lerp(from), lerp(to));
            return b2;
        }

        /// <summary> Expand the bounds equally on 3 sides </summary>
        public static BoundsInt Grown(this BoundsInt b, int i)
        {
            return b.Grown(Vector3Int.one * i);
        }

        /// <summary> Expand the bounds each min and max by vector </summary>
        public static BoundsInt Grown(this BoundsInt b, Vector3Int i)
        {
            b.max += i;
            b.min -= i;
            return b;
        }

        public static Rect AsRect(this Bounds bound)
        {
            return new Rect(bound.min, bound.size);
        }

        public static Rect AsRectTopDown(this Bounds bound)
        {
            return new Rect(new Vector2(bound.min.x, bound.min.z), new Vector2(bound.size.x, bound.size.z));
        }

        /// <summary> Interpolate a position inside a bounds from 3d normalized coordinates. (x,y,z) 0->1 </summary>
        public static Vector3 InnerLerp(this Bounds r, float x, float y, float z)
        {
            var min = r.min;
            var max = r.max;
            return new Vector3(Mathf.Lerp(min.x, max.x, x), Mathf.Lerp(min.y, max.y, y), Mathf.Lerp(min.z, max.z, z));
        }

        /// <summary> Interpolate a position inside a bounds from 3d normalized coordinates. (x,y,z) like 0->1, can be <0 and >1 </summary>
        public static Vector3 InnerLerpUnclamped(this Bounds r, float x, float y, float z)
        {
            var min = r.min;
            var max = r.max;
            return new Vector3(Mathf.LerpUnclamped(min.x, max.x, x), Mathf.LerpUnclamped(min.y, max.y, y), Mathf.LerpUnclamped(min.z, max.z, z));
        }

        /// <summary> Return the normalized & localized position of "pos" inside bounds </summary>
        public static Vector3 InnerInverseLerp(this Bounds r, Vector3 pos, bool clamped = false)
        {
            var outvec = Vector3.zero;
            var min = r.min;
            var max = r.max;
            outvec.x = clamped ? Mathf.InverseLerp(min.x, max.x, pos.x) : Numerics.InverseLerpUnclamped(min.x, max.x, pos.x);
            outvec.y = clamped ? Mathf.InverseLerp(min.y, max.y, pos.y) : Numerics.InverseLerpUnclamped(min.y, max.y, pos.y);
            outvec.z = clamped ? Mathf.InverseLerp(min.z, max.z, pos.z) : Numerics.InverseLerpUnclamped(min.z, max.z, pos.z);
            return outvec;
        }

        /// <summary> Get a random position inside a bounds </summary>
        public static Vector3 RandomPosition(this Bounds r)
        {
            return r.InnerLerp(Random.value, Random.value, Random.value);
        }

        public static Vector3 Clamp(this Bounds b, Vector3 v)
        {
            var vmin = b.min;
            var vmax = b.max;
            v.x = Mathf.Clamp(v.x, vmin.x, vmax.x);
            v.y = Mathf.Clamp(v.y, vmin.y, vmax.y);
            v.z = Mathf.Clamp(v.z, vmin.z, vmax.z);
            return v;
        }

        /// <summary> Check if a bounds fully contains another </summary>
        public static bool Contains(this Bounds b, Bounds another)
        {
            return b.Contains(another.min) && b.Contains(another.max);
        }

        public static Bounds ToEnvelopedWorld(this Bounds bounds, Transform transform)
        {
            var corners = bounds.GetCorners();
            return Math3DUtils.GetBounds(corners.Select(v => transform.TransformPoint(v)));
        }

        public static Bounds ToEnvelopedLocal(this Bounds bounds, Transform transform)
        {
            var corners = bounds.GetCorners();
            return Math3DUtils.GetBounds(corners.Select(v => transform.InverseTransformPoint(v)));
        }

        /// <summary>
        /// Calculate the intersection of a ray from inside the bounds, if possible.
        /// </summary>
        /// <returns>The multiplier or the ray direction</returns>
        public static bool IntersectRayFromInside(this Bounds b, Ray ray, out float dist)
        {
            // use planes.raycast
            dist = 0;
            if (!b.Contains(ray.origin)) return false;

            var planes = b.GetPlanes(true);
            float d = -1;
            foreach (var plane in planes)
            {
                if (plane.Raycast(ray, out float dp))
                {
                    if (d < 0 || d > dp) d = dp;
                }
            }
            if (d >= 0) dist = d;

            return true;
        }

        /// <summary> Calculate the 6 planes making up the bounds. With normals pointing out or in. </summary>
        public static Plane[] GetPlanes(this Bounds b, bool normalsInside = false)
        {
            float f = normalsInside ? -1 : 1;

            var pls = new Plane[6];
            pls[0] = new Plane(Vector3.left * f, b.min);
            pls[1] = new Plane(Vector3.back * f, b.min);
            pls[2] = new Plane(Vector3.down * f, b.min);
            pls[3] = new Plane(Vector3.right * f, b.max);
            pls[4] = new Plane(Vector3.forward * f, b.max);
            pls[5] = new Plane(Vector3.up * f, b.max);
            return pls;
        }

        /// <summary> Get the corners of the bounds </summary>
        public static IEnumerable<Vector3> GetCorners(this Bounds b)
        {
            var min = b.min;
            var max = b.max;
            foreach (var px in new[] { min.x, max.x })
                foreach (var py in new[] { min.y, max.y })
                    foreach (var pz in new[] { min.z, max.z })
                        yield return new Vector3(px, py, pz);
        }

        /// <summary> Get the corners of the bounds </summary>
        public static IEnumerable<Vector3Int> GetCorners(this BoundsInt b)
        {
            var min = b.min;
            var max = b.max;
            foreach (var px in new[] { min.x, max.x })
                foreach (var py in new[] { min.y, max.y })
                    foreach (var pz in new[] { min.z, max.z })
                        yield return new Vector3Int(px, py, pz);
        }

        public static IEnumerable<(Vector3, Vector3)> GetEdges(this Bounds b)
        {
            var corners = b.GetCorners().ToList();
            yield return (corners[0], corners[1]);
            yield return (corners[0], corners[2]);
            yield return (corners[0], corners[4]);
            yield return (corners[3], corners[1]);
            yield return (corners[3], corners[2]);
            yield return (corners[3], corners[7]);
            yield return (corners[6], corners[2]);
            yield return (corners[6], corners[4]);
            yield return (corners[6], corners[7]);
            yield return (corners[5], corners[1]);
            yield return (corners[5], corners[4]);
            yield return (corners[5], corners[7]);
        }

        public static float InsideDistance(this Bounds b, Vector3 pos)
        {
            var min = b.min;
            var max = b.max;
            var x = Mathf.Min(Mathf.Abs(pos.x - min.x), Mathf.Abs(pos.x - max.x));
            var y = Mathf.Min(Mathf.Abs(pos.y - min.y), Mathf.Abs(pos.y - max.y));
            var z = Mathf.Min(Mathf.Abs(pos.z - min.z), Mathf.Abs(pos.z - max.z));
            return Mathf.Min(x, y, z);
        }

        public static Vector3 ClosestPointToOutside(this Bounds b, Vector3 pos)
        {
            // TODO: NEW FUNCTION
            return pos;
        }

        #endregion


        #region BigInt
        public static BigInteger Multiply(this BigInteger val, decimal d)
        {
            var bits = decimal.GetBits(d);
            var numerator = (1 - ((bits[3] >> 30) & 2)) *
                            unchecked(((BigInteger)(uint)bits[2] << 64) |
                                      ((BigInteger)(uint)bits[1] << 32) |
                                      (BigInteger)(uint)bits[0]);
            var denominator = BigInteger.Pow(10, (bits[3] >> 16) & 0xff);
            return val * numerator / denominator;
        }

        public static BigInteger LerpTo(this BigInteger val, BigInteger v2, decimal d)
        {
            return val + (v2 - val).Multiply(d);
        }
        #endregion

    }
}
