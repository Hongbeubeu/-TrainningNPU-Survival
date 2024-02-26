using System;
using UnityEngine;
#if UNITY_EDITOR
using Npu.Helper;
using UnityEditor;
using Npu.EditorSupport;
#endif


namespace Npu
{
    public class CreateInnerAssetAttribute : PropertyAttribute
    {
        public CreateInnerAssetAttribute(Type type=null)
        {
            baseType = type;
        }
        
        public Type baseType;
    }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(CreateInnerAssetAttribute))]
    public class CreateInnerAssetAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            EditorGUIUtils.Box(position);
            
            position = position.Extended(-3, -3, -3, -5);

            position.width -= 50;
            EditorGUI.PropertyField(position, property, label);

            position.x += position.width + 10;
            position.width = 40;
            if (GUI.Button(position, "..."))
            {
                var ctx = new GenericMenu();
                
                ctx.AddItem(new GUIContent("Create"), false, () => { });
                ctx.AddSeparator("");

                var attr = attribute as CreateInnerAssetAttribute;
                var baseType = attr?.baseType ?? fieldInfo.FieldType;
                
                var types = baseType.GetDerived();

                foreach (var i in types)
                {
                    ctx.AddItem(new GUIContent(i.Name), false, () =>
                    {
                        AssetUtils.CreateChildAsset(property, i);
                    });
                }
                
                ctx.ShowAsContext();
            }
            
            EditorGUI.EndProperty();
            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label) + 6;
        }
    }
    

#endif
}