using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SharpToolkit.AccessSynchronization
{
    internal static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T PopLast<T>(this LinkedList<T> list)
        {
            var last = list.Last.Value;
            list.RemoveLast();

            return last;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T PopFirst<T>(this LinkedList<T> list)
        {
            var last = list.First.Value;
            list.RemoveLast();

            return last;
        }
    }

    public static class LinqExtensions
    {
        public static T Random<T>(this IEnumerable<T> list)
        {
            var i = 0;
            T result = default;

            var rand = new Random();

            foreach (var item in list)
            {
                if (rand.Next(0, i + 1) == i)
                    result = item;

                i += 1;
            }

            return result;
        }

        public static T Random<T>(this IEnumerable<T> list, Random rand)
        {
            var i = 0;
            T result = default;

            foreach (var item in list)
            {
                if (rand.Next(0, i + 1) == i)
                    result = item;

                i += 1;
            }

            return result;
        }
    }

    public static class FunctionalExtensions
    {
        public static Func<int> AsFunc(this Action action)
        {
            return new Func<int>(() => { action(); return 0; });
        }

        public static Func<T, int> AsFunc<T>(this Action<T> action)
        {
            return new Func<T, int>(x => { action(x); return 0; });
        }

        public static Func<T1, T2, int> AsFunc<T1, T2>(this Action<T1, T2> action)
        {
            return new Func<T1, T2, int>((a, b) => { action(a, b); return 0; });
        }

        public static Func<T1, T2, T3, int> AsFunc<T1, T2, T3>(this Action<T1, T2, T3> action)
        {
            return new Func<T1, T2, T3, int>((a, b, c) => { action(a, b, c); return 0; });
        }

        public static Func<T1, T2, T3, T4, int> AsFunc<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action)
        {
            return new Func<T1, T2, T3, T4, int>((a, b, c, d) => { action(a, b, c, d); return 0; });
        }

        public static Func<T1, T2, T3, T4, T5, int> AsFunc<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action)
        {
            return new Func<T1, T2, T3, T4, T5, int>((a, b, c, d, e) => { action(a, b, c, d, e); return 0; });
        }

        public static Func<T1, T2, T3, T4, T5, T6, int> AsFunc<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action)
        {
            return new Func<T1, T2, T3, T4, T5, T6, int>((a, b, c, d, e, f) => { action(a, b, c, d, e, f); return 0; });
        }

        public static void Deconstruct<K, V>(this KeyValuePair<K, V> kvp, out K key, out V value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }

        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }

        public static LinkedList<T> ToLinkedList<T>(this IEnumerable<T> collection)
        {
            return collection.Aggregate(new LinkedList<T>(), (aggr, cur) => { aggr.AddLast(cur); return aggr; });
        }
    }
}
