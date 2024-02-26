using System;
using UnityEngine;
using UnityEngine.Events;

namespace Npu
{

    public class EventListener : MonoBehaviour
    {
        public string eventName;
        [IntDropdown(0, 11)] public int priority;
        public Listener handler;

        EventType eventType;

        void OnEnable()
        {
            var names = Enum.GetNames(typeof(EventType));
            var index = Array.IndexOf(names, eventName);
            if (index < 0)
            {
                eventType = EventType.None;
            }
            else
            {
                eventType = (EventType) Enum.GetValues(typeof(EventType)).GetValue(index);
            }

            if (eventType != EventType.None)
            {
                EventDispatcher.Instance.AddListener(eventType, OnEvent, priority);
            }
        }

        void OnDisable()
        {
            if (eventType != EventType.None)
            {
                EventDispatcher.Instance.RemoveListener(eventType, OnEvent);
            }
        }

        void OnEvent(EventType key, object data)
        {
            handler?.Invoke(key, data);
        }

        [Serializable]
        public class Listener : UnityEvent<EventType, object>
        {
        }
    }

}