#define ENABLED
#define THREAD_LOG

using System;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Npu
{

    public static class Logger
    {
        public static void ThreadLog(string format, params object[] args)
        {
            Debug.LogFormat("[Thread {0} ({1})] {2}",
                Thread.CurrentThread.ManagedThreadId,
                Thread.CurrentThread.Name,
                string.Format(format, args)
            );
        }

        public static void Log<TTag>(string format, params object[] args)
        {
            Log(typeof(TTag).ToString(), format, args);
        }

        public static void Log<TTag>(Object context, string format, params object[] args)
        {
            Log(context, typeof(TTag).ToString(), format, args);
        }

        public static void Log(string tag, string format, params object[] args)
        {
            Debug.LogFormat($"[{tag}] {format}", args);
        }

        public static void Log(Object context, string tag, string format, params object[] args)
        {
            Debug.LogFormat(context, $"[{tag}] {format}", args);
        }

        public static void Error<TTag>(string format, params object[] args)
        {
            Error(typeof(TTag).ToString(), format, args);
        }

        public static void Error<TTag>(Object context, string format, params object[] args)
        {
            Error(context, typeof(TTag).ToString(), format, args);
        }

        public static void Error(string tag, string format, params object[] args)
        {
            Debug.LogErrorFormat($"[{tag}] {format}", args);
        }

        public static void Error(Object context, string tag, string format, params object[] args)
        {
            Debug.LogErrorFormat(context, $"[{tag}] {format}", args);
        }

        public static void _LongLog(string tag, string message)
        {
#if UNITY_IPHONE
            _Log(tag, message);
#else
            const int maxLogSize = 1000;
            for (var i = 0; i <= message.Length / maxLogSize; i++)
            {
                var start = i * maxLogSize;
                var len = Math.Min(maxLogSize, message.Length - start);
                Debug.Log($"[{tag}] {message.Substring(start, len)}");
            }
#endif
        }

        public static void _Log<TTag>(string format, params object[] args)
        {
            _Log(typeof(TTag).ToString(), format, args);
        }

        public static void _Log(string tag, string format, params object[] args)
        {
#if !NP_RELEASE
            Debug.LogFormat($"[{tag}] {format}", args);
#endif
        }

        public static void _Log<TTag>(Object context, string format, params object[] args)
        {
#if !NP_RELEASE
            Debug.LogFormat(context, $"[{typeof(TTag)}] {format}", args);
#endif
        }

        public static void _Error<TTag>(Object context, string format, params object[] args)
        {
#if !NP_RELEASE
            Error(context, typeof(TTag).ToString(), format, args);
#endif
        }

        public static void _Error(Object context, string tag, string format, params object[] args)
        {
#if UNITY_EDITOR || !NP_RELEASE
            Error(context, tag, format, args);
#endif
        }

        public static void _Error<TTag>(string format, params object[] args)
        {
            _Error(typeof(TTag).ToString(), format, args);
        }

        public static void _Error(string tag, string format, params object[] args)
        {
#if UNITY_EDITOR || !NP_RELEASE
            Debug.LogErrorFormat($"[{tag}] {format}", args);
#endif
        }
    }

    public static class TimerLogger
    {
        public const string Tag = "Timer";

        static double? firstTs;
        static double? lastTs;

        private static int count = 0;

        // [Conditional("TIMER_LOG")]
        public static double Log(string format, params object[] @params)
        {
#if UNITY_EDITOR || !NP_RELEASE
            count++;
            var currentTs = Time.realtimeSinceStartup;
            if (firstTs == null) firstTs = currentTs;
            if (lastTs == null) lastTs = currentTs;

            var dt = currentTs - lastTs.Value;
            var total = currentTs - firstTs;
            Debug.LogFormat("[{0}-{1:000}] [{2:0.000}, {3:0.000}, {4:0.000}] {5}",
                Tag, count, dt, total, currentTs, string.Format(format, @params));

            lastTs = currentTs;
            return dt;
#endif
            return 0;
        }
    }
}