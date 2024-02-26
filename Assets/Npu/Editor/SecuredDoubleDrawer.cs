using UnityEngine;
using UnityEditor;

namespace Npu.Core
{

    [CustomPropertyDrawer(typeof(SecuredDouble))]
    public class SecuredDoubleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SecuredTypeDrawer.DrawSecuredDouble(position, property, label, true, true);
        }
    }
}