using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Npu
{
    public class ReadOnlyAttribute : PropertyAttribute {}
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Npu.ReadOnlyAttribute))]
    public class ReadonlyAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    }
#endif
}