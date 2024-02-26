using Npu.Core;
using UnityEngine;
#if UNITY_EDITOR
using System;
using UnityEditor;

#endif

namespace Npu
{
    public class SecuredTypeAttribute : PropertyAttribute
    {
        public bool showLabel;
        public bool showExtraControl;

        public SecuredTypeAttribute(bool showLabel = true, bool showExtraControl = true)
        {
            this.showLabel = showLabel;
            this.showExtraControl = showExtraControl;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SecuredTypeAttribute))]
    public class SecuredTypeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var att = attribute as SecuredTypeAttribute;
            if (fieldInfo.FieldType == typeof(SecuredDouble))
            {
                DrawSecuredDouble(position, property, label, att.showLabel, att.showExtraControl);
            }
            else if (fieldInfo.FieldType == typeof(SecuredFloat))
            {
                DrawSecuredFloat(position, property, label, att.showLabel, att.showExtraControl);
            }
            else if (fieldInfo.FieldType == typeof(SecuredLong))
            {
                DrawSecuredLong(position, property, label, att.showLabel, att.showExtraControl);
            }
            else if (fieldInfo.FieldType == typeof(SecuredInt))
            {
                DrawSecuredInt(position, property, label, att.showLabel, att.showExtraControl);
            }
            else
            {
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                EditorGUI.LabelField(position, $"Not support {fieldInfo.FieldType}");
            }

            EditorGUI.EndProperty();
        }

        public static void DrawSecuredDouble(Rect position, SerializedProperty property, GUIContent label,
            bool showLabel, bool showExtraControls)
        {
            Func<Rect, SerializedProperty, SerializedProperty, object>
                valueHandler = (Rect rect, SerializedProperty key, SerializedProperty masked) =>
                {
                    return EditorGUI.DoubleField(rect, SecuredDouble.Encrypt(key.longValue, masked.doubleValue));
                };

            Action<SerializedProperty> keyHandler = (SerializedProperty key) =>
            {
                key.longValue = SecuredDouble.RandomKey();
            };

            Action<SerializedProperty, SerializedProperty, object>
                maskedHandler = (SerializedProperty key, SerializedProperty masked, object value) =>
                {
                    masked.doubleValue = SecuredDouble.Encrypt(key.longValue, (double) value);
                };

            Draw(position, property, label, showLabel, showExtraControls, valueHandler, keyHandler, maskedHandler);
        }

        public static void DrawSecuredFloat(Rect position, SerializedProperty property, GUIContent label,
            bool showLabel, bool showExtraControls)
        {
            Func<Rect, SerializedProperty, SerializedProperty, object>
                valueHandler = (Rect rect, SerializedProperty key, SerializedProperty masked) =>
                {
                    return EditorGUI.FloatField(rect, SecuredFloat.Encrypt(key.longValue, masked.floatValue));
                };

            Action<SerializedProperty> keyHandler = (SerializedProperty key) =>
            {
                key.longValue = SecuredDouble.RandomKey();
            };

            Action<SerializedProperty, SerializedProperty, object>
                maskedHandler = (SerializedProperty key, SerializedProperty masked, object value) =>
                {
                    masked.floatValue = SecuredFloat.Encrypt(key.longValue, (float) value);
                };

            Draw(position, property, label, showLabel, showExtraControls, valueHandler, keyHandler, maskedHandler);
        }

        public static void DrawSecuredLong(Rect position, SerializedProperty property, GUIContent label, bool showLabel,
            bool showExtraControls)
        {
            Func<Rect, SerializedProperty, SerializedProperty, object>
                valueHandler = (Rect rect, SerializedProperty key, SerializedProperty masked) =>
                {
                    return EditorGUI.LongField(rect, SecuredLong.Encrypt(key.longValue, masked.longValue));
                };

            Action<SerializedProperty> keyHandler = (SerializedProperty key) =>
            {
                key.longValue = SecuredDouble.RandomKey();
            };

            Action<SerializedProperty, SerializedProperty, object>
                maskedHandler = (SerializedProperty key, SerializedProperty masked, object value) =>
                {
                    masked.longValue = SecuredLong.Encrypt(key.longValue, (long) value);
                };

            Draw(position, property, label, showLabel, showExtraControls, valueHandler, keyHandler, maskedHandler);
        }

        public static void DrawSecuredInt(Rect position, SerializedProperty property, GUIContent label, bool showLabel,
            bool showExtraControls)
        {
            Func<Rect, SerializedProperty, SerializedProperty, object>
                valueHandler = (Rect rect, SerializedProperty key, SerializedProperty masked) =>
                {
                    return EditorGUI.IntField(rect, SecuredInt.Encrypt(key.intValue, masked.intValue));
                };

            Action<SerializedProperty> keyHandler = (SerializedProperty key) =>
            {
                key.intValue = SecuredInt.RandomKey();
            };

            Action<SerializedProperty, SerializedProperty, object>
                maskedHandler = (SerializedProperty key, SerializedProperty masked, object value) =>
                {
                    masked.intValue = SecuredInt.Encrypt(key.intValue, (int) value);
                };

            Draw(position, property, label, showLabel, showExtraControls, valueHandler, keyHandler, maskedHandler);
        }

        public static void Draw(Rect position, SerializedProperty property, GUIContent label,
            bool showLabel,
            bool showExtraControls,
            Func<Rect, SerializedProperty, SerializedProperty, object> valueHandler,
            Action<SerializedProperty> keyHandler,
            Action<SerializedProperty, SerializedProperty, object> maskedHandler
        )
        {
            if (property.serializedObject.isEditingMultipleObjects)
            {
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                GUI.enabled = false;
                EditorGUI.TextField(position, "--");
                GUI.enabled = true;
                return;
            }

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            if (showLabel)
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var valueRect = new Rect(position.x, position.y, position.width * 0.7f, position.height);
            var formatRect = new Rect(position.x + position.width * 0.7f, position.y, position.width * 0.3f,
                position.height);
            formatRect.x += 5;
            formatRect.width -= 5;

            if (!showExtraControls) valueRect.width = position.width;

            var maskedProp = property.FindPropertyRelative("masked");
            var keyProp = property.FindPropertyRelative("key");

            var key = keyProp.longValue;
            var value = valueHandler.Invoke(valueRect, keyProp, maskedProp);

            if (showExtraControls && GUI.Button(formatRect, "↑key↓") || key == 0)
            {
                keyHandler.Invoke(keyProp);
            }

            maskedHandler.Invoke(keyProp, maskedProp, value);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
#endif
}