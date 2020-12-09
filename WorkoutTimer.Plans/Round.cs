using System;

namespace WorkoutTimer.Plans
{
    public readonly struct Round : IEquatable<Round>, IComparable<Round>
    {
        public Round(int number, bool isLast)
        {
            if (number < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(number), number, "Value must be greater than 0");
            }
            Number = number;
            IsLast = isLast;
        }

        public bool IsFirst => Number == 1;

        public bool IsLast { get; }

        public int Number { get; }

        public static bool operator ==(Round left, Round right) => left.Equals(right);

        public static bool operator !=(Round left, Round right) => !left.Equals(right);

        public static bool operator >(Round left, Round right) => left.CompareTo(right) > 0;

        public static bool operator <(Round left, Round right) => left.CompareTo(right) < 0;

        public int CompareTo(Round other) => Number.CompareTo(other.Number);

        public bool Equals(Round other) => Number == other.Number;

        public override bool Equals(object obj) => obj is Round other && Equals(other);

        public override int GetHashCode() => Number;
    }
}