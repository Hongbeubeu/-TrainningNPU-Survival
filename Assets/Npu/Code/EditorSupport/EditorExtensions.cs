#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Npu.EditorSupport
{
    public static class EditorExtensions
    {
        public static void Dirty(this Object target)
        {
            EditorUtility.SetDirty(target);      
        }

        public static void RegisterUndo(this Object target, string undoText = null)
        {
            Undo.RegisterCompleteObjectUndo(target, undoText ?? target.name);
        }
        
        public static void DirtyWithUndo(this Object target, string undoText=null)
        {
            EditorUtility.SetDirty(target);      
            Undo.RegisterCompleteObjectUndo(target, undoText ?? target.name);
        }

        public static void Ping(this Object target)
        {
            EditorGUIUtility.PingObject(target);
        }
    }
}

#endif