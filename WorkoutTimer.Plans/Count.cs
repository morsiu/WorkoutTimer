using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkoutTimer.Plans
{
    public readonly struct Count : IEquatable<Count>, IComparable<Count>
    {
        private readonly int _valueMinusOne;

        public Count(int value)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "The value must be greater than zero.");
            }
            _valueMinusOne = value - 1;
        }

        private int Value => _valueMinusOne + 1;

        public static implicit operator int(Count x) => x.Value;

        public static Count? TryFromNumber(int number) =>
            number > 0
                ? new Count(number)
                : default(Count?);

        public int CompareTo(Count other) => _valueMinusOne.CompareTo(other._valueMinusOne);

        public bool Equals(Count other) => CompareTo(other) == 0;

        public override bool Equals(object obj) => obj is Count other && Equals(other);

        public IEnumerable<T> Enumerate<T>(
            Func<(int Number, bool IsLast), T> element) =>
            Enumerable
                .Range(1, _valueMinusOne)
                .Select(x => element((x, false)))
                .Concat(new[] { element((Value, true)) });

        public override int GetHashCode() => _valueMinusOne;

        public override string ToString() => Value.ToString();

        public static bool operator <(Count left, Count right) => left.CompareTo(right) < 0;

        public static bool operator >(Count left, Count right) => left.CompareTo(right) > 0;

        public static bool operator <=(Count left, Count right) => left.CompareTo(right) <= 0;

        public static bool operator >=(Count left, Count right) => left.CompareTo(right) >= 0;

        public static bool operator ==(Count left, Count right) => left.CompareTo(right) == 0;

        public static bool operator !=(Count left, Count right) => left.CompareTo(right) != 0;
    }
}