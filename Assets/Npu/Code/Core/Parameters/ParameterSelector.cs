using System;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using Npu.EditorSupport;
#endif

namespace Npu.Formula
{
    [Serializable]
    public class ParameterSelector
    {
        [TypeConstraint(typeof(IParameterContainer)), SerializeField] public Object target;
        [SerializeField] public string key;

        private IParameterContainer container;
        public IParameterContainer Container => container ?? (container = target as IParameterContainer);

        private IParameter value;
        public IParameter Value => value ?? (value = Container?.Get(key)); 
    }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ParameterSelector))]
    public class ParameterSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            using (new LabelWidth(80))
            {
                position = EditorGUI.PrefixLabel(position, label);
            }

            var targetProp = property.FindPropertyRelative("target");
            var keyProp = property.FindPropertyRelative("key");

            var rect = position;

            rect.width = 0.7f * position.width;
            EditorGUI.PropertyField(rect, targetProp, GUIContent.none);

            // rect.y += rect.height;
            if (targetProp.objectReferenceValue is IParameterContainer target)
            {
                rect.x += rect.width;
                rect.width = position.width * 0.3f;
                
                var keys = target.Keys;
                var key = keyProp.stringValue;
                var selected = Array.IndexOf(keys, key);
                var selection = EditorGUI.Popup(rect, selected, keys);
                if (selected != selection)
                {
                    keyProp.stringValue = keys[selection];
                }
            }
            else
            {
                
            }

            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }
    }
#endif

}