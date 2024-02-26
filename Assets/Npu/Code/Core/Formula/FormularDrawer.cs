using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Linq;
using Npu.Core;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

namespace Npu.Formula
{

    public class FormulaDrawerAttribute : PropertyAttribute
    {
        public Type parentType;
        public bool lineBreak;
        public string label;
        public float fixedWidth = -1;
        public int indent;
        public string[] tokens;

        public FormulaDrawerAttribute(Type parentType, string label, string formular, bool lineBreak = false,
            int indent = 0, float fixedWidth = -1)
        {
            this.parentType = parentType;
            this.label = label;
            this.lineBreak = lineBreak;
            this.indent = indent;
            this.fixedWidth = fixedWidth;
            this.tokens = Tokenize(formular).ToArray();
        }

        static List<char> variable_chars =
            Enumerable.Range((int) 'a', (int) 'z').Select(i => Convert.ToChar(i))
                .Union(Enumerable.Range((int) 'A', (int) 'Z').Select(i => Convert.ToChar(i)))
                .Union(Enumerable.Range((int) '0', (int) '9').Select(i => Convert.ToChar(i)))
                .Union(new List<char> {'[', ']', '_'})
                .ToList();

        static List<string> stops = new List<string> {"[F]"};

        public static List<string> Tokenize(string input)
        {
            var tokens = new List<string>();
            var stringBuilder = new StringBuilder();
            var t = 0;
            foreach (var i in input)
            {
                var e = variable_chars.Contains(i);
                switch (t)
                {
                    case 0:
                        t = stops.Any(s => s[0] == i) ? 1 : 3;
                        stringBuilder.Append(i);
                        break;

                    case 1:
                        if (e)
                        {
                            stringBuilder.Append(i);
                            var s = stringBuilder.ToString();
                            if (stops.Contains(s)) t = 2;
                        }
                        else
                        {
                            throw new Exception("Invalid char " + i);
                        }

                        break;

                    case 2:
                        if (e)
                        {
                            stringBuilder.Append(i);
                        }
                        else
                        {
                            tokens.Add(stringBuilder.ToString());
                            stringBuilder.Clear();
                            stringBuilder.Append(i);
                            t = 3;
                        }

                        break;

                    case 3:
                        if (stops.Any(s => s[0] == i))
                        {
                            tokens.Add(stringBuilder.ToString());
                            stringBuilder.Clear();
                            t = 1;
                        }

                        stringBuilder.Append(i);

                        break;
                }

            }

            tokens.Add(stringBuilder.ToString());

            return tokens;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(FormulaDrawerAttribute))]
    public class FormularDrawerDrawer : PropertyDrawer
    {
        const int InputWidth = 80;
        const int TabWidth = 30;

        public bool LineBreak => (attribute as FormulaDrawerAttribute).lineBreak;
        public int Lines => LineBreak ? 2 : 1;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var info = attribute as FormulaDrawerAttribute;
            EditorGUI.BeginProperty(position, label, property);

            position.height /= Lines;
            position.x += info.indent * 20;

            position = DrawLabel(info.label, position, EditorStyles.boldLabel, info.fixedWidth);
            if (LineBreak)
            {
                position.x = 0;
                position.y += position.height;
            }

            position = DrawLabel(" = ", position);

            foreach (var i in info.tokens)
            {
                position = DrawElement(property.serializedObject, i, position);
            }

            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }



