using System.Collections.Generic;
using UnityEngine;
using System;

namespace Npu.Common
{

    public class MainThreadExecutor : MonoBehaviour
    {
        private static Queue<Action> queue = new Queue<Action>();

        public static MainThreadExecutor Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Update()
        {
            lock (queue)
            {
                if (queue.Count > 0)
                {
                    queue.Dequeue().Invoke();
                }
            }
        }

        public void Queue(Action action)
        {
            lock (queue)
            {
                queue.Enqueue(action);
            }
        }
    }

}