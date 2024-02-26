#if UNITY_EDITOR


/*
namespace Npu.EditorSupport
{
    public class Spherize : ASub
    {
        float radius = 0.5f;

        CenterMode centerMode = CenterMode.AvgPositions;
        DistributeMode distributeMode = DistributeMode.Evenly;
        int dist_Even_OverrideCount = -1;
        int dist_LongLat_Longs = 8;
        int dist_LongLat_Lats = 8;
        bool dist_LongLat_SkipPolars = false;
        float dist_Spiral_Rotations = 4;

        bool faceOutward = true;
        Vector3 localDirToOutward = Vector3.up;

        public override void OnGUI()
        {
            Section("Spherize", () =>
            {
                radius = EditorGUILayout.FloatField("Radius", radius);

                EditorGUIUtils.DrawWithLabel(new GUIContent("Center mode"), () => EditorGUIUtils.EnumTabsColorful(() => centerMode, t => centerMode = t));
                EditorGUIUtils.DrawWithLabel(new GUIContent("Distribute mode"), () => EditorGUIUtils.EnumTabsColorful(() => distributeMode, t => distributeMode = t));
                using (new EditorGUIUtils.Indent(1, true))
                {
                    if (distributeMode == DistributeMode.Evenly)
                    {
                        dist_Even_OverrideCount = EditorGUILayout.IntField("Override count", dist_Even_OverrideCount);
                    }
                    else if (distributeMode == DistributeMode.LongLat)
                    {
                        dist_LongLat_Longs = EditorGUILayout.IntField("Longitudes", dist_LongLat_Longs);
                        dist_LongLat_Lats = EditorGUILayout.IntField("Latitudes", dist_LongLat_Lats);
                        dist_LongLat_SkipPolars = EditorGUILayout.Toggle("Skip polars", dist_LongLat_SkipPolars);
                    }
                    else if (distributeMode == DistributeMode.Spiral)
                    {
                        dist_Spiral_Rotations = EditorGUILayout.FloatField("Rotations", dist_Spiral_Rotations);
                    }
                }

                faceOutward = EditorGUILayout.Toggle("Face outward", faceOutward);
                using (new Indent())
                {
                    if (faceOutward) localDirToOutward = EditorGUILayout.Vector3Field("Local dir to outward", localDirToOutward);
                }

                using (new DisabledGui(TooFewObjects))
                {
                    if (GUILayout.Button("Sphereize"))
                    {
                        DoSpherize();
                    }
                }
            });
        }

        #region Actions

        void DoSpherize()
        {
            var gos = SelectedGameObjects;
            if (gos.Count < 2) return;

            var center = Vector3.zero;
            if (centerMode == CenterMode.ActiveObject) center = ActiveGameObject.transform.position;
            else if (centerMode == CenterMode.BoundsCenter) center = Math3DUtils.UnionBounds(gos.Select(go => ObjectUtils.GetRendererBounds(go))).center;
            else if (centerMode == CenterMode.AvgPositions) center = Math3DUtils.AverageVectors(gos.Select(go => go.transform.position));

            if (distributeMode == DistributeMode.Evenly)
            {
                int index = 0;
                foreach (var p in BasicGenerator.GenerateEvenlySpaceSpherePositions(gos.Count))
                {
                    DoPosition(gos[index], center, p * radius);
                    index++;
                }
            }
            else if (distributeMode == DistributeMode.LongLat)
            {
                float minLat = 0;
                float maxLat = dist_LongLat_Lats - 1;
                if (dist_LongLat_SkipPolars)
                {
                    minLat -= 0.5f;
                    maxLat += 0.5f;
                }
                int index = 0;
                for (int lati = 0; lati < dist_LongLat_Lats; lati++)
                {
                    var vert = (float)(lati - minLat) / (maxLat - minLat);
                    for (int longi = 0; longi < dist_LongLat_Longs; longi++)
                    {
                        if (!dist_LongLat_SkipPolars && longi > 0)
                        {
                            if (lati <= 0 || lati >= dist_LongLat_Lats - 1) break;
                        }

                        if (index < 0 || index >= gos.Count) break;

                        var horz = (float)longi / dist_LongLat_Longs;

                        var go = gos[index];
                        DoPosition(go, center, horz, vert);
                        index++;
                    }
                }
            }
            else if (distributeMode == DistributeMode.Spiral)
            {
                var count = gos.Count;
                for (int i = 0; i < count; i++)
                {
                    GameObject go = gos[i];
                    float r = (float)i / (count - 1);
                    float vert = r;
                    float horz = (float)Numerics.Repeat(r * dist_Spiral_Rotations, 0, 1);
                    DoPosition(go, center, horz, vert);
                }

            }
            else if (distributeMode == DistributeMode.Random)
            {
                foreach (var go in gos)
                {
                    DoPosition(go, center, Random.value, Random.value);
                }
            }
        }

        void DoPosition(GameObject go, Vector3 center, float horz, float vert)
        {
            var d = Quaternion.AngleAxis(horz * 360, Vector3.up) * Quaternion.AngleAxis(vert * 180, Vector3.right) * Vector3.down;
            DoPosition(go, center, d * radius);
        }

        void DoPosition(GameObject go, Vector3 center, Vector3 offset)
        {
            Edit(go.transform);
            go.transform.position = center + offset;
            if (faceOutward)
            {
                var goFW = go.transform.TransformDirection(localDirToOutward);
                var rot = Quaternion.FromToRotation(goFW, offset);
                go.transform.rotation = rot * go.transform.rotation;
            }
        }

        #endregion

        enum DistributeMode
        {
            Evenly,
            LongLat,
            Spiral,
            Random,
        }

    }
}
*/

#endif