        FieldInfo GetFieldInfo(string name)
        {
            return (attribute as FormulaDrawerAttribute).parentType.GetField(name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        Rect DrawElement(SerializedProperty parent, string text, Rect position)
        {
            if (text.StartsWith("[F]", StringComparison.Ordinal))
            {
                var fieldName = text.Substring(3);
                var field = GetFieldInfo(fieldName);
                if (field == null)
                {
                    Debug.LogErrorFormat("Field '{0}' not found", fieldName);
                    return DrawLabel(text, position);
                }

                if (field.FieldType == typeof(int))
                {
                    return DrawIntField(parent.FindPropertyRelative(fieldName), position);
                }

                if (field.FieldType == typeof(double))
                {
                    return DrawDoubleField(parent.FindPropertyRelative(fieldName), position);
                }

                if (field.FieldType == typeof(SecuredDouble))
                {
                    return DrawSecuredDoubleField(parent.FindPropertyRelative(fieldName), position);
                }

                if (field.FieldType == typeof(SecuredInt))
                {
                    return DrawSecuredIntField(parent.FindPropertyRelative(fieldName), position);
                }
            }

            return DrawLabel(text, position);
        }

        Rect DrawElement(SerializedObject serializedObject, string text, Rect position)
        {
            if (text.StartsWith("[F]", StringComparison.Ordinal))
            {
                var fieldName = text.Substring(3);
                var field = GetFieldInfo(fieldName);
                if (field == null)
                {
                    Debug.LogErrorFormat("Field '{0}' not found", fieldName);
                    return DrawLabel(text, position);
                }

                if (field.FieldType == typeof(int))
                {
                    return DrawIntField(serializedObject.FindProperty(fieldName), position);
                }

                if (field.FieldType == typeof(double))
                {
                    return DrawDoubleField(serializedObject.FindProperty(fieldName), position);
                }

                if (field.FieldType == typeof(SecuredDouble))
                {
                    return DrawSecuredDoubleField(serializedObject.FindProperty(fieldName), position);
                }

                if (field.FieldType == typeof(SecuredInt))
                {
                    return DrawSecuredIntField(serializedObject.FindProperty(fieldName), position);
                }
            }

            return DrawLabel(text, position);
        }

        Rect DrawSecuredDoubleField(SerializedProperty property, Rect position)
        {
            var mask = property.FindPropertyRelative("masked");
            var key = property.FindPropertyRelative("key");
            var value = SecuredDouble.Encrypt(key.longValue, mask.doubleValue);
            position = DrawDoubleField(ref value, position);
            mask.doubleValue = SecuredDouble.Encrypt(key.longValue, value);
            return position;
        }

        Rect DrawSecuredIntField(SerializedProperty property, Rect position)
        {
            var mask = property.FindPropertyRelative("masked");
            var key = property.FindPropertyRelative("key");
            var value = SecuredInt.Encrypt(key.intValue, mask.intValue);
            position = DrawIntField(ref value, position);
            mask.intValue = SecuredInt.Encrypt(key.intValue, value);
            return position;
        }

        Rect DrawIntField(SerializedProperty property, Rect position)
        {
            var v = property.intValue;
            var p = DrawIntField(ref v, position);
            property.intValue = v;

            return p;
        }

        Rect DrawDoubleField(SerializedProperty property, Rect position)
        {
            var v = property.doubleValue;
            var p = DrawDoubleField(ref v, position);
            property.doubleValue = v;

            return p;
        }

        Rect DrawIntField(ref int value, Rect position)
        {
            var rect = position;

            rect.width = InputWidth;
            value = EditorGUI.DelayedIntField(rect, value);

            position.x += rect.width;
            position.width -= rect.width;
            return position;
        }

        Rect DrawDoubleField(ref double value, Rect position)
        {
            var rect = position;

            rect.width = InputWidth;
            value = EditorGUI.DelayedDoubleField(rect, value);

            position.x += rect.width;
            position.width -= rect.width;
            return position;
        }

        Rect DrawLabel(string text, Rect position, GUIStyle style = null, float fixedWidth = -1)
        {
            style = style ?? EditorStyles.label;

            if (fixedWidth < 0)
            {
                var label = new GUIContent(text);
                var size = style.CalcSize(label);
                fixedWidth = size.x;
            }

            var rect = position;
            rect.width = fixedWidth;
            EditorGUI.LabelField(rect, text, style);

            position.x += rect.width;
            position.width -= rect.width;
            return position;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * Lines;
        }
    }
#endif


}