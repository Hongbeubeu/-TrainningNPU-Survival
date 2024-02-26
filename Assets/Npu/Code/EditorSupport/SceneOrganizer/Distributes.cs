#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using Npu.Common;
using Npu.Helper;
using Npu.Utilities;

/***
 * @authors: Thanh Le (William)  
 */

namespace Npu.EditorSupport
{
    public class Distributes : ASub
    {
        float distributeSpacing;
        float adjustFactor = 1;
        bool adjustFromCenter = true;
        Axis adjustAxes = Axis.X | Axis.Y | Axis.Z;
        Vector3 adjustRefFw = Vector3.forward;
        Vector3 adjustRefRot = Vector3.up;

        public override void OnGUI()
        {
            Section("Distribute", () =>
            {
                distributeSpacing = EditorGUILayout.FloatField("Spacing", distributeSpacing);
                using (new DisabledGui(TooFewObjects))
                {
                    for (var i = -1; i <= 1; i += 2)
                    {
                        using (new HorizontalLayout())
                        {
                            AxisButtons(default, i, ax => Distribute(SelectedGameObjects, ax, i, false, distributeSpacing));
                        }
                    }
                    for (var i = -1; i <= 1; i += 2)
                    {
                        using (new HorizontalLayout())
                        {
                            AxisButtons("{0} only", i, ax => Distribute(SelectedGameObjects, ax, i, true, distributeSpacing));
                        }
                    }
                }
            });
            Section("Adjust Distribution", () =>
            {
                adjustFactor = EditorGUILayout.FloatField("Factor", adjustFactor);
                adjustFromCenter = EditorGUILayout.Toggle("From center", adjustFromCenter);
                using (new HorizontalLayout())
                {
                    EditorGUILayout.PrefixLabel("Ref forward");
                    EditorGUILayout.BeginVertical();
                    adjustRefFw = EditorGUILayout.Vector3Field(GUIContent.none, adjustRefFw);
                    var eul = Quaternion.FromToRotation(Vector3.forward, adjustRefFw).eulerAngles;
                    eul = EditorGUILayout.Vector3Field(GUIContent.none, eul);
                    adjustRefFw = Quaternion.Euler(eul) * Vector3.forward;
                    EditorGUILayout.EndVertical();
                }
                using (new HorizontalLayout())
                {
                    EditorGUILayout.PrefixLabel("Ref rot axis");
                    EditorGUILayout.BeginVertical();
                    adjustRefRot = EditorGUILayout.Vector3Field(GUIContent.none, adjustRefRot);
                    var eul = Quaternion.FromToRotation(Vector3.up, adjustRefRot).eulerAngles;
                    eul = EditorGUILayout.Vector3Field(GUIContent.none, eul);
                    adjustRefRot = Quaternion.Euler(eul) * Vector3.up;
                    EditorGUILayout.EndVertical();
                }
                using (new HorizontalLayout())
                {
                    EditorGUILayout.PrefixLabel("Axis");
                    EditorGUIUtils.EnumFlags(() => adjustAxes, t => adjustAxes = t, s => GUILayout.Button(s));
                }
                using (new DisabledGui(TooFewObjects))
                {
                    using (new HorizontalLayout())
                    {
                        if (GUILayout.Button("Scale Positions"))
                            Distribute_ScalePosition(SelectedGameObjects, adjustAxes, adjustFromCenter, adjustFactor);
                        if (GUILayout.Button("Scale Angle"))
                            Distribute_ScaleAngle(SelectedGameObjects, adjustAxes, adjustFromCenter, adjustFactor, adjustRefFw);
                    }
                    using (new HorizontalLayout())
                    {
                        if (GUILayout.Button("Scale Up"))
                            Distribute_BulkScale(SelectedGameObjects, adjustAxes, adjustFactor);
                        if (GUILayout.Button("Scale Down"))
                            Distribute_BulkScale(SelectedGameObjects, adjustAxes, 1 / adjustFactor);
                    }

                    using (new HorizontalLayout())
                    {
                        if (GUILayout.Button("Center on Parent"))
                            Distribute_CenterOnParent(SelectedGameObjects, adjustAxes);
                    }

                    using (new HorizontalLayout())
                    {
                        if (GUILayout.Button("Even offset"))
                            Distribute_EvenOffset(SelectedGameObjects, adjustAxes, adjustFromCenter, adjustFactor);
                        if (GUILayout.Button("Even angles"))
                            Distribute_EvenAngles(SelectedGameObjects, adjustAxes, adjustFromCenter, adjustFactor, adjustRefFw, adjustRefRot);
                    }
                }
            });
        }

        #region Actions
        public void Distribute(IEnumerable<GameObject> gos, Axis axes, float direction, bool specificAxis, float spacing)
        {
            Vector3 center = default;
            Bounds prevBounds = default;
            var started = false;

            gos = gos.OrderBy(go => IsActive(go) ? 0 : 1);
            foreach (var go in gos)
            {
                var bounds = ObjectUtils.GetRendererBounds(go, false);
                if (!started)
                {
                    started = true;
                    center = go.transform.position;
                }
                else
                {
                    var offset = prevBounds.extents + bounds.extents + Vector3.one * spacing;
                    offset = offset.Mult(axes.HasFlag(Axis.X) ? 1 : 0, axes.HasFlag(Axis.Y) ? 1 : 0, axes.HasFlag(Axis.Z) ? 1 : 0) * direction;
                    center += offset;
                    Edit(go.transform);
                    if (specificAxis)
                    {
                        var p = go.transform.position;
                        if (axes.HasFlag(Axis.X)) p.x = center.x;
                        if (axes.HasFlag(Axis.Y)) p.y = center.y;
                        if (axes.HasFlag(Axis.Z)) p.z = center.z;
                        go.transform.position = p;
                    }
                    else go.transform.position = center;
                }
                prevBounds = bounds;
            }
        }

