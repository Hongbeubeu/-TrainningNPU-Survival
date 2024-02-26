using System;
using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

namespace Npu
{
    [Serializable]
    public class TimeSpanPicker
    {
        [SerializeField] private long ticks;

        public long Ticks => ticks;
        public double Seconds => (double)ticks/TimeSpan.TicksPerSecond;
        public TimeSpan TimeSpan => TimeSpan.FromTicks(ticks);
        
        public static implicit operator TimeSpanPicker(double seconds) => new TimeSpanPicker{ticks = (long)(seconds * TimeSpan.TicksPerSecond)};
        
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(TimeSpanPicker))]
    public class TimeSpanPickerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var ticksProp = property.FindPropertyRelative("ticks");
            var ticks = ticksProp.longValue;
            
            var timeSpan = TimeSpan.FromTicks(ticks);
            var days = timeSpan.Days;
            var hours = timeSpan.Hours;
            var minutes = timeSpan.Minutes;
            var seconds = timeSpan.Seconds;

            position.width /= 4;
            position.width -= 5;

            var style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
            var r = position;
            r.width = 5;
            
            days = EditorGUI.IntField(position, days);
            r.x = position.x + position.width;
            EditorGUI.LabelField(r, ".", style);

            var range = Enumerable.Range(0, 24).Select(i => i.ToString()).ToArray();
            position.x = r.x + r.width;
            hours = EditorGUI.Popup(position, hours, range);
            r.x = position.x + position.width;
            EditorGUI.LabelField(r, ":", style);
            
            range = Enumerable.Range(0, 60).Select(i => i.ToString()).ToArray();
            position.x = r.x + r.width;
            minutes = EditorGUI.Popup(position, minutes, range);
            r.x = position.x + position.width;
            EditorGUI.LabelField(r, ":", style);
            
            position.x = r.x + r.width;
            seconds = EditorGUI.Popup(position, seconds, range);
            
            ticksProp.longValue = new TimeSpan(days, hours, minutes, seconds).Ticks;
            
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }
    }
    
#endif
}