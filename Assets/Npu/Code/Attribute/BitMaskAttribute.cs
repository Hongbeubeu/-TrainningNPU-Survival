using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Npu
{

    public class BitMaskAttribute : PropertyAttribute
    {
        public System.Type enumType;
        public string label;
        public bool showValue;
        public bool showControl;
        public long mask = 0;

        public string[] Names { get; private set; }

        public int[] Values { get; private set; }

        public BitMaskAttribute(System.Type type, string label = null, bool showValue = false, bool showControl = false,
            params int[] masks)
        {
            enumType = type;
            this.label = label;
            this.showValue = showValue;
            this.showControl = showControl;

            var names = System.Enum.GetNames(enumType).ToList();
            var values = System.Enum.GetValues(enumType).Cast<int>().ToList();

            var ignore = values.Select((v, i) => new {i, v}).Where(o => masks.Contains(o.v)).Select(o => o.i);
            if (ignore.Count() > 0)
            {
                names = names.Select((v, i) => new {i, v}).Where(o => !ignore.Contains(o.i)).Select(o => o.v).ToList();
                values = values.Select((v, i) => new {i, v}).Where(o => !ignore.Contains(o.i)).Select(o => o.v)
                    .ToList();
            }

            Names = names.ToArray();
            Values = values.ToArray();
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(BitMaskAttribute))]
    public class BitMaskPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var attr = attribute as BitMaskAttribute;
            label.text = string.Format("{0} ({1})",
                attr.label ?? label.text,
                string.Format("{0}{1}", Count(prop.longValue, attr.Values),
                    attr.showValue ? " - " + prop.longValue : ""));
            prop.longValue = DrawBitMaskField(position, prop, prop.longValue, attr.Values, attr.Names, label,
                attr.showControl);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var type = attribute as BitMaskAttribute;
            var names = type.Names;
            var rows = 1;
            if (property.isExpanded) rows += Mathf.CeilToInt(names.Length / 2.0f);
            return base.GetPropertyHeight(property, label) * rows;
        }

        public static long DrawBitMaskField(Rect position, SerializedProperty prop, long value, int[] values,
            string[] names, GUIContent label, bool showControl)
        {
            var rows = 1 + (prop.isExpanded ? Mathf.CeilToInt(names.Length / 2.0f) : 0);
            var dy = position.height / rows;
            var rect = position;
            rect.height = dy;
            var newValue = value;

            var r = rect;
            r.width = rect.width / 2;
            prop.isExpanded = EditorGUI.Foldout(r, prop.isExpanded, label, true);

            if (prop.isExpanded && showControl)
            {
                r.width = 40;
                r.x = rect.width - r.width;
                if (GUI.Button(r, "None"))
                {
                    return 0x00;
                }

                r.x = rect.width - 2 * r.width - 5;
                if (GUI.Button(r, "All"))
                {
                    long v = 0;
                    for (var i = 0; i < values.Length; i++)
                    {
                        v |= 1 << values[i];
                    }

                    return v;
                }
            }

            rect.width /= 2;
            if (prop.isExpanded)
            {
                newValue = 0;
                EditorGUI.indentLevel++;
                for (var i = 0; i < names.Length; i++)
                {
                    rect.position = new Vector2(position.position.x + (i % 2) * rect.width,
                        position.position.y + (i / 2 + 1) * dy);
                    var v = EditorGUI.Toggle(rect, ObjectNames.NicifyVariableName(names[i]),
                        (value & (1 << values[i])) != 0);

                    if (v)
                    {
                        newValue |= 1 << values[i];
                    }
                }

                EditorGUI.indentLevel--;
            }

            return newValue;
        }

        int Count(long value, int[] values)
        {
            var count = 0;
            for (var i = 0; i < values.Length; i++)
            {
                if ((value & (1 << values[i])) != 0) count++;
            }

            return count;
        }
    }
#endif

}