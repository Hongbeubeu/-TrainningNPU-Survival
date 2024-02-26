using UnityEditor;
using UnityEngine;

namespace Npu.Core
{

    [CustomPropertyDrawer(typeof(SecuredInt))]
    public class SecuredIntDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SecuredTypeDrawer.DrawSecuredInt(position, property, label, true, true);
        }
    }

}