using System;

namespace Timer.WorkoutPlans
{
    public readonly struct Round
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

        public bool IsLast { get; }

        public int Number { get; }
    }
}