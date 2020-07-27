using System;
using System.Collections.Generic;

namespace Timer.WorkoutTracking.Sound
{
    internal static class EnumerableExtensions 
    {
        public static IEnumerable<T> Append<T>(
            this IEnumerable<T> sequence,
            Func<T, T> toAppendFromLast,
            Func<T> toAppendFromEmpty)
        {
            using var enumerator = sequence.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                yield return toAppendFromEmpty();
                yield break;
            }
            yield return enumerator.Current;
            var last = enumerator.Current;
            while (enumerator.MoveNext())
            {
                last = enumerator.Current;
                yield return enumerator.Current;
            }
            yield return toAppendFromLast(last);
        }

        public static IEnumerable<(T Item, TAggregate Sum)> Scan<T, TAggregate>(
            this IEnumerable<T> sequence,
            TAggregate zero,
            Func<TAggregate, T, TAggregate> concat)
        {
            var sum = zero;
            foreach (var element in sequence)
            {
                sum = concat(sum, element);
                yield return (element, sum);
            }
        }
    }
}