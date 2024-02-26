using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Npu
{
    public class InlineAttribute : PropertyAttribute
    {
        
    }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(InlineAttribute))]
    public class InlineAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
    }
#endif    
}