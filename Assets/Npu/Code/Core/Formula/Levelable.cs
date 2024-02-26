using UnityEngine;
using Npu.Core;
#if UNITY_EDITOR
using UnityEditor;
using Npu.EditorSupport;
#endif

namespace Npu.Formula
{

    public abstract class Levelable : ScriptableObject
    {
        public abstract SecuredDouble GetValue(int n);
    }

#if UNITY_EDITOR
    public class LevelableEditor : Editor
    {
        int level;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();

            using (new Indent())
            {
                EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);

                level = EditorGUILayout.IntSlider("Level", level, 0, 100);
                EditorGUILayout.LabelField($"Value: {(target as Levelable).GetValue(level)}");
            }
        }
    }
#endif

}