using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

using Random = UnityEngine.Random;

namespace Npu.Common
{

    public class PriorityLinkedList<T> : IEnumerable<T>, ICollection<T> where T : IComparable<T>
    {
        LinkedList<T> nodes = new LinkedList<T>();

        public int Count => nodes.Count;
        public bool IsReadOnly => false;

        public void Add(T item)
        {
            if (nodes.First == null)
            {
                nodes.AddFirst(item);
                return;
            }

            var n = nodes.First;
            while (n != null && n.Value.CompareTo(item) > 0)
            {
                n = n.Next;
            }

            if (n == null) nodes.AddLast(item);
            else nodes.AddBefore(n, item);
        }

        public bool Remove(T item) => nodes.Remove(item);
        public void Clear() => nodes.Clear();
        public bool Contains(T item) => nodes.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => nodes.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => nodes.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => nodes.GetEnumerator();

        public bool Consistent()
        {
            var n1 = nodes.First;
            var n2 = n1?.Next;

            while (n1 != null && n2 != null)
            {
                if (n1.Value.CompareTo(n2.Value) < 0) return false;
                n1 = n2;
                n2 = n1?.Next;
            }

            return true;
        }

        public string Description
        {
            get
            {
                var builder = new StringBuilder();
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    builder.Append(e.Current.ToString() + ",");
                }

                return builder.ToString();

            }
        }

        public static void OneTest()
        {
            var t = new PriorityLinkedList<int>();
            var count = Random.Range(20, 40);
            for (var i = 0; i < count; i++)
            {
                t.Add(Random.Range(0, 1000));
            }

            if (!t.Consistent()) Logger.Error("TEST", t.Description);
            else Logger.Log("TEST", t.Description);
        }
    }

}