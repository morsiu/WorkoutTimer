using Timer.WorkoutPlans;

namespace Timer
{
    internal sealed class WorkoutStep
    {
        public int LengthInSeconds { get; set; }
        
        public WorkoutStepPurpose Purpose { get; set; }

        public IWorkoutStep ToWorkoutStep()
        {
            var duration = Duration.FromSeconds(LengthInSeconds);
            if (duration == null)
            {
                return null;
            }
            switch (Purpose)
            {
                case WorkoutStepPurpose.Exercise:
                    return WorkoutStepFactory.Exercise(duration.Value);
                case WorkoutStepPurpose.Break:
                    return WorkoutStepFactory.Break(duration.Value);
                default:
                    return null;
            }
        }
    }
}