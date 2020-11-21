using System;

namespace Timer.WorkoutPlans
{
    public readonly struct Index : IEquatable<Index>, IComparable<Index>
    {
        private readonly int _value;

        public Index(int value) => _value = value;

        public int CompareTo(Index other) =>
            _value.CompareTo(other._value);

        public override bool Equals(object obj) =>
            obj is Index other && Equals(other);

        public bool Equals(Index other) =>
            other._value == _value;

        public override int GetHashCode() =>
            _value.GetHashCode();

        public override string ToString() =>
            _value.ToString();
    }
}
