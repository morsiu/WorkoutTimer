using System;

namespace WorkoutTimer.Plans
{
    public readonly struct Index : IEquatable<Index>, IComparable<Index>
    {
        private readonly int _value;

        public Index(int value) => _value = value;

        public int CompareTo(Index other) =>
            _value.CompareTo(other._value);

        public override bool Equals(object? obj) =>
            obj is Index other && Equals(other);

        public bool Equals(Index other) =>
            other._value == _value;

        public override int GetHashCode() =>
            _value.GetHashCode();

        public override string ToString() =>
            _value.ToString();

        public static bool operator ==(Index left, Index right) => left.Equals(right);

        public static bool operator !=(Index left, Index right) => !(left == right);

        public static bool operator <(Index left, Index right) => left.CompareTo(right) < 0;

        public static bool operator <=(Index left, Index right) => left.CompareTo(right) <= 0;

        public static bool operator >(Index left, Index right) => left.CompareTo(right) > 0;

        public static bool operator >=(Index left, Index right) => left.CompareTo(right) >= 0;
    }
}
