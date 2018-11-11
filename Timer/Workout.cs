using Timer.WorkoutPlans;

namespace Timer
{
    internal sealed class Workout
    {
        public int LengthInSeconds { get; set; }
        
        public WorkoutType Type { get; set; }

        public WorkoutRound AddTo(WorkoutRound workoutRound)
        {
            var duration = Duration.TryFromSeconds(LengthInSeconds);
            if (duration == null)
            {
                return workoutRound;
            }
            switch (Type)
            {
                case WorkoutType.Exercise:
                    return workoutRound.AddExerciseWorkout(duration.Value);
                case WorkoutType.Break:
                    return workoutRound.AddBreakWorkout(duration.Value);
                default:
                    return workoutRound;
            }
        }
    }
}