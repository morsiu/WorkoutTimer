using Timer.WorkoutPlans;

namespace Timer
{
    internal sealed class WorkoutRoundStep
    {
        public int LengthInSeconds { get; set; }
        
        public WorkoutStepPurpose Purpose { get; set; }

        public WorkoutRound AddTo(WorkoutRound workoutRound)
        {
            var duration = Duration.TryFromSeconds(LengthInSeconds);
            if (duration == null)
            {
                return workoutRound;
            }
            switch (Purpose)
            {
                case WorkoutStepPurpose.Exercise:
                    return workoutRound.AddExercise(duration.Value);
                case WorkoutStepPurpose.Break:
                    return workoutRound.AddBreak(duration.Value);
                default:
                    return workoutRound;
            }
        }
    }
}