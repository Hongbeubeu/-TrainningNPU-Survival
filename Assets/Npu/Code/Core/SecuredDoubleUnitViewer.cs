using UnityEngine;
#if UNITY_EDITOR
using Npu.EditorSupport;
using UnityEditor;
#endif

namespace Npu.Core
{
    public class SecuredDoubleUnitViewer : MonoBehaviour
    {
        public SecuredDouble value = 1;
        public Unit unitFormat;
        public SecuredDouble multi = 1000;

        public long key;
        public double masked;

        public enum Unit
        {
            aabb,
            e,
            DuTr
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SecuredDoubleUnitViewer))]
    public class SecuredDoubleUnitViewerEditor : Editor
    {
        double n = 1000;

        public override void OnInspectorGUI()
        {
            var t = target as SecuredDoubleUnitViewer;

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SecuredDoubleUnitViewer.value)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SecuredDoubleUnitViewer.unitFormat)));

            SecuredDouble.SetUnit((int) t.unitFormat);

            using (new HorizontalLayout())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SecuredDoubleUnitViewer.multi)));
                // n = EditorGUILayout.DoubleField(n);
                if (GUILayout.Button("*", GUILayout.Width(30)))
                {
                    t.value *= t.multi;
                }

                if (GUILayout.Button("/", GUILayout.Width(30)))
                {
                    t.value /= t.multi;
                }
            }

            EditorGUILayout.LabelField(string.Format("\tValue: {0}", t.value));

            EditorGUIUtils.Header("Debug");

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SecuredDoubleUnitViewer.key)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SecuredDoubleUnitViewer.masked)));
            EditorGUILayout.LabelField(string.Format("\tValue: {0}",
                new SecuredDouble(SecuredDouble.Pack(t.key, t.masked))));

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

}