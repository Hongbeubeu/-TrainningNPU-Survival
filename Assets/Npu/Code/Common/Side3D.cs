using UnityEngine;
#if UNITY_EDITOR

#endif

namespace Npu.Common
{

    public enum Side3D
    {
        None = 0,
        Top = 1,
        Right = 2,
        Front = 3,
        Bottom = 4,
        Left = 5,
        Back = 6,
    }

    public static class Side3DExtensions
    {
        public static readonly Side3D[] AllSides = { Side3D.Top, Side3D.Right, Side3D.Front, Side3D.Bottom, Side3D.Left, Side3D.Back };
        public static readonly Side3D[] XZSides = { Side3D.Right, Side3D.Front, Side3D.Left, Side3D.Back };

        public static bool Contains(this Side3D prop, Side3D prop2)
        {
            return (prop & prop2) != 0;
        }

        public static Vector3Int Dir(this Side3D side)
        {
            switch (side)
            {
                case Side3D.Top: return Vector3Int.up;
                case Side3D.Right: return Vector3Int.right;
                case Side3D.Front: return new Vector3Int(0, 0, 1);
                case Side3D.Bottom: return Vector3Int.down;
                case Side3D.Left: return Vector3Int.left;
                case Side3D.Back: return new Vector3Int(0, 0, -1);
                default: return Vector3Int.zero;
            }
        }

        public static Axis GetAxis(this Side3D side)
        {
            if (side == Side3D.Right || side == Side3D.Left) return Axis.X;
            if (side == Side3D.Top || side == Side3D.Bottom) return Axis.Y;
            if (side == Side3D.Front || side == Side3D.Back) return Axis.Z;
            return 0;
        }

        public static Axis GetAxisExact(this Side3D side)
        {
            if (side == Side3D.Right) return Axis.X;
            if (side == Side3D.Top) return Axis.Y;
            if (side == Side3D.Front) return Axis.Z;
            return 0;
        }

        public static Side3D FromDir(Vector3 dir)
        {
            var dx = Mathf.Abs(dir.x);
            var dy = Mathf.Abs(dir.y);
            var dz = Mathf.Abs(dir.z);

            if (dx > dy && dx > dz) return dir.x > 0 ? Side3D.Right : Side3D.Left;
            else if (dy > dx && dy > dz) return dir.y > 0 ? Side3D.Top : Side3D.Bottom;
            else if (dz > dx && dz > dy) return dir.z > 0 ? Side3D.Front : Side3D.Back;
            return Side3D.None;
        }

        public static Side3D FromDirExact(Vector3Int dir)
        {
            foreach (var side in AllSides)
            {
                if (side.Dir() == dir) return side;
            }
            return Side3D.None;
        }

        public static bool IsDir(this Side3D side, Vector3 dir, float angle = 45)
        {
            return Vector3.Angle(side.Dir(), dir) <= angle;
        }


        public static Side3D Flip(this Side3D s)
        {
            if (s == Side3D.Top) return Side3D.Bottom;
            if (s == Side3D.Right) return Side3D.Left;
            if (s == Side3D.Front) return Side3D.Back;
            if (s == Side3D.Bottom) return Side3D.Top;
            if (s == Side3D.Left) return Side3D.Right;
            if (s == Side3D.Back) return Side3D.Front;
            return Side3D.None;
        }

        public static Side3D XZCW(this Side3D s)
        {
            if (s == Side3D.Front) return Side3D.Right;
            if (s == Side3D.Right) return Side3D.Back;
            if (s == Side3D.Back) return Side3D.Left;
            if (s == Side3D.Left) return Side3D.Front;
            return s;
        }
    }

}