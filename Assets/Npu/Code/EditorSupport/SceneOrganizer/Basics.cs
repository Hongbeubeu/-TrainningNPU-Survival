#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using Object = UnityEngine.Object;

/***
 * @authors: Thanh Le (William)  
 */

namespace Npu.EditorSupport
{
    public class Basics : ASub
    {
        protected override bool DefaultOpened => true;

        int autoNameStartIndex;
        string autoNameText = "name_{0}";
        GameObject replacingPrefab;

        public override void OnGUI()
        {
            Section("Naming", () =>
            {
                autoNameText = EditorGUILayout.DelayedTextField("Text", autoNameText);
                autoNameStartIndex = EditorGUILayout.IntField("Start at", autoNameStartIndex);

                using (new DisabledGui(NoObjects))
                {
                    using (new HorizontalLayout())
                    {
                        try
                        {
                            var v = string.Format(autoNameText, autoNameStartIndex);
                            if (GUILayout.Button($"Name by hierarchy ( {v} )")) AutoName_Hierarchy(base.SelectedGameObjects, autoNameStartIndex, autoNameText);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError((object)$"{e.Message} {e.StackTrace}");
                        }
                    }

                    using (new HorizontalLayout())
                    {
                        if (GUILayout.Button("Add prefix")) AutoName_AddFix(base.SelectedGameObjects, autoNameText, true);
                        if (GUILayout.Button("Add suffix")) AutoName_AddFix(base.SelectedGameObjects, autoNameText, false);
                        
                    }
                    
                }
            });

            Section("Selections", () =>
            {
                using (new DisabledGui(NoObjects))
                {
                    using (new HorizontalLayout())
                    {
                        if (GUILayout.Button("Select Children")) Select_Children(SelectedGameObjects);
                        if (GUILayout.Button("Select Parents")) Select_Parents(SelectedGameObjects);
                    }

                    using (new HorizontalLayout())
                    {
                        replacingPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", replacingPrefab, typeof(GameObject), true);
                        using (new DisabledGui(replacingPrefab == null))
                        {
                            if (GUILayout.Button("Replace")) ReplaceSelections(replacingPrefab, SelectedGameObjects);
                        }
                    }
                }
            });

            Section("Parent", () =>
            {
                using (new DisabledGui(base.NoObjects))
                {

                    using (new HorizontalLayout())
                    {
                        if (GUILayout.Button((string)"Group")) SceneObjectsUtils.GroupObjects((List<GameObject>)Enumerable.ToList<GameObject>(base.SelectedGameObjects));
                        
                    }

                    using (new HorizontalLayout())
                    {
                        if (GUILayout.Button((string)"Wrap")) SceneObjectsUtils.WrapObjects((List<GameObject>)Enumerable.ToList<GameObject>(base.SelectedGameObjects));
                        if (GUILayout.Button((string)"Unwrap")) Unwrap((IEnumerable<GameObject>)base.SelectedGameObjects);
                        
                    }
                    
                    
                }
            });

        }

        #region Actions

        public string AutoName_ActualFormatTemplate(string template)
        {
            var repl = "[0]";
            if (!template.Contains(repl)) template += repl;
            template = template.Replace(repl, "{0}");
            return template;
        }

        public void AutoName_Hierarchy(IEnumerable<GameObject> gos, int startIndex, string template)
        {
            // template = AutoName_ActualFormatTemplate(template);

            var sortedGOs = gos.OrderBy(go => go.transform.GetSiblingIndex());
            var number = startIndex;
            foreach (var go in sortedGOs)
            {
                Edit(go);
                go.name = string.Format(template, number);
                number++;
            }
        }

        public void AutoName_AddFix(IEnumerable<GameObject> gos, string template, bool prefix)
        {
            foreach (var go in gos)
            {
                Edit(go);
                go.name = (prefix ? template : "") + go.name + (prefix ? "" : template);
            }
        }

        public void Select_Children(IEnumerable<GameObject> selections)
        {
            Selection.objects = selections.SelectMany(i => i.transform.OfType<Transform>()).Select(t => t.gameObject).ToArray();
        }

        public void Select_Parents(IEnumerable<GameObject> selections)
        {
            Selection.objects = selections.Select(i => i.transform.parent.gameObject).Distinct().ToArray();
        }

        public void ReplaceSelections(GameObject prefab, IEnumerable<GameObject> selections)
        {
            foreach (var i in selections) Replace(prefab, i);
        }

        public void Replace(GameObject prefab, GameObject selection)
        {
            Edit(selection.transform.parent);
            var t = selection.transform;

            var o = prefab.scene.name == null
                ? PrefabUtility.InstantiatePrefab(prefab, t.parent) as GameObject
                : Object.Instantiate(prefab, t);

            o.transform.localScale = t.localScale;
            o.transform.position = t.position;
            o.transform.rotation = t.rotation;
            o.transform.SetSiblingIndex(t.GetSiblingIndex());

            Undo.RegisterCreatedObjectUndo(o, "SceneOrg");
            Undo.DestroyObjectImmediate(selection);
        }

        public void Unwrap(IEnumerable<GameObject> gos)
        {
            foreach (var go in gos)
            {
                if (PrefabUtility.IsPartOfAnyPrefab(go))
                {
                    Debug.LogError("Skipping unwrapping a prefab");
                    continue;
                }

                for (var i = go.transform.childCount - 1; i >= 0; i--)
                {
                    var child = go.transform.GetChild(i);
                    Edit(child);
                    Undo.SetTransformParent(child, go.transform.parent, "construction");
                }
                Undo.DestroyObjectImmediate(go);
            }
        }

        #endregion
    }
}

#endif