using System;
using System.Collections;
using Npu.Helper;
using UnityEngine;
using UnityEngine.Events;

namespace Npu.Common
{

    [DefaultExecutionOrder(-1000)]
    public class Executor : MonoBehaviour
    {
        [SerializeField] private bool dontDestroyOnLoad;
        
        public static Executor Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                if (dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
                Instance = this;
            }
        }

        public Coroutine RunUpdate(Action func, float delay = 0)
        {
            return StartCoroutine(DoUpdate(func, delay));
        }

        private IEnumerator DoUpdate(Action func, float delay)
        {
            while (true)
            {
                func();

                yield return new WaitForSeconds(delay);
            }
        }

        public Coroutine RunLateUpdate(Action func, float delay = 0)
        {
            return StartCoroutine(DoLateUpdate(func, delay));
        }

        private IEnumerator DoLateUpdate(Action func, float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                yield return new WaitForEndOfFrame();

                func();
            }
        }

        public Coroutine Run(Action<float> func, float duration, bool realtime, UnityAction onComplete)
        {
            return StartCoroutine(DoRun(func, duration, realtime, onComplete));
        }

        private IEnumerator DoRun(Action<float> func, float duration, bool realtime, UnityAction onComplete)
        {
            float elapsed = 0;
            while (elapsed <= duration)
            {
                func(elapsed / duration);

                elapsed += realtime ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }

            onComplete?.Invoke();
        }

        private IEnumerator DoRun(Action func, float delay, float duration, UnityAction onComplete)
        {
            yield return new WaitForSeconds(delay);

            float elapsed = 0;
            while (elapsed <= duration)
            {
                func();

                elapsed += Time.deltaTime;
                yield return null;
            }

            onComplete?.Invoke();
        }

        public Coroutine Delay(float delay, Action callback, bool isRealtime = false)
        {
            return CoroutineUtils.Delay(this, delay, callback, isRealtime);
            
        }

        public void ExecuteOnEndFrame(Action func)
        {
            this.WaitForEndOfFrame(func);
        }
        
        public Coroutine ScheduleRepeat(Action<float> func, float period, bool realtime)
        {
            return StartCoroutine(IRepeat(func, period, realtime));
        }

        private static IEnumerator IRepeat(Action<float> func, float period, bool realtime = false)
        {
            while (true)
            {
                func(period);

                if (realtime)
                    yield return new WaitForSecondsRealtime(period);
                else
                    yield return new WaitForSeconds(period);
            }
        }

        public Coroutine ScheduleRepeat(Action func, float duration, float period, bool realtime,
            Action onComplete)
        {
            return StartCoroutine(IRepeat(func, duration, period, realtime, onComplete));
        }

        private IEnumerator IRepeat(Action func, float duration, float period, bool realtime = false,
            Action onComplete = null)
        {
            float elapsed = 0;
            while (elapsed <= duration)
            {
                func();

                if (realtime)
                    yield return new WaitForSecondsRealtime(period);
                else
                    yield return new WaitForSeconds(period);
                elapsed += period;
            }

            onComplete?.Invoke();
        }

        public void WaitUntil(Func<bool> condition, Action callback)
        {
            CoroutineUtils.WaitUntil(this, condition, callback);
        }
        
    }

}