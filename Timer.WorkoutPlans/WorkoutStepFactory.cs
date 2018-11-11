namespace Timer.WorkoutPlans
{
    public static class WorkoutStepFactory
    {
        public static IWorkoutStep Break(Duration duration) => new Break(duration);

        public static IWorkoutStep Exercise(Duration duration) => new Exercise(duration);
    }
}
