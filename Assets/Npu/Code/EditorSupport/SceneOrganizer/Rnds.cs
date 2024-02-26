#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Npu.Common;
using Npu.Helper;

/***
 * @authors: Thanh Le (William)  
 */

namespace Npu.EditorSupport
{
    public class Rnds : ASub
    {
        RangeFloat randomPosX = new RangeFloat(-1, 1);
        RangeFloat randomPosY = new RangeFloat(-1, 1);
        RangeFloat randomPosZ = new RangeFloat(-1, 1);
        RangeFloat randomPosXZ = new RangeFloat(-1, 1);
        RangeFloat randomPosXY = new RangeFloat(-1, 1);
        RangeFloat randomPosXYZ = new RangeFloat(-1, 1);

        RangeFloat randomRotX = new RangeFloat(-90, 90);
        RangeFloat randomRotY = new RangeFloat(-180, 180);
        RangeFloat randomRotZ = new RangeFloat(-90, 90);

        RangeFloat randomScaleX = new RangeFloat(0.8f, 1.2f);
        RangeFloat randomScaleY = new RangeFloat(0.8f, 1.2f);
        RangeFloat randomScaleZ = new RangeFloat(0.8f, 1.2f);
        RangeFloat randomScaleXYZ = new RangeFloat(0.8f, 1.2f);

        public override void OnGUI()
        {
            Section("Randomize", () =>
            {
                GUILayout.Label("Position");
                using (new Indent(1))
                {
                    DrawRndPositionRow(Axis.X, ref randomPosX);
                    DrawRndPositionRow(Axis.Y, ref randomPosY);
                    DrawRndPositionRow(Axis.Z, ref randomPosZ);
                    DrawRndPositionRow(Axis.X | Axis.Y, ref randomPosXY);
                    DrawRndPositionRow(Axis.X | Axis.Z, ref randomPosXZ);
                    DrawRndPositionRow(Axis.X | Axis.Y | Axis.Z, ref randomPosXYZ);
                }
                GUILayout.Label("Rotate");
                using (new Indent())
                {
                    DrawRndRotateRow(Axis.X, ref randomRotX);
                    DrawRndRotateRow(Axis.Y, ref randomRotY);
                    DrawRndRotateRow(Axis.Z, ref randomRotZ);
                }
                GUILayout.Label("Scale");
                using (new Indent())
                {
                    DrawRndScaleRow(Axis.X, ref randomScaleX);
                    DrawRndScaleRow(Axis.Y, ref randomScaleY);
                    DrawRndScaleRow(Axis.Z, ref randomScaleZ);
                    DrawRndScaleRow((Axis)~0, ref randomScaleXYZ);
                }
            });
        }

        void DrawRndPositionRow(Axis axis, ref RangeFloat rangeRef)
        {
            var range = rangeRef;
            using (new HorizontalLayout())
            {
                var label = axis.ToString();
                if (axis != Axis.X && axis != Axis.Y && axis != Axis.Z)
                {
                    label = string.Join("", axis.Separate().Select(a => a.ToString()));
                }
                EditorGUILayout.PrefixLabel(label);

                range.a = EditorGUILayout.FloatField(range.a);
                range.b = EditorGUILayout.FloatField(range.b);
                if (GUILayout.Button("Set")) Rnd_Position(SelectedGameObjects, axis, range, 0);
                if (GUILayout.Button("+")) Rnd_Position(SelectedGameObjects, axis, range, 1);
                if (GUILayout.Button("*")) Rnd_Position(SelectedGameObjects, axis, range, 2);
            }
            rangeRef = range;
        }

        void DrawRndRotateRow(Axis axis, ref RangeFloat rangeRef)
        {
            var range = rangeRef;
            using (new HorizontalLayout())
            {
                var label = axis.ToString();
                if (axis != Axis.X && axis != Axis.Y && axis != Axis.Z)
                {
                    label = string.Join("", axis.Separate().Select(a => a.ToString()));
                }
                EditorGUILayout.PrefixLabel(label);

                range.a = EditorGUILayout.FloatField(range.a);
                range.b = EditorGUILayout.FloatField(range.b);
                if (GUILayout.Button("Set")) Rnd_Rotate(SelectedGameObjects, axis, range, true, false);
                if (GUILayout.Button("+")) Rnd_Rotate(SelectedGameObjects, axis, range, false, false);
            }
            rangeRef = range;
        }

        void DrawRndScaleRow(Axis axis, ref RangeFloat rangeRef)
        {
            var range = rangeRef;
            using (new HorizontalLayout())
            {
                var label = axis.ToString();
                if (axis != Axis.X && axis != Axis.Y && axis != Axis.Z)
                {
                    label = string.Join("", axis.Separate().Select(a => a.ToString()));
                }
                EditorGUILayout.PrefixLabel(label);

                range.a = EditorGUILayout.FloatField(range.a);
                range.b = EditorGUILayout.FloatField(range.b);
                if (GUILayout.Button("Set")) Rnd_Scale(SelectedGameObjects, axis, range, 0);
                if (GUILayout.Button("+")) Rnd_Scale(SelectedGameObjects, axis, range, 1);
                if (GUILayout.Button("*")) Rnd_Scale(SelectedGameObjects, axis, range, 2);
            }
            rangeRef = range;
        }

        #region Actions

        public void Rnd_Position(IEnumerable<GameObject> gos, Axis axis, RangeFloat value, int mode)
        {
            foreach (var go in gos)
            {
                Edit(go);
                Edit(go.transform);

                var pos = go.transform.localPosition;
                var rnd = value.Rnd;

                if (mode == 0)
                {
                    axis.SetValue(ref pos, rnd);
                }
                else if (mode == 1)
                {
                    var offset = Vector3.zero;
                    axis.SetValue(ref offset, rnd);
                    pos += offset;
                }
                else
                {
                    var offset = Vector3.one;
                    axis.SetValue(ref offset, rnd);
                    pos = pos.Mult(offset);
                }

                go.transform.localPosition = pos;
            }
        }

        public void Rnd_Rotate(IEnumerable<GameObject> gos, Axis axis, RangeFloat value, bool set, bool world)
        {
            foreach (var go in gos)
            {
                Edit(go);
                Edit(go.transform);
                var eul = world ? go.transform.eulerAngles : go.transform.localEulerAngles;
                if (set)
                {
                    axis.SetValue(ref eul, value.Rnd);
                }
                else
                {
                    var offset = Vector3.one;
                    axis.SetValue(ref offset, value.Rnd);
                    eul += offset;
                }
                if (world) go.transform.eulerAngles = eul;
                else go.transform.localEulerAngles = eul;
            }
        }

        public void Rnd_Scale(IEnumerable<GameObject> gos, Axis axis, RangeFloat value, int mode)
        {
            foreach (var go in gos)
            {
                Edit(go);
                Edit(go.transform);

                var scale = go.transform.localScale;
                var rnd = value.Rnd;

                if (mode == 0)
                {
                    axis.SetValue(ref scale, rnd);
                }
                else if (mode == 1)
                {
                    var offset = Vector3.zero;
                    axis.SetValue(ref offset, rnd);
                    scale += offset;
                }
                else
                {
                    var offset = Vector3.zero;
                    axis.SetValue(ref offset, rnd);
                    scale = scale.Mult(offset);
                }

                go.transform.localScale = scale;
            }
        }

        #endregion
    }
}

#endif