using Timer.WorkoutPlans;

namespace Timer.WorkoutTracking.Visual
{
    internal sealed class Workouts : IWorkouts<(Duration Duration, IWorkout Workout)>
    {
        public (Duration Duration, IWorkout Workout) Warmup(Duration duration)
        {
            return (duration, new Workout(WorkoutType.WarmUp, duration, round: null));
        }

        public (Duration Duration, IWorkout Workout) Exercise(Round round, Duration duration)
        {
            return (duration, new Workout(WorkoutType.Exercise, duration, round));
        }

        public (Duration Duration, IWorkout Workout) Break(Round round, Duration duration)
        {
            return (duration, new Workout(WorkoutType.Break, duration, round));
        }
    }
}