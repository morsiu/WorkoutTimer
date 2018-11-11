using System;

namespace Timer.WorkoutPlans
{
    public readonly struct SetCount : IEquatable<SetCount>, IComparable<SetCount>
    {
        private readonly int _numberOfSets;

        public SetCount(int numberOfSets)
        {
            if (numberOfSets <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfSets), "The value must be greater than zero.");
            }
            _numberOfSets = numberOfSets;
        }

        public static bool operator <(SetCount left, SetCount right) => left.CompareTo(right) < 0;

        public static bool operator >(SetCount left, SetCount right) => left.CompareTo(right) > 0;

        public static bool operator <=(SetCount left, SetCount right) => left.CompareTo(right) <= 0;

        public static bool operator >=(SetCount left, SetCount right) => left.CompareTo(right) >= 0;

        public static bool operator ==(SetCount left, SetCount right) => left.CompareTo(right) == 0;

        public static bool operator !=(SetCount left, SetCount right) => left.CompareTo(right) != 0;

        public static implicit operator int(SetCount x) => x._numberOfSets;

        public static SetCount? FromNumber(int number) =>
            number >= 0
                ? new SetCount(number)
                : default(SetCount?);

        public int CompareTo(SetCount other) => _numberOfSets.CompareTo(other._numberOfSets);

        public bool Equals(SetCount other) => CompareTo(other) == 0;

        public override bool Equals(object obj) => obj is SetCount other && Equals(other);

        public override int GetHashCode() => _numberOfSets;

        public override string ToString() => _numberOfSets.ToString();
    }
}