using Npu.Helper;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Npu.EditorSupport;
#endif

namespace Npu
{
    public class BoxAttribute : PropertyAttribute
    {
        public float top = 3;
        public float right = -3;
        public float bottom = 3;
        public float left = -15;
    }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(BoxAttribute))]
    public class BoxAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUIUtils.Box(EditorGUI.IndentedRect(position));
            var attr = attribute as BoxAttribute;
            position = position.Extended(-attr.top, attr.right, -attr.bottom, attr.left);
            EditorGUI.PropertyField(position, property, new GUIContent(ObjectNames.NicifyVariableName(property.name)), true);
        }
        
        public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
        {
            var attr = attribute as BoxAttribute;
            return EditorGUI.GetPropertyHeight(property, label) + attr.top + attr.bottom;
        }
    }
#endif

}