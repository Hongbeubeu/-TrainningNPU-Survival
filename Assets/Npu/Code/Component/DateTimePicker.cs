using UnityEngine;
using System;
using System.Linq;
using Npu.Helper;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Npu
{

    [Serializable]
    public class DateTimePicker
    {
        [SerializeField] private long ticks;

        public long Ticks => ticks;
        public double Seconds => (double) ticks / TimeSpan.TicksPerSecond;
        public DateTime DateTime => TimeUtils.Epoch + TimeSpan.FromTicks(ticks);
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DateTimePicker))]
    public class DateTimeAttributeDrawer : PropertyDrawer
    {

        private static string[] Months = 
            { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.BeginProperty(position, label, property);

            position.height /= 2;
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var ticksProp = property.FindPropertyRelative("ticks");
            var ticks = ticksProp.longValue;
            if (ticks == 0)
            {
                ticks = TimeUtils.CurrentTicks;
            }

            var timeSpan = TimeSpan.FromTicks(ticks);
            var date = timeSpan.Days * TimeSpan.TicksPerDay;
            var time = ticks - date;

            date = DateGUI(position, date);
            position.y += position.height;
            time = TimeGUI(position, time);
            
            ticksProp.longValue = date + time;
            
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

        private static long DateGUI(Rect position, long ts)
        {
            position.width /= 3;
            var dateTime = TimeUtils.TicksToDateTime(ts);
            
            var days = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            var range = Enumerable.Range(1, days).ToArray();
            var day = 1 + EditorGUI.Popup(position, Array.IndexOf(range, dateTime.Day),
                range.Select(i => i.ToString()).ToArray());

            position.x += position.width;
            var month = 1 + EditorGUI.Popup(position, dateTime.Month - 1, Months);
            
            position.x += position.width;
            range = Enumerable.Range(2010, 20).ToArray();
            var year = range[0] + EditorGUI.Popup(position, Array.IndexOf(range, dateTime.Year), range.Select(i => i.ToString()).ToArray());
            
            return TimeUtils.DateTimeToTicks(new DateTime(year, month, day));
        }
        
        private static long TimeGUI(Rect position, long ts)
        {
            position.width /= 3;

            var timeSpan = TimeSpan.FromTicks(ts);
            var hours = timeSpan.Hours;
            var mins = timeSpan.Minutes;
            var secs = timeSpan.Seconds;
            
            var range = Enumerable.Range(0, 24).ToArray();
            hours = EditorGUI.Popup(position, (int)hours, range.Select(i => i.ToString()).ToArray());

            position.x += position.width;
            range = Enumerable.Range(0, 60).ToArray();
            mins = EditorGUI.Popup(position, (int)mins, range.Select(i => i.ToString()).ToArray());
            
            position.x += position.width;
            range = Enumerable.Range(0, 60).ToArray();
            secs = EditorGUI.Popup(position, (int)secs, range.Select(i => i.ToString()).ToArray());
            
            return hours * TimeSpan.TicksPerHour + mins * TimeSpan.TicksPerMinute + secs * TimeSpan.TicksPerSecond;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 2;
        }
    }
#endif
}