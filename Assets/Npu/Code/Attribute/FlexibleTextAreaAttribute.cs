using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

namespace Npu
{

    public class FlexibleTextAreaAttribute : PropertyAttribute
    {
        public readonly int maxLines;

        public FlexibleTextAreaAttribute(int maxLines=2)
        {
            this.maxLines = maxLines;
        }
    }


#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(FlexibleTextAreaAttribute))]
    public class FlexibleTextAreaAttributeDrawer : PropertyDrawer
    {
        private FlexibleTextAreaAttribute Attribute => attribute as FlexibleTextAreaAttribute;
        private int MaxLines => Attribute.maxLines;
        private int ActualLines(string text) => (text?.Count(i => i == '\n') ?? 0) + 1;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var h0 = EditorGUIUtility.singleLineHeight;
            var h = position.height;

            position.height = h0;
            position = EditorGUI.PrefixLabel(position, label);

            position.height = h;
            property.stringValue = EditorGUI.TextArea(position, property.stringValue);

            property.serializedObject.ApplyModifiedProperties();
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var h0 = EditorGUIUtility.singleLineHeight;
            return h0 * Mathf.Min(MaxLines, ActualLines(property.stringValue));
        }
    }
#endif

}