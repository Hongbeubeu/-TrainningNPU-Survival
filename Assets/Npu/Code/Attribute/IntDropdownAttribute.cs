using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

namespace Npu
{
    public class IntDropdownAttribute : PropertyAttribute
    {
        public int min, max;

        public IntDropdownAttribute(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
    }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(IntDropdownAttribute))]
    public class IntDropdownAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var att = attribute as IntDropdownAttribute;

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var v = property.intValue;

            var range = Enumerable.Range(att.min, att.max);
            var selected = v - att.min;
            var options = range.Select(i => i.ToString()).ToArray();

            var selection = EditorGUI.Popup(position, selected, options);
            if (selected != selection)
            {
                property.intValue = selection + att.min;
            }

            EditorGUI.EndProperty();
        }
    }
#endif
    
}