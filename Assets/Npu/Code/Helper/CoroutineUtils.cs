using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Npu.Helper
{
    public static class CoroutineUtils
    {
        public static Coroutine WaitForFrames(this MonoBehaviour component, int frames, Action action)
        {
            return component.StartCoroutine(IEWaitForFrames(action, frames));
        }

        public static Coroutine WaitForNextFrame(this MonoBehaviour component, Action action)
        {
            return component.StartCoroutine(IEWaitForNextFrame(action));
        }

        public static Coroutine WaitForEndOfFrame(this MonoBehaviour component, Action action)
        {
            return component.StartCoroutine(IEWaitForEndOfFrame(action));
        }

        public static Coroutine Delay(this MonoBehaviour component, float delay, Action action, bool realtime = false)
        {
            return component.StartCoroutine(IEDelay(delay, action, realtime));
        }

        public static Coroutine WaitUntil(this MonoBehaviour component, Func<bool> predicate, Action action)
        {
            return component.StartCoroutine(WaitUntil(predicate, action));
        }

        private static IEnumerator WaitUntil(Func<bool> predicate, Action action)
        {
            while (!predicate.Invoke())
                yield return null;

            action?.Invoke();
        }

        private static IEnumerator IEWaitForNextFrame(Action action)
        {
            yield return null;
            action?.Invoke();
        }

        private static IEnumerator IEWaitForEndOfFrame(Action action)
        {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }

        private static IEnumerator IEWaitForFrames(Action action, int frames)
        {
            while (frames-- > 0) yield return null;
            action?.Invoke();
        }

        private static IEnumerator IEDelay(float delay, Action action, bool realtime = false)
        {
            if (realtime)
                yield return new WaitForSecondsRealtime(delay);
            else
                yield return new WaitForSeconds(delay);

            action?.Invoke();
        }
        
        
    }
}