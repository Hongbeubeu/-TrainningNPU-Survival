using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Npu.Helper;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

/***
 * @authors: Thanh Le (William)  
 */

#if UNITY_EDITOR
namespace Npu.EditorSupport
{
    public static class SceneObjectsUtils
    {



        public static void GroupObjects(List<GameObject> objs)
        {
            if (objs.Count <= 0) return;
            var name = objs.Count <= 1 ? objs[0].name : "parent";
            var layer = objs[0].layer;

            var bounds = objs.Select(o => ObjectUtils.GetRendererBounds(o));
            var b = bounds.FirstOrDefault();
            foreach (var b1 in bounds.Skip(1)) b.Encapsulate(b1);

            var pos = b.center;
            if (Event.current.capsLock) { }
            else pos.y = b.min.y;

            var parent = objs[0].transform.parent;
            var sibIndex = objs[0].transform.GetSiblingIndex();
            var groupObj = new GameObject(name);
            groupObj.transform.SetParent(parent);
            groupObj.transform.SetSiblingIndex(sibIndex);
            groupObj.transform.position = pos;

            Undo.RegisterCreatedObjectUndo(groupObj, "Group objects");
            foreach (var obj in objs.OrderBy(o => o.transform.GetSiblingIndex()))
            {
                Undo.SetTransformParent(obj.transform, groupObj.transform, "Group objects");
            }
            EditorApplication.RepaintHierarchyWindow();
            Selection.activeGameObject = null;
            Selection.activeGameObject = groupObj;
        }


        public static void WrapObjects(List<GameObject> objs)
        {
            foreach (var obj in objs)
            {
                if (ComponentUtility.CopyComponent(obj.transform))
                {
                    var wrapper = new GameObject(obj.name);
                    wrapper.transform.SetParent(obj.transform.parent);
                    if (obj.transform is RectTransform) wrapper.AddComponent<RectTransform>();

                    if (ComponentUtility.PasteComponentValues(wrapper.transform))
                    {
                        wrapper.layer = obj.layer;
                        wrapper.transform.SetSiblingIndex(obj.transform.GetSiblingIndex());
                        obj.transform.SetParent(wrapper.transform);
                    }
                    else
                    {
                        Object.DestroyImmediate(wrapper);
                    }
                }
            }
        }

        public static void SetToIdentityWithoutChildren(Transform tr)
        {
            ModifyTransformWithoutChildren(tr, t => t.ResetToIdentity(), "Reset to identity");
        }

        public static void ModifyTransformWithoutChildren(Transform tr, System.Action<Transform> modification, string label = "Modify parent")
        {
            var childs = new List<Transform>();
            for (var i = 0; i < tr.childCount; i++)
            {
                childs.Add(tr.GetChild(i));
            }

            foreach (var c in childs)
            {
                Undo.SetTransformParent(c, tr.parent, label);
            }

            tr.DirtyWithUndo();
            modification?.Invoke(tr);

            foreach (var c in childs)
            {
                Undo.SetTransformParent(c, tr, label);
                EditorUtility.SetDirty(c);
            }

        }

        public static void CopyPasteChanges(GameObject from, List<GameObject> tos)
        {
            var mods = PrefabUtility.GetPropertyModifications(from);
            var addComps = PrefabUtility.GetAddedComponents(from);
            var addGOs = PrefabUtility.GetAddedGameObjects(from);
            foreach (var to in tos)
            {
                Undo.RegisterFullObjectHierarchyUndo(to, "Copy paste changes");
                EditorUtility.SetDirty(to);
                PrefabUtility.SetPropertyModifications(to, mods);
            }
        }
    }
}
#endif
