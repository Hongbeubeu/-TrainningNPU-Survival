using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Npu
{
    public class IndentAttribute : PropertyAttribute 
    {
        public readonly int indent;
        public IndentAttribute(int indent=1)
        {
            this.indent = indent;
        }
    }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(IndentAttribute))]
    public class IndentAttributeDrawer : PropertyDrawer
    {
        const float IndentWidth = 20;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var att = attribute as Npu.IndentAttribute;
            position.width -= att.indent * IndentWidth;
            position.x += att.indent * IndentWidth;
            EditorGUI.PropertyField(position, property, label);
            
            EditorGUI.EndProperty();
        }
    }
#endif
}