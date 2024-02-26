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
    public static class Math3DUtils
    {
        /// <summary>
        /// Project <paramref name="baseVec"/> onto <paramref name="projectOn"/> along <paramref name="projectOn"/> ray. 
        /// </summary>
        public static Vector3 ProjectForward(Vector3 baseVec, Vector3 projectOn)
        {
            var angle = Vector3.Angle(baseVec, projectOn);
            angle = Mathf.Min(90, angle);
            var cos = Mathf.Cos(angle * Mathf.Deg2Rad);
            return projectOn.normalized * baseVec.magnitude * cos;
        }

        /// <summary>
        /// Project <paramref name="point"/> onto line made from <paramref name="linePoint"/> and <paramref name="lineDir"/>
        /// </summary>
        public static Vector3 ProjectPointOnLine(Vector3 point, Vector3 linePoint, Vector3 lineDir)
        {
            var projVec = Vector3.Project(point - linePoint, lineDir);
            return linePoint + projVec;
        }

        public static Vector3 PointToLine(Vector3 point, Vector3 linePoint, Vector3 lineDir)
        {
            return ProjectPointOnLine(point, linePoint, lineDir) - point;
        }

        /// <summary>
        /// return Sqr distance between segments, the out floats are factor of seg 1 and 2 to get the closest points
        /// </summary>
        public static float SegmentsShortestPath(Vector3 s1a, Vector3 s1b, Vector3 s2a, Vector3 s2b, out float r1, out float r2)
        {
            var u = s1b - s1a;
            var v = s2b - s2a;
            var w = s1a - s2a;
            var a = Vector3.Dot(u, u);         // always >= 0
            var b = Vector3.Dot(u, v);
            var c = Vector3.Dot(v, v);         // always >= 0
            var d = Vector3.Dot(u, w);
            var e = Vector3.Dot(v, w);
            var D = a * c - b * b;        // always >= 0
            float sc, sN, sD = D;       // sc = sN / sD, default sD = D >= 0
            float tc, tN, tD = D;       // tc = tN / tD, default tD = D >= 0

            // compute the line parameters of the two closest points
            if (D < float.Epsilon)
            { // the lines are almost parallel
                sN = 0f;         // force using point P0 on segment S1
                sD = 1f;         // to prevent possible division by 0.0 later
                tN = e;
                tD = c;
            }
            else
            {                 // get the closest points on the infinite lines
                sN = (b * e - c * d);
                tN = (a * e - b * d);
                if (sN < 0)
                {        // sc < 0 => the s=0 edge is visible
                    sN = 0f;
                    tN = e;
                    tD = c;
                }
                else if (sN > sD)
                {  // sc > 1  => the s=1 edge is visible
                    sN = sD;
                    tN = e + b;
                    tD = c;
                }
            }

            if (tN < 0.0)
            {            // tc < 0 => the t=0 edge is visible
                tN = 0f;
                // recompute sc for this edge
                if (-d < 0.0)
                    sN = 0f;
                else if (-d > a)
                    sN = sD;
                else
                {
                    sN = -d;
                    sD = a;
                }
            }
            else if (tN > tD)
            {      // tc > 1  => the t=1 edge is visible
                tN = tD;
                // recompute sc for this edge
                if ((-d + b) < 0.0)
                    sN = 0;
                else if ((-d + b) > a)
                    sN = sD;
                else
                {
                    sN = (-d + b);
                    sD = a;
                }
            }
            // finally do the division to get sc and tc
            sc = (Mathf.Abs(sN) < float.Epsilon ? 0f : sN / sD);
            tc = (Mathf.Abs(tN) < float.Epsilon ? 0f : tN / tD);

            // get the difference of the two closest points
            var dP = w + (sc * u) - (tc * v);  // =  S1(sc) - S2(tc)

            r1 = sc;
            r2 = tc;
            return dP.sqrMagnitude;   // return the closest distance
        }





        /// <summary>
        /// Project <paramref name="point"/> onto line segment from <paramref name="sA"/> to <paramref name="sB"/>. <para/>
        /// Outcome is the projected point and lerp factor from <paramref name="sA"/> to <paramref name="sB"/>. <para/>
        /// Return true if the projected point is within the segment
        /// </summary>
        /// <param name="point">Starting point</param>
        /// <param name="sA">Segment point A</param>
        /// <param name="sB">Segment point B</param>
        /// <param name="proj">Projected point</param>
        /// <param name="factor">Projected point lerp factor from A->B</param>
        /// <returns>true if the projected point is within the segment</returns>
        public static bool ProjectPointOnSegment(Vector3 point, Vector3 sA, Vector3 sB, out Vector3 proj, out float factor)
        {
            var vPA = point - sA;
            var vAB = sB - sA;
            var l = vAB.magnitude;
            vAB = vAB.normalized;
            var t = Vector3.Dot(vPA, vAB);

            proj = sA + vAB * t;
            factor = t / l;

            return factor >= 0 && factor <= 1;
        }

        public static Vector3 ClosestPointOnSegment(Vector3 point, Vector3 sA, Vector3 sB, out float factor)
        {
            if (!ProjectPointOnSegment(point, sA, sB, out var proj, out factor))
            {
                factor = Mathf.Clamp01(factor);
                proj = Vector3.Lerp(sA, sB, factor);
            }
            return proj;
        }
        
        public static Vector3 ClosestPointOnSegment (Vector3 p, Vector3 a, Vector3 b) {
            var aB = b - a;
            var aP = p - a;
            var sqrLenAB = aB.sqrMagnitude;

            if (sqrLenAB == 0)
                return a;

            var t = Mathf.Clamp01 (Vector3.Dot (aP, aB) / sqrLenAB);
            return a + aB * t;
        }

        /// <summary>
        /// Check if point <paramref name="p"/> is inside triangle t1-t2-t3
        /// /// </summary>
        public static bool IsPointInTriangle(Vector3 p, Vector3 t1, Vector3 t2, Vector3 t3)
        {
            float sign(Vector3 p1, Vector3 p2, Vector3 p3) => (p1.x - p3.x) * (p2.z - p3.z) - (p2.x - p3.x) * (p1.z - p3.z);

            var b1 = sign(p, t1, t2) < 0.0f;
            var b2 = sign(p, t2, t3) < 0.0f;
            var b3 = sign(p, t3, t1) < 0.0f;
            return ((b1 == b2) && (b2 == b3));

        }

        public static Vector3 RandomPointInTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            var v1 = b - a; var r1 = UnityEngine.Random.value;
            var v2 = c - a; var r2 = UnityEngine.Random.value;
            var p = a + r1 * v1 + r2 * v2;
            if (!IsPointInTriangle(p, a, b, c))
            {
                var m = (b + c) / 2f;
                p = m - (p - m);
            }
            return p;
        }




        /// <summary>
        /// Get smoothed position from <paramref name="p1"/> to <paramref name="p2"/> from <paramref name="t"/> <para/>
        /// With <paramref name="p0"/> before <paramref name="p1"/> and <paramref name="p3"/> after <paramref name="p2"/>
        /// </summary>
        public static Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
            var a = 2f * p1;
            var b = p2 - p0;
            var c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            var d = -p0 + 3f * p1 - 3f * p2 + p3;

            //The cubic polynomial: a + b * t + c * t^2 + d * t^3
            var pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

            return pos;
        }



        /// <summary>
        /// Return a bounds containing all vectors, zero bounds if no vectors found
        /// </summary>
        public static BoundsInt GetBounds(IEnumerable<Vector3Int> vs)
        {
            var min = Vector3Int.zero;
            var max = Vector3Int.zero;
            var any = false;
            foreach (var v in vs)
            {
                if (!any)
                {
                    any = true;
                    min = v;
                    max = v;
                }
                else
                {
                    min = Vector3Int.Min(min, v);
                    max = Vector3Int.Max(max, v);
                }
            }
            return new BoundsInt(min, max - min);
        }

        /// <summary>
        /// Return a bounds containing all vectors, zero bounds if no vectors found
        /// </summary>
        public static Bounds GetBounds(IEnumerable<Vector3> vs)
        {
            var min = Vector3.zero;
            var max = Vector3.zero;
            var any = false;
            foreach (var v in vs)
            {
                if (!any)
                {
                    any = true;
                    min = v;
                    max = v;
                }
                else
                {
                    min = Vector3.Min(min, v);
                    max = Vector3.Max(max, v);
                }
            }
            return new Bounds((min + max) / 2f, max - min);
        }

        public static Bounds UnionBounds(IEnumerable<Bounds> bounds)
        {
            var b = new Bounds();
            var any = false;
            foreach (var bb in bounds)
            {
                if (bb == null) continue;
                if (!any)
                {
                    b = bb;
                    any = true;
                }
                else b.Encapsulate(bb);
            }
            return b;
        }

        public static Bounds ApproxTransformBounds(Bounds bounds, Matrix4x4 from, Matrix4x4 to)
        {
            var conversion = to * from;
            return GetBounds(bounds.GetCorners().Select(p => conversion.MultiplyPoint(p)));
        }



        /// <summary>
        /// Calculate average of vectors. Vector zero if empty list
        /// </summary>
        public static Vector3 AverageVectors(IEnumerable<Vector3> vs)
        {
            var v = Vector3.zero;
            var i = 0;
            foreach (var vx in vs)
            {
                v += vx;
                i++;
            }
            return i > 0 ? v / i : v;
        }





        public static bool GetNearestPositionOnSegments<T>(Vector3 from,
            IEnumerable<T> objects,
            Func<T, IEnumerable<(T, T)>> segmentFunc,
            Func<T, Vector3> posFunc,
            out Vector3 pos, out (T, T) segment, out float factor)
        {
            segment = default;
            pos = default;
            factor = -1;

            if (objects == null || !objects.Any() || segmentFunc == null || posFunc == null) return false;

            float minDSqr = -1;
            foreach (var obj in objects)
            {
                foreach (var seg in segmentFunc.Invoke(obj))
                {
                    var p1 = posFunc.Invoke(seg.Item1);
                    var p2 = posFunc.Invoke(seg.Item2);
                    var closest = ClosestPointOnSegment(from, p1, p2, out var f);
                    var dSqr = (from - closest).sqrMagnitude;
                    if (minDSqr < 0 || dSqr <= minDSqr)
                    {
                        pos = closest;
                        segment = seg;
                        factor = f;
                        minDSqr = dSqr;
                    }
                }
            }
            return factor >= 0;
        }


        public static Matrix4x4 ChildToParentMatrix(Transform child, Transform parent, bool includeChild, bool excludeScales = false)
        {
            if (child == parent) return Matrix4x4.identity;
            if (!child.IsChildOf(parent)) return parent.worldToLocalMatrix * child.localToWorldMatrix;

            Matrix4x4 tmt(Transform t) => Matrix4x4.TRS(t.localPosition, t.localRotation, excludeScales ? Vector3.one : t.localScale);

            var mt = includeChild ? tmt(child) : Matrix4x4.identity;
            var p = child;
            while (p && p != parent)
            {
                p = p.parent;
                mt = tmt(p) * mt;
            }
            return mt;
        }
    }
}
