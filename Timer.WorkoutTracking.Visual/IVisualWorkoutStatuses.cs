using Timer.WorkoutPlans;

namespace Timer.WorkoutTracking.Visual
{
    public interface IVisualWorkoutStatuses
    {
        IVisualWorkoutStatus WarmUp(Duration duration);

        IVisualWorkoutStatus Exercise(Duration duration, Round round);

        IVisualWorkoutStatus Break(Duration duration, Round round);

        IVisualWorkoutStatus Done();
    }
}