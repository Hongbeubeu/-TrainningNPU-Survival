
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Npu
{
    public class DropdownAttribute : PropertyAttribute
    {
        public string[] options;
        
        public DropdownAttribute(params string[] options)
        {
            this.options = options;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DropdownAttribute))]
    public class DropdownAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var att = attribute as DropdownAttribute;

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var v = property.stringValue;
            var options = att.options;
            var selected = Array.IndexOf(options, v);
            
            var selection = EditorGUI.Popup(position, selected, options);
            if (selected != selection)
            {
                property.stringValue = options[selection];
            }

            EditorGUI.EndProperty();
        }
    }
#endif
}