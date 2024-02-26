using UnityEngine;

namespace Npu.Helper
{
    public static class TransformExtension
    {
        public static void ScaleAround(this Transform target, Transform pivot, Vector3 scale)
        {
            var pivotParent = pivot.parent;
            var pivotPos = pivot.position;
            pivot.parent = target;
            target.localScale = scale;
            target.position += pivotPos - pivot.position;
            pivot.parent = pivotParent;
        }

        public static TTransform GetAncestor<TTransform>(this TTransform transform, int ancestor)
            where TTransform : Transform
        {
            for (var i = 0; i < ancestor; i++)
            {
                if (transform.parent == null || !(transform.parent is TTransform)) return transform;
                transform = transform.parent as TTransform;
            }
            return transform;
        }
        
        public static Transform GetAncestor(this Transform transform, int ancestor)
        {
            for (var i = 0; i < ancestor; i++)
            {
                if (transform.parent == null) return transform;
                transform = transform.parent;
            }
            return transform;
        }
    }
}