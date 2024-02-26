using System.Collections.Generic;
using System.Linq;
using Npu.Helper;
using UnityEngine;

/***
 * @authors: Thanh Le (William)  
 */

namespace Npu.Utilities
{
    public static class Math2DUtils
    {
        #region Distances

        public static float PointToRectSquared(Vector2 point, Rect r)
        {
            float dx = 0;
            float dy = 0;
            if (point.x < r.xMin) dx = r.xMin - point.x;
            else if (point.x > r.xMax) dx = point.x - r.xMax;
            if (point.y < r.yMin) dy = r.yMin - point.y;
            else if (point.y > r.yMax) dy = point.y - r.yMax;
            return dx * dx + dy * dy;
        }

        public static float PointToLineSquared(Vector2 point, Vector2 lp, Vector2 ld)
        {
            return (ClosestPointOnLine(point, lp, ld) - point).sqrMagnitude;
        }

        public static float PointToSegmentSquared(Vector2 point, Vector2 sa, Vector2 sb)
        {
            return (ClosestPointOnSegment(point, sa, sb) - point).sqrMagnitude;
        }

        public static float SegmentToSegmentSquared(Vector2 s1a, Vector2 s1b, Vector2 s2a, Vector2 s2b)
        {
            if (s1a == s1b) return PointToSegmentSquared(s1a, s2a, s2b);
            if (s2a == s2b) return PointToSegmentSquared(s2a, s1a, s1b);

            SegmentsIntersect(s1a, s1b, s2a, s2b, out var intersect, out var overlap);
            if (overlap || intersect.HasValue) return 0;

            var d = PointToSegmentSquared(s1a, s2a, s2b);
            if (d <= 0) return 0;
            d = Mathf.Min(d, PointToSegmentSquared(s1b, s2a, s2b));
            if (d <= 0) return 0;
            d = Mathf.Min(d, PointToSegmentSquared(s2a, s1a, s1b));
            if (d <= 0) return 0;
            d = Mathf.Min(d, PointToSegmentSquared(s2b, s1a, s1b));
            return d;
        }

        #endregion


        #region Closest point

        public static Vector2 ClosestPointOnLine(Vector2 p, Vector2 lp, Vector2 ld)
        {
            if (ld.x == 0 && ld.y == 0) return lp;
            if (ld.x == 0) return new Vector2(lp.x, p.y);
            if (ld.y == 0) return new Vector2(p.x, lp.y);

            var k = ((p.x - lp.x) * ld.x + (p.y - lp.y) * ld.y) / (ld.x * ld.x + ld.y * ld.y);
            var x = lp.x + k * ld.x;
            var y = lp.y + k * ld.y;
            return new Vector2(x, y);
        }

        public static Vector2 ClosestPointOnSegment(Vector2 p, Vector2 sa, Vector2 sb)
        {
            if (sa == sb) return sa;

            var pol = ClosestPointOnLine(p, sa, sb - sa);
            var s1ToPol = pol - sa;
            var s1Tos2 = sb - sa;
            var ratio = s1Tos2.x == 0 ? s1ToPol.y / s1Tos2.y : s1ToPol.x / s1Tos2.x;

            if (ratio >= 1) return sb;
            else if (ratio <= 0) return sa;
            else return pol;
        }

        #endregion


        #region Intersections

        public static void LineSegmentIntersect(Vector2 p, Vector2 d, Vector2 sa, Vector2 sb, out Vector2? intersect,
            out bool overlap)
        {
            LinesIntersect(p, d, sa, sb - sa, out intersect, out overlap);
            if (intersect.HasValue)
            {
                var i = intersect.Value;
                if (!IsPointInSegmentRect(i, sa, sb)) intersect = null;
            }
        }


        public static void SegmentsIntersect(Vector2 s1a, Vector2 s1b, Vector2 s2a, Vector2 s2b, out Vector2? intersect,
            out bool overlap)
        {
            LinesIntersect(s1a, s1b - s1a, s2a, s2b - s2a, out intersect, out overlap);
            if (intersect.HasValue)
            {
                var i = intersect.Value;
                if (!IsPointInSegmentRect(i, s1a, s1b) || !IsPointInSegmentRect(i, s2a, s2b)) intersect = null;
            }
        }

        public static void LinesIntersect(Vector2 p1, Vector2 d1, Vector2 p2, Vector2 d2, out Vector2? intersect,
            out bool overlap)
        {
            if (AreParallel(d1, d2))
            {
                //is parallel
                var d3 = p2 - p1;
                overlap = d3 == Vector2.zero || AreParallel(d3, d1);
                intersect = null;
                return;
            }

            float x, y;
            if (d1.x == 0)
            {
                x = p1.x;
                y = (x - p2.x) / d2.x * d2.y + p2.y;
            }
            else if (d2.x == 0)
            {
                x = p2.x;
                y = (x - p1.x) / d1.x * d1.y + p1.y;
            }
            else
            {
                var s1 = d1.y / d1.x;
                var s2 = d2.y / d2.x;
                var b1 = p1.y - s1 * p1.x;
                var b2 = p2.y - s2 * p2.x;
                x = (b1 - b2) / (s2 - s1);
                y = s1 * x + b1;
            }

            intersect = new Vector2(x, y);
            overlap = false;
        }

