using UnityEngine;
using UnityEditor;

namespace Npu.Core
{

    [CustomPropertyDrawer(typeof(SecuredLong))]
    public class SecuredLongDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SecuredTypeDrawer.DrawSecuredLong(position, property, label, true, true);
        }
    }

}