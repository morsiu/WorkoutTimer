using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Timer.WorkoutPlans
{
    public readonly struct RoundCount : IEquatable<RoundCount>, IComparable<RoundCount>
    {
        private readonly int _numberOfRounds;

        public RoundCount(int numberOfRounds)
        {
            if (numberOfRounds <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfRounds), "The value must be greater than zero.");
            }
            _numberOfRounds = numberOfRounds;
        }

        public static bool operator <(RoundCount left, RoundCount right) => left.CompareTo(right) < 0;

        public static bool operator >(RoundCount left, RoundCount right) => left.CompareTo(right) > 0;

        public static bool operator <=(RoundCount left, RoundCount right) => left.CompareTo(right) <= 0;

        public static bool operator >=(RoundCount left, RoundCount right) => left.CompareTo(right) >= 0;

        public static bool operator ==(RoundCount left, RoundCount right) => left.CompareTo(right) == 0;

        public static bool operator !=(RoundCount left, RoundCount right) => left.CompareTo(right) != 0;

        public static implicit operator int(RoundCount x) => x._numberOfRounds;

        public static RoundCount? FromNumber(int number) =>
            number >= 0
                ? new RoundCount(number)
                : default(RoundCount?);

        public int CompareTo(RoundCount other) => _numberOfRounds.CompareTo(other._numberOfRounds);

        public bool Equals(RoundCount other) => CompareTo(other) == 0;

        public override bool Equals(object obj) => obj is RoundCount other && Equals(other);

        public override int GetHashCode() => _numberOfRounds;

        [Pure]
        public IEnumerable<Round> Rounds() =>
            Enumerable
                .Repeat(new Round(isLast: false), _numberOfRounds - 1)
                .Concat(new[] {new Round(isLast: true)});

        public override string ToString() => _numberOfRounds.ToString();
    }
}