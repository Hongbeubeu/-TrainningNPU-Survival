using System.Collections.Generic;
using System;

namespace Npu
{

    public class EventQueue<T>
    {
        private List<T> queue = new List<T>();

        private Action<T> handler;

        public Action<T> Handler
        {
            get => handler;
            set
            {
                handler = value;
                if (handler == null || queue.Count <= 0) return;

                Logger.Log<EventQueue<T>>("Delivering queued events");
                foreach (var i in queue)
                {
                    handler.Invoke(i);
                }

                queue.Clear();
            }
        }

        public void Queue(T @object)
        {
            if (Handler != null)
            {
                Handler.Invoke(@object);
            }
            else
            {
                Logger.Log<EventQueue<T>>("No Handler. Queue event");
                queue.Add(@object);
            }
        }

    }
}