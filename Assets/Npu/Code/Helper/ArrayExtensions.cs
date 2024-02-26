using System.Collections.Generic;
using System.Linq;
using Npu.Utilities;
using UnityEngine;

/***
 * @authors: Thanh Le (William)  
 */

namespace Npu.Helper
{
    public static class ArrayExtensions
    {

        #region List Get

        public static T TryGet<T>(this IList<T> l, int i, T defaultVal = default)
        {
            if (i < 0 || i >= l.Count) return defaultVal;
            return l[i];
        }

        public static T GetRepeat<T>(this IList<T> l, int i)
        {
            return l[Numerics.Repeat(i, l.Count)];
        }

        public static T GetPingPong<T>(this IList<T> l, int i)
        {
            return l[(int)Numerics.PingPong(i, l.Count - 1)];
        }

        public static T GetClamped<T>(this IList<T> l, int i)
        {
            return l[Mathf.Clamp(i, 0, l.Count - 1)];
        }

        public static T GetRandom<T>(this IList<T> l)
        {
            if (l != null && l.Count > 0)
            {
                return l[Random.Range(0, l.Count)];
            }
            return default;
        }

        #endregion



        #region Array Get

        public static T GetRandom<T>(this T[] l)
        {
            if (l != null && l.Length > 0)
            {
                return l[Random.Range(0, l.Length)];
            }
            return default;
        }

        #endregion



        #region IEnum Get

        public static T GetRepeat<T>(this IEnumerable<T> l, int i)
        {
            return l.Skip(Numerics.Repeat(i, l.Count())).FirstOrDefault();
        }

        public static T GetPingPong<T>(this IEnumerable<T> l, int i)
        {
            return l.Skip((int)Numerics.PingPong(i, l.Count() - 1)).FirstOrDefault();
        }

        public static T GetClamped<T>(this IEnumerable<T> l, int i)
        {
            return l.Skip(Mathf.Clamp(i, 0, l.Count() - 1)).FirstOrDefault();
        }

        public static T GetRandom<T>(this IEnumerable<T> l)
        {
            return l.Skip(Random.Range(0, l.Count())).FirstOrDefault();
        }

        public static T GetRandom<T>(this IEnumerable<T> l, System.Func<T, string> seedFunc)
        {
            if (l == null || !l.Any()) return default;

            string hashFunc(T obj)
            {
                var seed = seedFunc?.Invoke(obj) ?? obj.GetHashCode().ToString();
                var hashed = StringUtils.FastHash(seed);
                return hashed;
            }
            return l.OrderBy(hashFunc).FirstOrDefault();
        }

        public static T GetRandom<T>(this IEnumerable<T> l, System.Func<T, int, string> seedFunc)
        {
            if (l == null || !l.Any()) return default;

            string hashFunc((T, int) pair)
            {
                var seed = seedFunc?.Invoke(pair.Item1, pair.Item2) ?? (pair.Item1.GetHashCode().ToString() + pair.Item2);
                var hashed = StringUtils.FastHash(seed);
                return hashed;
            }
            return l.Select((li, i) => (li, i)).OrderBy(hashFunc).Select(zz => zz.li).FirstOrDefault();
        }

        public static IEnumerable<T> GetRandom<T>(this IEnumerable<T> l, int count, System.Func<T, string> seedFunc)
        {
            if (l == null || !l.Any()) return default;
            return l.SeededShuffle(seedFunc).Take(count);
        }

        // get all first element of all lists, then all 2nd element, then all 3rd elements
        public static IEnumerable<T> LoopAcross<T>(this IEnumerable<IEnumerable<T>> ieie)
        {
            var enumerators = ieie.Select(ie => ie.GetEnumerator()).ToList();
            var any = true;
            while (any)
            {
                any = false;
                foreach (var enu in enumerators)
                {
                    if (enu.MoveNext())
                    {
                        any = true;
                        yield return enu.Current;
                    }
                }
            }
        }
        #endregion



        #region IEnum Shuffle

        public static IEnumerable<T> SeededShuffle<T>(this IEnumerable<T> l, System.Func<T, string> seedFunc)
        {
            if (l == null || !l.Any()) return default;
            string hashFunc(T obj)
            {
                var seed = seedFunc?.Invoke(obj) ?? obj.GetHashCode().ToString();
                var hashed = StringUtils.FastHash(seed);
                return hashed;
            }
            var shuffled = l.OrderBy(hashFunc);
            return shuffled;
        }

        public static IEnumerable<T> RandomShuffle<T>(this IEnumerable<T> l)
        {
            if (l == null || !l.Any()) return default;
            return l.OrderBy(o => Random.value);
        }
        