        public void Distribute_ScalePosition(IEnumerable<GameObject> gos, Axis axes, bool fromCenter, float mult)
        {
            if (!gos.Any() || mult == 1) return;

            var pivot = gos.FirstOrDefault().transform.position;
            if (fromCenter) pivot = Math3DUtils.AverageVectors(gos.Select(go => go.transform.position));

            foreach (var i in gos)
            {
                var t = i.transform;
                Edit(t);

                var pOld = t.position;
                var pNew = (pOld - pivot) * mult + pivot;
                var pFinal = pOld;
                axes.TransferValue(pNew, ref pFinal);
                t.position = pFinal;
            }
        }

        public void Distribute_BulkScale(IEnumerable<GameObject> gos, Axis axes, float mult)
        {
            if (!gos.Any() || mult == 1) return;

            foreach (var i in gos)
            {
                var t = i.transform;
                Edit(t);

                var pOld = t.localScale;
                var pNew = pOld * mult;
                var pFinal = pOld;
                axes.TransferValue(pNew, ref pFinal);
                t.localScale = pFinal;
            }
        }

        public void Distribute_CenterOnParent(IEnumerable<GameObject> gos, Axis axes)
        {
            if (!gos.Any() || gos.GroupBy(i => i.transform.parent).Count() > 1)
            {
                Debug.LogError("WHAT?");
                return;
            }

            var parent = gos.First().transform.parent.position;
            var center = Math3DUtils.AverageVectors(gos.Select(go => go.transform.position));
            var offset = parent - center;

            foreach (var i in gos)
            {
                var t = i.transform;
                Edit(t);

                var pOld = t.position;
                var pNew = pOld + offset;
                var pFinal = pOld;
                axes.TransferValue(pNew, ref pFinal);
                t.position = pFinal;
            }
        }

        public void Distribute_ScaleAngle(IEnumerable<GameObject> gos, Axis axes, bool fromCenter, float mult, Vector3 fw)
        {
            if (!gos.Any() || mult == 1) return;

            var pivot = gos.FirstOrDefault().transform.position;
            if (fromCenter) pivot = Math3DUtils.AverageVectors(gos.Select(go => go.transform.position));

            foreach (var i in gos)
            {
                var t = i.transform;
                Edit(t);

                var pOld = t.position;
                var offset = pOld - pivot;
                var newOffset = Vector3.SlerpUnclamped(fw, offset, mult);
                newOffset = newOffset.normalized * offset.magnitude;
                var pNew = pivot + newOffset;
                var pFinal = pOld;
                axes.TransferValue(pNew, ref pFinal);
                t.position = pFinal;
            }
        }

        public void Distribute_EvenOffset(IEnumerable<GameObject> gos, Axis axes, bool fromCenter, float factor)
        {
            if (!gos.Any() || factor == 0) return;

            var pivot = gos.FirstOrDefault().transform.position;
            if (fromCenter) pivot = Math3DUtils.AverageVectors(gos.Select(go => go.transform.position));

            var avgOffsetLength = gos
                .Select(go => (go.transform.position - pivot).sqrMagnitude)
                .Where(s2 => s2 > 0).Select(s2 => Mathf.Sqrt(s2))
                .DefaultIfEmpty(0).Average();

            foreach (var go in gos)
            {
                var tr = go.transform;

                var offset = tr.position - pivot;
                if (offset.sqrMagnitude > 0)
                {
                    var properOffset = offset.normalized * avgOffsetLength;
                    var newOffset = Vector3.LerpUnclamped(offset, properOffset, factor);
                    var finalOffset = offset;
                    axes.TransferValue(newOffset, ref finalOffset);

                    Edit(tr);
                    tr.position = pivot + finalOffset;
                }
            }
        }

        public void Distribute_EvenAngles(IEnumerable<GameObject> gos, Axis axes, bool fromCenter, float factor, Vector3 fw, Vector3 rotAxis)
        {
            if (!gos.Any() || factor == 0) return;

            var pivot = gos.FirstOrDefault().transform.position;
            if (fromCenter) pivot = Math3DUtils.AverageVectors(gos.Select(go => go.transform.position));

            var finalGOs = gos.Where(go => go.transform.position != pivot).ToList();

            for (var i = 0; i < finalGOs.Count; i++)
            {
                var go = finalGOs[i];
                var tr = go.transform;

                var angle = (float)i / finalGOs.Count * 360;
                var myFW = Quaternion.AngleAxis(angle, rotAxis) * fw;

                var offset = tr.position - pivot;
                var magnitude = offset.magnitude;
                var properOffset = myFW.normalized * magnitude;
                var newOffset = Vector3.LerpUnclamped(offset, properOffset, factor).normalized * magnitude;
                var finalOffset = offset;
                axes.TransferValue(newOffset, ref finalOffset);

                Edit(tr);
                tr.position = pivot + finalOffset;
            }
        }
        #endregion
    }
}

#endif