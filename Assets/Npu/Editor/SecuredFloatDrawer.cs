using UnityEngine;
using UnityEditor;

namespace Npu.Core
{

    [CustomPropertyDrawer(typeof(SecuredFloat))]
    public class SecuredFloatDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SecuredTypeDrawer.DrawSecuredFloat(position, property, label, true, true);
        }
    }
}