using Timer.WorkoutPlans;

namespace Timer.WorkoutTracking.Visual
{
    internal interface IWorkouts<out T>
    {
        T Warmup(Duration duration);

        T Exercise(Round round, Duration duration);

        T Break(Round round, Duration duration);
    }
}