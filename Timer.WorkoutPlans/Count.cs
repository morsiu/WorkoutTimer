using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Timer.WorkoutPlans
{
    public readonly struct Count : IEquatable<Count>, IComparable<Count>
    {
        private readonly int _value;

        public Count(int value)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "The value must be greater than zero.");
            }
            _value = value;
        }

        public static bool operator <(Count left, Count right) => left.CompareTo(right) < 0;

        public static bool operator >(Count left, Count right) => left.CompareTo(right) > 0;

        public static bool operator <=(Count left, Count right) => left.CompareTo(right) <= 0;

        public static bool operator >=(Count left, Count right) => left.CompareTo(right) >= 0;

        public static bool operator ==(Count left, Count right) => left.CompareTo(right) == 0;

        public static bool operator !=(Count left, Count right) => left.CompareTo(right) != 0;

        public static implicit operator int(Count x) => x._value;

        public static Count? FromNumber(int number) =>
            number >= 0
                ? new Count(number)
                : default(Count?);

        public int CompareTo(Count other) => _value.CompareTo(other._value);

        public bool Equals(Count other) => CompareTo(other) == 0;

        public override bool Equals(object obj) => obj is Count other && Equals(other);

        public override int GetHashCode() => _value;

        [Pure]
        public IEnumerable<Round> Rounds() =>
            Enumerable
                .Range(1, _value - 1)
                .Select(x => new Round(number: x, isLast: false))
                .Concat(new[] {new Round(number: _value, isLast: true)});

        public override string ToString() => _value.ToString();
    }
}