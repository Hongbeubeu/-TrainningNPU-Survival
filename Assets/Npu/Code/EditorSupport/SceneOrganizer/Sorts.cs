#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using Npu.Common;
using Npu.Helper;

/***
 * @authors: Thanh Le (William)  
 */

namespace Npu.EditorSupport
{
    public class Sorts : ASub
    {
        bool sortUsingBounds;

        public override void OnGUI()
        {
            // sort index
            Section("Sort index", () =>
            {
                using (new DisabledGui(TooFewObjects))
                {
                    using (new HorizontalLayout())
                    {
                        EditorGUILayout.PrefixLabel("Sort basis");
                        if (GUILayout.Button((sortUsingBounds ? "bounds center" : "position"))) sortUsingBounds = !sortUsingBounds;
                    }
                    for (var i = -1; i <= 1; i += 2)
                    {
                        using (new HorizontalLayout())
                        {
                            AxisButtons("By {0}", i, ax => Sort_Position(SelectedGameObjects, ax, sortUsingBounds, i));
                        }
                    }
                    using (new HorizontalLayout())
                    {
                        if (ActiveGameObject && GUILayout.Button("Sort by name asc")) Sort_Name(SelectedGameObjects, true);
                        if (ActiveGameObject && GUILayout.Button("Sort by name desc")) Sort_Name(SelectedGameObjects, false);
                    }
                    using (new HorizontalLayout())
                    {
                        if (ActiveGameObject && GUILayout.Button("Place after " + ActiveGameObject)) Sort_SiblingIndex(SelectedGameObjects, ActiveGameObject);
                    }
                }
            });
        }

        #region Actions

        public void Sort_Position(IEnumerable<GameObject> gos, Axis axes, bool usingBounds, float mult = 1)
        {
            if (!gos.Any()) return;

            var index = gos.Select(go => go.transform.GetSiblingIndex()).Min();
            var sorted = gos.OrderBy(go =>
            {
                var pos = go.transform.position;
                if (usingBounds)
                {
                    pos = ObjectUtils.GetRendererBounds(go).center;
                }
                pos *= mult;
                pos.x *= axes.HasFlag(Axis.X) ? 1 : 0;
                pos.y *= axes.HasFlag(Axis.Y) ? 1 : 0;
                pos.z *= axes.HasFlag(Axis.Z) ? 1 : 0;
                return pos.x + pos.y + pos.z;
            }).ToList();
            foreach (var go in sorted)
            {
                Edit(go);
                Edit(go.transform);
                go.transform.SetSiblingIndex(index++);
            }
        }

        public void Sort_Name(IEnumerable<GameObject> gos, bool ascending)
        {
            var index = gos.Select(go => go.transform.GetSiblingIndex()).Min();
            var sorted = ascending ? gos.OrderBy(go => go.name) : gos.OrderByDescending(go => go.name);
            foreach (var go in sorted)
            {
                Edit(go);
                Edit(go.transform);
                go.transform.SetSiblingIndex(index++);
            }
        }

        public void Sort_SiblingIndex(IEnumerable<GameObject> gos, GameObject target)
        {
            var index = target.transform.GetSiblingIndex();
            var sorted = gos.OrderBy(go => Mathf.Abs(go.transform.GetSiblingIndex() - index)).ThenBy(go => go.transform.GetSiblingIndex() - index);
            foreach (var go in sorted)
            {
                Edit(go);
                Edit(go.transform);
                go.transform.SetSiblingIndex(index++);
            }
        }

        #endregion
    }
}

#endif