using System;
using System.Linq;
using Npu.EditorSupport;
using Npu.Helper;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Npu.Common
{

    public class TypeSelectorAttribute : PropertyAttribute
    {
        public Type parent;

        public TypeSelectorAttribute(Type t=null)
        {
            parent = t;
        }
    }
    
    [Serializable]
    public class TypeSelector
    {
        public string name;

        public Type Type => Type.GetType(name);
    }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(TypeSelector))]
    public class TypeSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var types = ReflectionUtils.GetAllTypes();
            var options = types.Select(i => i.FullName).ToArray();
            EditorGUI.Popup(position, -1, options);
        }
    }
    
    [CustomPropertyDrawer(typeof(TypeSelectorAttribute))]
    public class TypeSelectorAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            position = EditorGUI.PrefixLabel(position, label);

            var (r1, r2) = position.HFixedSplit(position.width - 25);

            EditorGUI.TextField(r1, property.stringValue);

            using (new GuiColor(Color.cyan))
            if (GUI.Button(r2, "..."))
            {
                var at = attribute as TypeSelectorAttribute;
                var types = at.parent == null ? ReflectionUtils.GetAllTypes() : ReflectionUtils.GetAllTypes(at.parent);
                var options = types.Select(i => i.FullName).ToArray();
                
                var menu = new GenericMenu();
                foreach (var s in options)
                {
                    menu.AddItem(new GUIContent(s), s == property.stringValue, data =>
                    {
                        property.stringValue = (string) data;
                        property.serializedObject.ApplyModifiedProperties();
                    }, s);
                }
                
                menu.ShowAsContext();
            }
            
            
            EditorGUI.EndProperty();
        }
    }
#endif
}