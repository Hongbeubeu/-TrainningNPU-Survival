using UnityEngine;

namespace Npu.Algorithm
{

    public class Polygon
    {
        Vector3[] vertices;

        public Vector3 Center { get; private set; }

        public Polygon(Vector3[] vertices)
        {
            this.vertices = vertices;
            for (var i = 0; i < vertices.Length; i++)
            {
                Center += vertices[i];
            }

            Center /= vertices.Length;
        }

        public bool IsInside(Vector3 p)
        {
            int i, j = vertices.Length - 1;
            var oddNodes = false;

            for (i = 0; i < vertices.Length; i++)
            {

                if ((vertices[i].z < p.z && vertices[j].z >= p.z
                     || vertices[j].z < p.z && vertices[i].z >= p.z)
                    && (vertices[i].x <= p.x || vertices[j].x <= p.x))
                {
                    oddNodes ^= (vertices[i].x + (p.z - vertices[i].z) / (vertices[j].z - vertices[i].z) *
                        (vertices[j].x - vertices[i].x) < p.x);
                }

                j = i;
            }

            return oddNodes;
        }

        public bool Intersect(Vector3 p0, Vector3 p1, out Vector3 intersection)
        {
            intersection = Vector3.zero;
            for (var i = 0; i < vertices.Length; i++)
            {
                if (LineSegementsIntersect(p0, p1, vertices[i], vertices[(i + 1) % vertices.Length], out intersection))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsZero(float p) => Mathf.Abs(p) < 1e-5f;

        public static float Cross(Vector3 a, Vector3 b) => a.x * b.z - a.z * b.x;
        public static float Dot(Vector3 a, Vector3 b) => a.x * b.x + a.z * b.z;

        public static bool LineSegementsIntersect(Vector3 p, Vector3 p2, Vector3 q, Vector3 q2,
            out Vector3 intersection, bool considerCollinearOverlapAsIntersect = false)
        {
            intersection = new Vector3();

            var r = p2 - p;
            var s = q2 - q;
            var rxs = Cross(r, s);
            var qpxr = Cross(q - p, r);

            // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
            if (IsZero(rxs) && IsZero(qpxr))
            {
                // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
                // then the two lines are overlapping,
                if (considerCollinearOverlapAsIntersect)
                    if ((0 <= Dot((q - p), r) && Dot((q - p), r) <= Dot(r, r)) ||
                        (0 <= Dot((p - q), s) && Dot((p - q), s) <= Dot(s, s)))
                        return true;

                // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
                // then the two lines are collinear but disjoint.
                // No need to implement this expression, as it follows from the expression above.
                return false;
            }

            // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
            if (IsZero(rxs) && !IsZero(qpxr))
                return false;

            // t = (q - p) x s / (r x s)
            var t = Cross(q - p, s) / rxs;

            // u = (q - p) x r / (r x s)

            var u = Cross(q - p, r) / rxs;

            // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
            // the two line segments meet at the point p + t r = q + u s.
            if (!IsZero(rxs) && (0 <= t && t <= 1) && (0 <= u && u <= 1))
            {
                // We can calculate the intersection point using either t or u.
                intersection = p + t * r;

                // An intersection was found.
                return true;
            }

            // 5. Otherwise, the two line segments are not parallel but do not intersect.
            return false;
        }
    }

}