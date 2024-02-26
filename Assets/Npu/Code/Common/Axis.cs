using UnityEngine;
using System.Collections.Generic;

namespace Npu.Common
{
    [System.Flags]
    public enum Axis
    {
        X = 1 << 0,
        Y = 1 << 1,
        Z = 1 << 2,
    }

    public static class AxisUtils
    {
        public static readonly Axis[] Axes = new Axis[] { Axis.X, Axis.Y, Axis.Z };

        public static bool IsAll(this Axis prop)
        {
            var pi = (int)prop;
            return pi == -1 || pi == ~0 || prop.Contains(Axis.X | Axis.Y | Axis.Z);
        }

        public static bool Contains(this Axis prop, Axis prop2)
        {
            return (prop & prop2) != 0;
        }

        public static Side3D Side(this Axis axis)
        {
            switch (axis)
            {
                case Axis.X: return Side3D.Right;
                case Axis.Y: return Side3D.Top;
                case Axis.Z: return Side3D.Front;
            }
            return 0;
        }

        public static Vector3Int Dir(this Axis axis)
        {
            return axis.Side().Dir();
        }

        public static float GetValue(this Axis axis, Vector3 vec)
        {
            if (axis == Axis.X) return vec.x;
            else if (axis == Axis.Y) return vec.y;
            else if (axis == Axis.Z) return vec.z;
            return 0;
        }
        
        public static void SetValue(this Axis axis, ref Vector3 vec, float val)
        {
            if (axis.Contains(Axis.X)) vec.x = val;
            if (axis.Contains(Axis.Y)) vec.y = val;
            if (axis.Contains(Axis.Z)) vec.z = val;
        }

        public static IEnumerable<Axis> Separate(this Axis axis)
        {
            for (var i = 0; i < Axes.Length; i++)
            {
                var a = Axes[i];
                if (axis.Contains(a)) yield return a;
            }
        }

        public static void TransferValue(this Axis axis, Vector3 from, ref Vector3 to)
        {
            for (var i = 0; i < Axes.Length; i++)
            {
                var a = Axes[i];
                if (axis.Contains(a))
                {
                    a.SetValue(ref to, a.GetValue(@from));
                }
            }
        }
    }

}
