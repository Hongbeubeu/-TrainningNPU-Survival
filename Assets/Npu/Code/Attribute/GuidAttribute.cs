using UnityEngine;
#if UNITY_EDITOR
using Npu.Helper;
using UnityEditor;
#endif

namespace Npu
{
    public class GuidAttribute : PropertyAttribute
    {
        public readonly int length;

        public GuidAttribute(int length = 8)
        {
            this.length = length;
        }
    }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(GuidAttribute))]
    public class GuidAttributeDrawer : PropertyDrawer
    {
        private int Length => (attribute as GuidAttribute).length;
        
    
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Context(position, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var guid = property.stringValue;
            if (string.IsNullOrEmpty(guid))
            {
                guid = Utils.NewGuid(Length);
            }
            GUI.enabled = false;
            property.stringValue = EditorGUI.TextField(position, guid);
            GUI.enabled = true;

            property.serializedObject.ApplyModifiedProperties();

            EditorGUI.EndProperty();
        }

        void Context(Rect rect, SerializedProperty property)
        {
            var current = Event.current;

            if (rect.Contains(current.mousePosition) && current.type == UnityEngine.EventType.ContextClick)
            {
                var menu = new GenericMenu();

                menu.AddItem(new GUIContent("Reset"), false, 
                    () =>
                    {
                        property.stringValue = Utils.NewGuid(Length);
                        property.serializedObject.ApplyModifiedProperties();
                    });
                menu.ShowAsContext();

                current.Use();
            }
        }
    }
#endif
}