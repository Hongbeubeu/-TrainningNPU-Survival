using UnityEditor;
using System;

namespace Npu
{

    [CustomEditor(typeof(EventListener))]
    public class EventListenerEditor : Editor
    {
        EventListener listener;

        private void OnEnable()
        {
            listener = target as EventListener;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var options = Enum.GetNames(typeof(EventType));
            var selected = Array.IndexOf(options, listener.eventName);
            var selection = EditorGUILayout.Popup("Event", selected, options);
            if (selection != selected && selection >= 0)
            {
                listener.eventName = options[selection];
                EditorUtility.SetDirty(listener);
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("priority"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("handler"));

            serializedObject.ApplyModifiedProperties();
        }
    }

}