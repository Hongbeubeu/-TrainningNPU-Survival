#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using Npu.Helper;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Npu.EditorSupport
{
    public static class SerializedUtils
    {
        public static void LayoutVisibleChildren(this SerializedProperty property)
        {
            using (new VerticalLayout())
            {
                foreach (var i in property.EnumerateVisibleChildren())
                {
                    EditorGUILayout.PropertyField(i);
                }
            }
        }

        public static void LayoutChildren(this SerializedProperty property)
        {
            using (new VerticalLayout())
            {
                foreach (var i in property.EnumerateChildren())
                {
                    EditorGUILayout.PropertyField(i, true);
                }
            }
        }

        public static SerializedProperty GetSiblingProperty(SerializedProperty prop, string siblingRelativePath)
        {
            var path = prop.propertyPath;
            var lastDot = path.LastIndexOf('.');
            var parentPath = lastDot >= 0 ? (path.Substring(0, lastDot) + ".") : "";
            var siblingPath = parentPath + siblingRelativePath;
            var so = prop.serializedObject;
            return so.FindProperty(siblingPath);
        }

        public static void ForeachArrayElement(SerializedProperty arrProp, System.Action<SerializedProperty, int> action)
        {
            var count = 0;
            var endProp = arrProp.GetEndProperty();
            var eleProp = arrProp.Copy();
            eleProp.Next(true);
            eleProp.Next(true);
            eleProp.Next(false);
            while (!SerializedProperty.EqualContents(eleProp, endProp))
            {
                action?.Invoke(eleProp, count);
                if (!eleProp.Next(false)) break;
                count++;
            }
        }

        public static int PropertyLayoutWithControls(this SerializedProperty property, params string[] buttons)
            => property.PropertyLayoutWithControls(buttons.Select(i => new GUIContent(i)).ToArray());

        public static int PropertyLayoutWithControls(this SerializedProperty property, params GUIContent[] buttons)
        {
            var clicked = -1;
            using (new VerticalLayout())
            {
                using (new HorizontalLayout())
                using (new Indent())
                {
                    property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, property.displayName, true);
                    if (!property.isExpanded) return clicked;

                    GUILayout.FlexibleSpace();

                    using (new GuiColor(Color.cyan))
                    {
                        buttons.ForEach((c, i) =>
                        {
                            if (GUILayout.Button(c)) clicked = i;
                        });
                    }
                }

                using (new Indent()) property.LayoutChildren();
            }

            return clicked;
        }

        public static int PropertyLayoutWithDeleteControl(this SerializedProperty property)
            => property.PropertyLayoutWithControls(new GUIContent("x", "Delete this entry"));

        public static int PropertyLayoutWithOrderControls(this SerializedProperty property)
            => property.PropertyLayoutWithControls(
                new GUIContent("↑", "Move Up"),
                new GUIContent("↓", "Move Down"));

        public static int PropertyLayoutWithOrderAndDeleteControls(this SerializedProperty property)
            => property.PropertyLayoutWithControls(
                new GUIContent("↑", "Move Up"),
                new GUIContent("↓", "Move Down"),
                new GUIContent("x", "Delete this entry"));


        public static IEnumerable<SerializedProperty> EnumerateSerializedProperties(this Object @object)
        {
            return new SerializedObject(@object).EnumerateSerializedProperties();
        }

        public static IEnumerable<SerializedProperty> EnumerateSerializedProperties(this SerializedObject serializedObject)
        {
            var iter = serializedObject.GetIterator();
            while (iter.Next(true))
            {
                yield return iter;
            }
        }

        public static IEnumerable<SerializedProperty> EnumerateArrayElements(this SerializedProperty serializedProperty)
        {
            for (var i = 0; i < serializedProperty.arraySize; i++)
            {
                yield return serializedProperty.GetArrayElementAtIndex(i);
            }
        }

        public static IEnumerable<SerializedProperty> EnumerateChildren(this SerializedProperty property)
        {
            var currentProperty = property.Copy();
            var nextSiblingProperty = property.Copy();
            {
                nextSiblingProperty.Next(false);
            }

            if (!currentProperty.Next(true)) yield break;
            do
            {
                if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                    break;

                yield return currentProperty;
            } while (currentProperty.Next(false));
        }

        /// <summary>
        /// Gets visible children of `SerializedProperty` at 1 level depth.
        /// </summary>
        /// <param name="property">Parent `SerializedProperty`.</param>
        /// <returns>Collection of `SerializedProperty` children.</returns>
        public static IEnumerable<SerializedProperty> EnumerateVisibleChildren(this SerializedProperty property)
        {
            var currentProperty = property.Copy();
            var nextSiblingProperty = property.Copy();
            {
                nextSiblingProperty.NextVisible(false);
            }

            if (!currentProperty.NextVisible(true)) yield break;
            do
            {
                if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                    break;

                yield return currentProperty;
            } while (currentProperty.NextVisible(false));
        }

        public static bool PropertyExpandedGui(this SerializedObject serializedObject, string prop, string label)
        {
            var property = serializedObject.FindProperty(prop);
            return property.PropertyExpandedGui(label);
        }

        public static bool PropertyExpandedGui(this SerializedProperty property, string label)
        {
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, label, true);
            return property.isExpanded;
        }

        public static object GetParent(this SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = obj.GetFieldOrPropertyValue(elementName, index);
                }
                else
                {
                    obj = obj.GetFieldOrPropertyValue(element);
                }
            }

            return obj;
        }
    }
}

#endif