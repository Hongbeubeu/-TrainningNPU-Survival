using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
using System;
#endif

namespace Npu
{
    public class SceneSelectorAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SceneSelectorAttribute))]
    public class SceneSelectorAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var value = ReplaceSlash(property.stringValue);
            var scenes = EditorBuildSettings.scenes.Select(i => ReplaceSlash(i.path)).ToArray();
            var selected = Array.IndexOf(scenes, value);
            var selection = EditorGUI.Popup(position, "Game Scene", selected, scenes);
            if (selected != selection)
            {
                property.stringValue = ReverseSlash(scenes[selection]);
                property.serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUI.EndProperty();
        }
        
        public static string ReplaceSlash(string text_)
        {
            return text_.Replace('/', '\u2215');
        }
 
        public static string ReverseSlash(string text_)
        {
            return text_.Replace('\u2215', '/');
        }
    }
#endif
}