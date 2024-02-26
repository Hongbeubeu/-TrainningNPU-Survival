using System;
using UnityEngine;

/***
 * @authors: Thanh Le (William)  
 */


namespace Npu.Utilities
{
    [Serializable]
    public struct LineEquation
    {
        public Vector3 point;
        public Vector3 direction;

        public LineEquation(Vector3 point, Vector3 dir)
        {
            this.point = point;
            this.direction = dir;
        }

        public Vector3 Evaluate(float length)
        {
            return point + direction * length;
        }

        public Vector3? EvaluateAt(float? x = null, float? y = null, float? z = null)
        {
            var kx = (x - point.x) / direction.x;
            var ky = (y - point.y) / direction.y;
            var kz = (z - point.z) / direction.z;
            bool isGood(float? f) => f.HasValue && !float.IsNaN(f.Value) && !float.IsInfinity(f.Value);
            if (isGood(kx)) return Evaluate(kx.Value);
            else if (isGood(ky)) return Evaluate(ky.Value);
            else if (isGood(kz)) return Evaluate(kz.Value);
            return null;
        }

        public override string ToString()
        {
            return $"l : {point} + k{direction}";
        }

    }
}
