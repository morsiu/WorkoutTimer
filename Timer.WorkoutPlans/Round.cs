namespace Timer.WorkoutPlans
{
    public readonly struct Round
    {
        public Round(bool isLast)
        {
            IsLast = isLast;
        }

        public bool IsLast { get; }
    }
}