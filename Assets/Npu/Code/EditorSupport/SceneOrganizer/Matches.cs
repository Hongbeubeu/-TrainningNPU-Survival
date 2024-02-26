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
    public class Matches : ASub
    {
        public override void OnGUI()
        {
            Section("Match", () =>
            {
                using (new DisabledGui(TooFewObjects))
                {
                    GUILayout.Label("Position");
                    using (new Indent())
                    {
                        using (new HorizontalLayout())
                        {
                            EditorGUILayout.PrefixLabel("Local");
                            AxisButtons(default, 0, ax => Match_Pos(SelectedGameObjects, ax, false));
                        }
                        using (new HorizontalLayout())
                        {
                            EditorGUILayout.PrefixLabel("World");
                            AxisButtons(default, 0, ax => Match_Pos(SelectedGameObjects, ax, true));
                        }
                    }

                    GUILayout.Label("Bounds");
                    using (new Indent(1))
                    {
                        foreach (var bs in BoundsSteps)
                        {
                            using (new HorizontalLayout())
                            {
                                EditorGUILayout.PrefixLabel(bs.ToString());
                                AxisButtons(default, 0, ax => Match_Bounds(SelectedGameObjects, ax, bs, bs));
                            }
                        }
                        using (new HorizontalLayout())
                        {
                            EditorGUILayout.PrefixLabel("Before active");
                            AxisButtons(default, 0, ax => Match_Bounds(SelectedGameObjects, ax, BoundsStep.Min, BoundsStep.Max));
                        }
                        using (new HorizontalLayout())
                        {
                            EditorGUILayout.PrefixLabel("After active");
                            AxisButtons(default, 0, ax => Match_Bounds(SelectedGameObjects, ax, BoundsStep.Max, BoundsStep.Min));
                        }
                    }

                    GUILayout.Label("Rotation");
                    using (new Indent())
                    {
                        using (new HorizontalLayout())
                        {
                            EditorGUILayout.PrefixLabel("Rotation");
                            if (GUILayout.Button("Local")) Match_Rot(SelectedGameObjects, false);
                            if (GUILayout.Button("World")) Match_Rot(SelectedGameObjects, true);
                        }
                        using (new HorizontalLayout())
                        {
                            EditorGUILayout.PrefixLabel("Direction");
                            AxisButtons(default, 0, ax => Match_Dir(SelectedGameObjects, ax));
                        }
                    }
                }
            });
        }

        #region Actions

        public void Match_Pos(IEnumerable<GameObject> gos, Axis axis, bool world)
        {
            var baseGO = gos.FirstOrDefault(IsActive) ?? gos.FirstOrDefault();
            var basePos = world ? baseGO.transform.position : baseGO.transform.localPosition;

            foreach (var go in gos)
            {
                if (go != baseGO)
                {
                    var myPos = world ? go.transform.position : go.transform.localPosition;
                    var offset = (basePos - myPos).Mult(axis.Dir());

                    Edit(go.transform);
                    if (world) go.transform.position += offset;
                    else go.transform.localPosition += offset;
                }
            }
        }

        public void Match_Bounds(IEnumerable<GameObject> gos, Axis axis, BoundsStep baseStep, BoundsStep step)
        {
            Vector3 getPos(Bounds b, BoundsStep st) => st == BoundsStep.Min ? b.min : st == BoundsStep.Max ? b.max : b.center;

            var baseGO = gos.FirstOrDefault(IsActive) ?? gos.FirstOrDefault();
            var baseBounds = ObjectUtils.GetRendererBounds(baseGO, false);
            var basePos = getPos(baseBounds, baseStep);

            foreach (var go in gos)
            {
                if (go != baseGO)
                {
                    var myBounds = ObjectUtils.GetRendererBounds(go, false);
                    var myPos = getPos(myBounds, step);
                    var offset = (basePos - myPos).Mult(axis.Dir());

                    Edit(go.transform);
                    go.transform.position += offset;
                }
            }
        }

        public void Match_Rot(IEnumerable<GameObject> gos, bool world)
        {
            var baseGO = gos.FirstOrDefault(IsActive) ?? gos.FirstOrDefault();
            foreach (var go in gos)
            {
                if (go != baseGO)
                {
                    Edit(go.transform);
                    if (world) go.transform.rotation = baseGO.transform.rotation;
                    else go.transform.localRotation = baseGO.transform.localRotation;
                }
            }
        }

        public void Match_Dir(IEnumerable<GameObject> gos, Axis axis)
        {
            Vector3 getVec(GameObject go) => axis == Axis.X ? go.transform.right : axis == Axis.Y ? go.transform.up : axis == Axis.Z ? go.transform.forward : Vector3.zero;

            var baseGO = gos.FirstOrDefault(IsActive) ?? gos.FirstOrDefault();
            var baseDir = getVec(baseGO);

            foreach (var go in gos)
            {
                if (go != baseGO)
                {
                    var myDir = getVec(go);
                    var rot = Quaternion.FromToRotation(myDir, baseDir);

                    Edit(go.transform);
                    go.transform.rotation = rot * go.transform.rotation;
                }
            }
        }

        #endregion
    }
}

#endif