        public static IEnumerator<Vector2> SegmentPolyIntersect(Vector2 sa, Vector2 sb, List<Vector2> poly)
        {
            if (poly.Count <= 1)
            {
                yield break;
            }

            for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i, i++)
            {
                var pa = poly[i];
                var pb = poly[j];
                SegmentsIntersect(sa, sb, pa, pb, out var pi, out var povl);
                if (pi.HasValue && !povl)
                {
                    yield return pi.Value;
                }
            }
        }

        #endregion


        #region Point check

        static float _Sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }

        public static bool IsPointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            var b1 = _Sign(pt, v1, v2) < 0.0f;
            var b2 = _Sign(pt, v2, v3) < 0.0f;
            var b3 = _Sign(pt, v3, v1) < 0.0f;
            return ((b1 == b2) && (b2 == b3));
        }

        public static bool IsPointInPoly(Vector2 p, List<Vector2> poly, bool preBoundsCheck = false)
        {
            var count = poly?.Count ?? -1;
            if (count <= 0) return false;
            else if (count == 1) return p == poly[0];
            else if (count == 2) return IsPointOnSegment(p, poly[0], poly[1]);
            else if (preBoundsCheck && !GetVectorsRect(poly).Contains(p)) return false;

            var inside = false;
            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                var vi = poly[i];
                var vj = poly[j];
                if ((vi.y > p.y) != (vj.y > p.y) && p.x < (vj.x - vi.x) * (p.y - vi.y) / (vj.y - vi.y) + vi.x)
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        public static bool IsPointOnSegment(Vector2 p, Vector2 sa, Vector2 sb)
        {
            if (sa == sb) return p == sa;
            var d = p - sa;
            var rx = sb.x == sa.x ? float.NaN : (d.x - sa.x) / (sb.x - sa.x);
            var ry = sb.y == sa.y ? float.NaN : (d.y - sa.y) / (sb.y - sa.y);
            return !float.IsNaN(rx) && !float.IsNaN(ry) && rx >= 0 && rx <= 1 && ry >= 0 && ry <= 1 &&
                   Mathf.Approximately(rx, ry);
        }

        public static bool IsPointInSegmentRect(Vector2 p, Vector2 sa, Vector2 sb)
        {
            return !(p.x < Mathf.Min(sa.x, sb.x) || p.x > Mathf.Max(sa.x, sb.x) || p.y < Mathf.Min(sa.y, sb.y) ||
                     p.y > Mathf.Max(sa.y, sb.y));
        }

        #endregion


        #region Vector2 / Vector2Int

        public static bool AreParallel(Vector2Int v1, Vector2Int v2)
        {
            if (v1.x == 0) return v2.x == 0 && (v1.y == v2.y || v1.y * v2.y != 0);
            return v2.x * v1.y / v1.x == v2.y;
        }

        public static bool AreParallel(Vector2 d1, Vector2 d2)
        {
            return d1.x == 0 && d2.x == 0 || d1.x * d2.x != 0 && d1.y / d1.x == d2.y / d2.x;
        }

        public static Rect GetVectorsRect(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return new Rect()
            {
                xMin = Mathf.Min(v1.x, v2.x, v3.x),
                xMax = Mathf.Max(v1.x, v2.x, v3.x),
                yMin = Mathf.Min(v1.y, v2.y, v3.y),
                yMax = Mathf.Max(v1.y, v2.y, v3.y),
            };
        }

        public static Rect GetVectorsRect(IEnumerable<Vector2> vs)
        {
            var min = Vector2.zero;
            var max = Vector2.zero;
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
                    min = Vector2.Min(min, v);
                    max = Vector2.Max(max, v);
                }
            }

            return new Rect(min, max - min);
        }

        public static Rect GetVectorsRect(List<Vector2Int> vecs)
        {
            var count = vecs?.Count ?? 0;
            if (count <= 0) return default;

            float minX = vecs[0].x;
            float minY = vecs[0].y;
            float maxX = vecs[0].x;
            float maxY = vecs[0].y;
            for (var i = 1; i < vecs.Count; i++)
            {
                Vector2 v = vecs[i];
                if (minX > v.x) minX = v.x;
                if (maxX < v.x) maxX = v.x;
                if (minY > v.y) minY = v.y;
                if (maxY < v.y) maxY = v.y;
            }

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// Calculate average of vectors. Vector zero if empty list
        /// </summary>
        public static Vector2 AverageVectors(List<Vector2> vs)
        {
            if (vs == null || vs.Count == 0) return Vector2.zero;
            var count = vs.Count;
            var sum = vs.Aggregate(Vector2.zero, (v1, v2) => v1 + v2);
            return sum / count;
        }

        #endregion


        #region Polygon

        public static bool IsPolyClockwise(List<Vector2> vs)
        {
            if (vs.Count <= 2) return false;

            float val = 0;
            for (int i = 0, j = vs.Count - 1; i < vs.Count; j = i++)
            {
                var vj = vs[j];
                var vi = vs[i];
                var x = (vi.x - vj.x) * (vi.y + vj.y);
                val += x;
            }

            return val >= 0;
        }


        public static List<Vector2> GetPolyOutwards(List<Vector2> vs)
        {
            var clockwise = IsPolyClockwise(vs);

            var l = new List<Vector2>();
            for (var i = 0; i < vs.Count; i++)
            {
                var pPrev = vs.GetRepeat(i - 1);
                var pThis = vs[i];
                var pNext = vs.GetRepeat(i + 1);

                var d1 = (pNext - pThis).normalized;
                var d2 = (pThis - pPrev).normalized;
                var sin = Mathf.Sin((180 - Vector2.Angle(d1, d2)) / 2 * Mathf.Deg2Rad);
                var mult = sin != 0 ? 1 / sin : 1;

                var dir = Vector2.Perpendicular(d1 + d2).normalized * mult;
                if (!clockwise) dir = -dir;

                l.Add(dir);
            }

            return l;
        }

        public static List<Vector2> ExpandPolyline(List<Vector2> vs, float thicknessHalf)
        {
            if (vs.Count < 2) return vs;

            var dirs = GetPolyOutwards(vs);
            dirs[0] = Vector2.Perpendicular(vs[0] - vs[1]).normalized;
            dirs[vs.Count - 1] = Vector2.Perpendicular(vs[vs.Count - 2] - vs[vs.Count - 1]).normalized;
            if (Vector2.Dot(dirs[0], dirs[1]) < 0) dirs[0] *= -1;
            if (Vector2.Dot(dirs[vs.Count - 1], dirs[vs.Count - 2]) < 0) dirs[vs.Count - 1] *= -1;

            var poly = vs.Zip(dirs, (v, d) => v + d * thicknessHalf)
                .Concat(vs.Zip(dirs, (v, d) => v - d * thicknessHalf).Reverse()).ToList();
            return poly;
        }

        public static List<Vector2> RemovePolygonIntersections(List<Vector2> poly)
        {
            if (poly.Count >= 4)
            {
                var vs = new List<Vector2>(poly);
                var toRemove = new List<int>();
                while (true)
                {
                    toRemove.Clear();
                    for (var i = 0; i < vs.Count; i++)
                    {
                        var pA = vs[i];
                        var pAPrev = vs.GetRepeat(i - 1);
                        var pB = vs.GetRepeat(i + 1);
                        var pBNext = vs.GetRepeat(i + 2);

                        SegmentsIntersect(pA, pAPrev, pB, pBNext, out var intersect, out var overlap);
                        if (intersect.HasValue && !overlap)
                        {
                            var pI = intersect.Value;
                            if (pI == pA) toRemove.Add(i);
                            else if (pI != pB)
                            {
                                vs[i] = pI;
                                toRemove.Add(i + 1);
                            }
                        }
                    }

                    if (toRemove.Count <= 0) break;
                    else
                    {
                        foreach (var i in toRemove.OrderByDescending(ii => ii))
                        {
                            vs.RemoveAt(i);
                        }
                    }
                }

                return vs;
            }

            return poly;
        }

        #endregion


        #region Rect

        // convert the movement of a rectangle into a polygon. think of a box sweeping on the ground, leaving a "polygon" trail
        public static IEnumerable<Vector2> RectOffsetIntoPolygon(Rect rect, Vector2 off)
        {
            var rect2 = new Rect(rect);
            rect2.position += off;
            if (off.x < 0) return RectOffsetIntoPolygon(rect2, -off);

            var c1 = rect.GetCorners();
            var c2 = rect2.GetCorners();

            if (off.x > 0)
            {
                if (off.y > 0) return new List<Vector2>() { c1[0], c1[1], c2[1], c2[3], c2[2], c1[2] };
                else if (off.y < 0) return new List<Vector2>() { c1[0], c1[1], c2[3], c2[3], c2[2], c1[0] };
                else return new List<Vector2>() { c1[0], c1[1], c2[3], c2[2] };
            }
            else
            {
                if (off.y > 0) return new List<Vector2>() { c1[0], c2[1], c2[3], c1[2] };
                else if (off.y < 0) return new List<Vector2>() { c1[1], c1[3], c2[2], c2[0] };
                else return new List<Vector2>() { c1[0], c1[1], c1[3], c1[2] };
            }
        }

        #endregion
    }
}