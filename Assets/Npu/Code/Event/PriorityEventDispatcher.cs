using UnityEngine;
using System.Collections.Generic;
using System;
using Npu.Common;
using Random = UnityEngine.Random;

namespace Npu
{

    public class PriorityEventDispatcher<K, T, C> where C : IEqualityComparer<K>, new()
    {
        public delegate void Handler(K key, T data);

        private static Dictionary<K, PriorityList<Listener>> nameToEvent = new Dictionary<K, PriorityList<Listener>>();

        public bool Paused { get; set; }

        public PriorityEventDispatcher()
        {
            nameToEvent = new Dictionary<K, PriorityList<Listener>>(new C());
        }

        public void AddListener(K key, Handler handler, int priority = 0)
        {
            PriorityList<Listener> listeners = null;
            if (nameToEvent.TryGetValue(key, out listeners))
            {
                if (!listeners.Exists(x => x.Handler == handler))
                {
                    listeners.Add(new Listener(handler, priority));
                }
                else
                {
                    Debug.LogWarning(string.Format("PriorityEventDispatcher::AddListener : ({0}, {1}) already added",
                        key, handler));
                }
            }
            else
            {
                listeners = new PriorityList<Listener>();
                listeners.Add(new Listener(handler, priority));
                nameToEvent[key] = listeners;
            }
        }

        public void RemoveListener(K key, Handler handler)
        {
            PriorityList<Listener> listeners = null;
            if (nameToEvent.TryGetValue(key, out listeners))
            {
                listeners.RemoveAll(x => x.Handler == handler);
            }
        }

        public void RemoveAllListeners()
        {
            nameToEvent.Clear();
        }

        public void Dispatch(K key, T data)
        {
            if (Paused)
            {
                return;
            }

            PriorityList<Listener> listeners = null;
            if (nameToEvent.TryGetValue(key, out listeners))
            {
                var copied = listeners.Copy();
                foreach (var i in copied)
                {
                    i.Handler(key, data);
                }
            }
        }

        class Listener : IComparable<Listener>
        {
            private int priority;
            private Handler handler;

            public Listener(Handler handler, int priority = 0)
            {
                this.handler = handler;
                this.priority = priority;
            }

            public int Priority
            {
                get { return priority; }
                set { priority = value; }
            }

            public Handler Handler
            {
                get { return handler; }
            }

            public int CompareTo(Listener other)
            {
                return priority - other.priority;
            }

            public override string ToString()
            {
                return string.Format("[Listener: Priority={0}]", Priority);
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                var other = obj as Listener;
                if ((object) other == null)
                {
                    return false;
                }

                return handler.Equals(other.handler);
            }

            public bool Equals(Listener other)
            {
                return handler.Equals(other.handler);
            }
        }

        public static void Test()
        {
            var p = new PriorityList<Listener>();

            var ok = true;

            for (var i = 0; i < 100; i++)
            {
                p.Add(new Listener(null, Random.Range(0, 200)));
                ok = ok && p.CheckConsistent();
                Debug.Log(string.Format("Consistent ? {0}", p.CheckConsistent()));
            }

            Debug.Log(string.Format("{0} - {1}", ok, p));

        }
    }

}