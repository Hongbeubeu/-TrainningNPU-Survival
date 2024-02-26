using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Npu.Common
{
    public interface ISelector<T>
    {
        string[] GetIdentifiers();
        T Select(string id);
    }
    
    public class PropertyIdentifierAttribute : Attribute
    {
        public string name;
    }
    
    public abstract class Property<TContainer, TProperty>
    {
        public abstract TProperty Value { get; }

        public static string[] Properties
            => typeof(TContainer).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(i => typeof(TProperty).IsAssignableFrom(i.PropertyType))
                .Select(i => i.Name).ToArray();
    }

    [System.Serializable]
    public abstract class PropertySelector<TContainer, TProperty>
    {
        public string name;

        public abstract TProperty Value { get; }

        PropertyInfo property;
        public PropertyInfo Property
        {
            get
            {
                if (property == null)
                {
                    property = Pairs.FirstOrDefault(i => (i.Item2.name ?? i.i.Name).Equals(name)).i;
                    if (property == null)
                        Debug.LogErrorFormat("Property {0} not found in {1}", name, typeof(TContainer));
                }

                return property;
            }
        }

        public static string[] Identifiers
        {
            get => Pairs.Select(i => i.Item2.name ?? i.i.Name).ToArray();
        }

        static IEnumerable<(PropertyInfo i, PropertyIdentifierAttribute)> Pairs
        {
            get => typeof(TContainer).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Select(i => (i, i.GetCustomAttribute(typeof(PropertyIdentifierAttribute)) as PropertyIdentifierAttribute))
                    .Where(i => i.Item2 != null);
        }
    }

#if UNITY_EDITOR
    public class PropertySelectorDrawer<TContainer, TProperty> : StringDropdown
    {
        public override string[] Options => PropertySelector<TContainer, TProperty>.Identifiers;
    }
    
    public class PropertyDrawer<TContainer, TProperty> : StringDropdown
    {
        public override string[] Options => Property<TContainer, TProperty>.Properties;
    }

    public abstract class StringDropdown : PropertyDrawer
    {
        public abstract string[] Options { get; }
        public virtual string PropertyName => "name";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var prop = property.FindPropertyRelative(PropertyName);
            var options = Options;

            var selected = System.Array.IndexOf(options, prop.stringValue);
            var selection = EditorGUI.Popup(position, selected, options);
            if (selected != selection)
            {
                prop.stringValue = options[selection];
            }

            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }
    }
#endif


}