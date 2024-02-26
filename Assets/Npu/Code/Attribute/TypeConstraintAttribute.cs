using UnityEngine;
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using Npu.EditorSupport;
#endif

namespace Npu
{
    public class TypeConstraintAttribute : PropertyAttribute
    {
        public readonly Type[] types;
        public float labelWidth = -1;
        public bool showAsError;

        public TypeConstraintAttribute(params Type[] types)
        {
            this.types = types;
        }
    }


#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(TypeConstraintAttribute))]
    public class TypeConstraintAttributeDrawer : PropertyDrawer
    {
        private bool IsValid(SerializedProperty property) =>
            ((TypeConstraintAttribute) attribute).types.Any(i => i.IsInstanceOfType(property.objectReferenceValue));

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var att = attribute as TypeConstraintAttribute;
            
            var valid = IsValid(property);
            if (!valid)
            {
                label.tooltip = $"{ObjectNames.NicifyVariableName(property.name)} must be of type {att?.types}";
            }
            
            using (new LabelWidth(att.labelWidth > 0 ? att.labelWidth :  EditorGUIUtility.labelWidth))
            using (new GuiColor(valid ? Color.white : att.showAsError ? Color.red : Color.yellow))
            {
                position.width -= 30;
                EditorGUI.ObjectField(position, property, label);
            }
            
            var btnRect = position;
            btnRect.x += position.width + 2;
            btnRect.width = 25;
            Menu(btnRect, property, att.types);

            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

        private static void Menu(Rect rect, SerializedProperty property, Type[] types)
        {
            var value = property.objectReferenceValue ? property.objectReferenceValue : property.serializedObject.targetObject;
            
            var go = value as GameObject ?? (value as Component)?.gameObject;

            if (go != null)
            {
                using (new GuiColor(Color.cyan))
                {
                    if (!GUI.Button(rect, "...")) return;
                }

                var candidates = types.SelectMany(i => go.GetComponentsInChildren(i, true)).ToArray();
                var menu = new GenericMenu();
                    
                foreach (var c in candidates)
                {
                    menu.AddItem(new GUIContent(c.GetType().Name), value == c,
                        () =>
                        {
                            property.objectReferenceValue = c;
                            property.serializedObject.ApplyModifiedProperties();
                        });
                }
                    
                menu.ShowAsContext();
            }
            else
            {
                using (new DisabledGui(true))
                {
                    GUI.Button(rect, "...");
                }
            }
            
            
        }
    }

#endif

}