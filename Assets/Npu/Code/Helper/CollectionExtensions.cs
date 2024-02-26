using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Npu.Helper
{
    public static class CollectionExtensions
    {
        public static TData TryGet<TData>(this IList<TData> values, int index) => index >= 0 && index < values.Count ? values[index] : default;

        public static TData TryGetClampedIndex<TData>(this IList<TData> values, int index)
            => values.Count != 0 ? values[Mathf.Clamp(index, 0, values.Count-1)] : default;

        public static bool TryGet<TData>(this IEnumerable<TData> values, Predicate<TData> predicate, out TData value)
        {
            
            foreach (var i in values)
            {
                if (!predicate.Invoke(i)) continue;
                
                value = i;
                return true;
            }

            value = default;
            return false;
        }

        public static TData Random<TData>(this IList<TData> values) where TData : class
        {
            var r = UnityEngine.Random.Range(0, values.Count);
            return values.Count == 0 ? null : values[r];
        }

        public static Dictionary<TKey, TData> Add<TKey, TData>(this Dictionary<TKey, TData> root, Dictionary<TKey, TData> additions)
        {
            foreach (var i in additions)
            {
                root[i.Key] = i.Value;
            }

            return root;
        }

        public static Dictionary<TKey, TData> Append<TKey, TData>(this Dictionary<TKey, TData> root, TKey key, TData value)
        {
            root[key] = value;
            return root;
        }

        public static void ForEach<TData>(this IEnumerable<TData> values, Action<TData> func)
        {
            foreach (var i in values) func?.Invoke(i);
        }

        public static void ForEach<TData>(this IEnumerable<TData> values, Action<TData, int> func)
        {
            var index = -1;
            foreach (var i in values)
            {
                index++;
                func?.Invoke(i, index);
            }
        }

        public static bool OrderEquals<TData>(this IEnumerable<TData> values, IEnumerable<TData> other)
        {
            if (values == null && other == null) return true;
            if (values == null || other == null) return false;
            if (values.Count() != other.Count()) return false;
            return values.Zip(other, (i, z) => (thiz: i, othez: z))
                .All(i => i.thiz.Equals(i.othez));
        }

        public static IEnumerable<TData> AsEnumerable<TData>(this TData item, params TData[] others)
        {
            yield return item;
            foreach (var i in others) yield return i;
        }

        public static List<TTarget> CastAndCache<TTarget, TSource>(this IEnumerable<TSource> sources,
            ref List<TTarget> output)
        {
            return output ?? (output = sources.OfType<TTarget>().ToList());
        }
        
        public static TTarget[] CastAndCache<TTarget, TSource>(this IEnumerable<TSource> sources, ref TTarget[] output)
        {
            return output ?? (output = sources.OfType<TTarget>().ToArray());
        }
        
        public static string Join(this IEnumerable<string> data, string separator = ",")
        {
            return string.Join(separator, data);
        }
        public static string Join<TData>(this IEnumerable<TData> data, Func<TData, string> selector, string separator=",")
        {
            return data.Select(selector.Invoke).Join(separator);
        }
    }
}