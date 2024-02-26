using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
using System;
#endif

namespace Npu
{
    public class SortingLayerAttribute : PropertyAttribute {}
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SortingLayerAttribute))]
    public class SortingLayerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var sortingLayerNames = SortingLayer.layers.Select(l => l.name).ToArray();
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.HelpBox(position, string.Format("{0} is not an integer but has [SortingLayer].", property.name), MessageType.Error);
            }
            else if (sortingLayerNames != null)
            {
                EditorGUI.BeginProperty(position, label, property);

                // Look up the layer name using the current layer ID
                var oldName = SortingLayer.IDToName(property.intValue);

                // Use the name to look up our array index into the names list
                var oldLayerIndex = Array.IndexOf(sortingLayerNames, oldName);

                // Show the popup for the names
                var newLayerIndex = EditorGUI.Popup(position, label.text, oldLayerIndex, sortingLayerNames);

                // If the index changes, look up the ID for the new index to store as the new ID
                if (newLayerIndex != oldLayerIndex)
                {
                    property.intValue = SortingLayer.NameToID(sortingLayerNames[newLayerIndex]);
                }

                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.BeginProperty(position, label, property);
                var newValue = EditorGUI.IntField(position, label.text, property.intValue);
                if (newValue != property.intValue)
                {
                    property.intValue = newValue;
                }
                EditorGUI.EndProperty();
            }
        }
    }
#endif
}