        public static IEnumerable<T> GetRandomElements<T>(this IEnumerable<T> l, int count)
        {
            if (l == null || !l.Any()) return default;
            return l.RandomShuffle().Take(count);
        }

        #endregion



        #region IEnum Miscs

        public static float RatioOf<T>(this IEnumerable<T> arr, System.Predicate<T> pred, int rounds = -1)
        {
            var count = 0;
            var good = 0;
            foreach (var t in arr)
            {
                count++;
                if (pred?.Invoke(t) ?? false) good++;
            }
            var f = (float)good / count;
            if (rounds >= 0) f = (float)System.Math.Round(f, rounds);
            return f;
        }

        public static bool HasDuplicate<T>(this IEnumerable<T> ie)
        {
            var set = new HashSet<T>();
            foreach (var i in ie)
            {
                if (!set.Add(i)) return true;
            }
            return false;
        }

        public static Dictionary<T, T> Match<T, K>(this IEnumerable<T> ie1, IEnumerable<T> ie2, System.Func<T, K> keyFunc)
        {
            return ie1.Match(ie2, keyFunc, keyFunc);
        }

        public static Dictionary<T, T> Match<T, K>(this IEnumerable<T> ie1, IEnumerable<T> ie2, System.Func<T, K> key1Func, System.Func<T, K> key2Func)
        {
            var d1 = ie1.ToLookup(i => i, i => key1Func(i));
            var d2 = ie2.ToLookup(i => key2Func(i), i => i);
            return d1.ToDictionary(g1 => g1.Key, g1 => d2.Contains(g1.FirstOrDefault()) ? d2[g1.FirstOrDefault()].FirstOrDefault() : default);
        }

        public static IEnumerable<K> ChainPair<T, K>(this IEnumerable<T> points, System.Func<T, T, K> paring)
        {
            var i = 0;
            T prev = default;
            foreach (var p in points)
            {
                if (i > 0)
                {
                    yield return paring(p, prev);
                }
                prev = p;
                i++;
            }
        }

        public static IEnumerable<T> SendToBack<T>(this IEnumerable<T> ie, int count)
        {
            var firsts = Enumerable.Empty<T>();
            var passed = 0;
            foreach (var e in ie)
            {
                if (passed >= count) yield return e;
                else
                {
                    firsts = firsts.Append(e);
                }
                passed++;
            }
            foreach (var f in firsts) yield return f;
            //return ie.Skip(count).Concat(ie.Take(count));
        }
        #endregion



        #region Split

        public static IEnumerable<IEnumerable<T>> SplitEvery<T>(this IEnumerable<T> l, int every)
        {
            every = Mathf.Max(1, every);

            var enumerator = l.GetEnumerator();
            var ended = !enumerator.MoveNext();

            IEnumerable<T> loop()
            {
                var i = 0;
                while (i < every && !ended)
                {
                    yield return enumerator.Current;
                    i++;
                    ended = !enumerator.MoveNext();
                }
            }

            while (!ended)
            {
                yield return loop();
            }

        }

        public static IEnumerable<IEnumerable<T>> SplitWhere<T>(this IEnumerable<T> l, System.Predicate<T> where)
        {
            var enumerator = l.GetEnumerator();
            var ended = !enumerator.MoveNext();

            IEnumerable<T> loop()
            {
                var any = false;
                while (!ended && (!any || !where(enumerator.Current)))
                {
                    yield return enumerator.Current;
                    ended = !enumerator.MoveNext();
                    any = true;
                }
            }

            while (!ended)
            {
                yield return loop();
            }

        }

        #endregion



        #region IDict

        public static K GetOrDefault<T, K>(this IDictionary<T, K> dict, T key, K defaultOverride = default)
        {
            if (dict.TryGetValue(key, out var val))
            {
                return val;
            }
            return defaultOverride;
        }

        public static K GetOrDefaultFunc<T, K>(this IDictionary<T, K> dict, T key, System.Func<K> defaultFunc = null)
        {
            if (dict.TryGetValue(key, out var val))
            {
                return val;
            }
            if (defaultFunc != null) return defaultFunc.Invoke();
            return default;
        }

        public static Dictionary<V, K> Flip<K, V>(this IDictionary<K, V> dict)
        {
            return dict.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }

        public static Dictionary<V, K> FlipSafe<K, V>(this IDictionary<K, V> dict, System.Func<IGrouping<V, K>, K> picker = null)
        {
            if (picker == null) picker = g => g.FirstOrDefault();
            return dict.GroupBy(kvp => kvp.Value, kvp => kvp.Key).ToDictionary(g => g.Key, g => picker(g));
        }

        #endregion

    }
}
