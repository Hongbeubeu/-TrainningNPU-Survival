using System.Collections.Generic;
using System.Linq;
using Npu.Helper;
using UnityEngine;

#if UNITY_EDITOR
using  UnityEditor;
using Npu.EditorSupport;
#endif

namespace Npu
{
    public class GroupAttribute : PropertyAttribute
    {
        public string name;
        public string[] siblings;

        public GroupAttribute(string name, params string[] siblings)
        {
            this.name = name;
            this.siblings = siblings;
        }
    }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(GroupAttribute))]
    public class GroupAttributeDrawer : PropertyDrawer
    {
        private const float Spacing = 2f;
        private GroupAttribute Attr => attribute as GroupAttribute;

        private string PrefKey(SerializedProperty property) => $"{property.serializedObject.targetObject.GetType().Name}-{property.propertyPath}-{Attr.name}";
        private bool Expanded(SerializedProperty property) => EditorPrefs.GetBool(PrefKey(property));
        private void SetExpanded(SerializedProperty property, bool value) => EditorPrefs.SetBool(PrefKey(property), value);
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var expanded = Expanded(property);

            var rect = position;
            rect.height = base.GetPropertyHeight(property, label);
            rect = rect.Indented(0.5f);

            var style = new GUIStyle(EditorStyles.foldout) {fontStyle = FontStyle.Bold};
            var expansion = EditorGUI.Foldout(rect, expanded, Attr.name, true, style);
            if (expanded != expansion) SetExpanded(property, expansion);
            
            if (!expansion) return;

            position = position.Extended(-rect.height, 0, 0, 0);
            // position.y += rect.height;
            // position.height -= rect.height;

            EditorGUIUtils.Box(position);
            position = position.Extended(-Spacing, -Spacing, -Spacing, -Spacing);
            
            var siblings = Siblings(property);
            siblings.Insert(0, property);
            foreach (var i in siblings)
            {
                var h = EditorGUI.GetPropertyHeight(i);
                position.height = h;
                EditorGUI.PropertyField(position, i);
                position.y += h + Spacing;
            }

        }
        
        private List<SerializedProperty> Siblings(SerializedProperty property)
        {
            var siblings = new List<SerializedProperty>();
            foreach (var i in Attr.siblings)
            {
                var p = property.serializedObject.FindProperty(i);
                if (p != null) siblings.Add(p);
                else Logger.Error<GroupAttribute>($"No field {i} in property {property.displayName} of {property.GetParent()}");
            }
            return siblings;
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var baseH = base.GetPropertyHeight(property, label);
            if (!Expanded(property)) return baseH;
            
            return baseH + EditorGUI.GetPropertyHeight(property) + 2 * Spacing
                + Siblings(property).Sum(i => EditorGUI.GetPropertyHeight(i) + Spacing);
        }
    }
#endif
}