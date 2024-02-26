using UnityEngine;
using System;
using Npu.Helper;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Npu
{

    [Serializable]
    public class DataConverter
    {
        public Type type;
        public string format = "{0}";
        public double defaultValue;

        public object Convert(object data)
        {
            return Convert(data, type);
        }

        private object Convert(object data, Type dtype)
        {
            switch (dtype)
            {
                case Type.None:
                    return data;

                case Type.String:
                    return string.Format(format, data);

                case Type.LowerCaseString:
                    return string.Format(format, data).ToLower();

                case Type.UpperCaseString:
                    return string.Format(format, data).ToUpper();

                case Type.CapitalizedString:
                    return string.Format(format, data);
                
                case Type.Int:
                {
                    if (data?.GetType() == typeof(int)) return data;
                    var str = $"{data}";
                    return int.TryParse(str, out var v) ? v : (int) defaultValue;
                }

                case Type.Long:
                {
                    if (data?.GetType() == typeof(long)) return data;
                    var str = $"{data}";
                    return long.TryParse(str, out var v) ? v : (long) defaultValue;
                }

                case Type.Float:
                {
                    if (data?.GetType() == typeof(float)) return data;
                    var str = $"{data}";
                    return float.TryParse(str, out var v) ? v : (float) defaultValue;
                }

                case Type.Double:
                {
                    if (data?.GetType() == typeof(double)) return data;
                    var str = $"{data}";
                    return double.TryParse(str, out var v) ? v : defaultValue;
                }

                case Type.Bool:
                {
                    var type = data?.GetType();
                    if (type == typeof(bool)) return data;
                    if (type?.IsNumericType() ?? false) return (int) data != 0;

                    if (type != typeof(string)) return data != null;
                    var str = $"{data}";
                    return int.TryParse(str, out var v) ? (v != 0) : (int) defaultValue != 0;

                }

                case Type.NotBool:
                {
                    return !(bool) Convert(data, Type.Bool);
                }

                default: return data;
            }

        }

        public DataConverter Clone()
        {
            return new DataConverter
            {
                type = type,
                format = (string) format.Clone(),
                defaultValue = defaultValue
            };
        }

        public enum Type
        {
            None,
            String,
            Int,
            Long,
            Float,
            Double,
            Bool,
            NotBool,
            LowerCaseString,
            UpperCaseString,
            CapitalizedString
        }
    }



#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DataConverter))]
    public class ConverterDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var lw = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 90;

            position.width *= 0.5f;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("type"), new GUIContent("Convert"));

            position.x += position.width;
            var type = property.FindPropertyRelative("type").intValue;
            if (type == (int) DataConverter.Type.String
                || type == (int) DataConverter.Type.LowerCaseString
                || type == (int) DataConverter.Type.UpperCaseString
                || type == (int) DataConverter.Type.CapitalizedString
            )
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative("format"), new GUIContent("Format"));
            }
            else if (type != 0)
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative("defaultValue"),
                    new GUIContent("Default"));
            }

            EditorGUIUtility.labelWidth = lw;
            EditorGUI.EndProperty();
        }
    }
#endif

}