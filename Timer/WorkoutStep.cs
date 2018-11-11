using System;
using Timer.WorkoutPlans;

namespace Timer
{
    internal sealed class WorkoutStep
    {
        public TimeSpan Length { get; set; }
        
        public WorkoutStepPurpose Purpose { get; set; }

        public IWorkoutStep ToWorkoutStep()
        {
            var duration = Duration.FromSeconds((int)Length.TotalSeconds);
            if (duration == null)
            {
                return null;
            }
            switch (Purpose)
            {
                case WorkoutStepPurpose.Exercise:
                    return new Exercise(duration.Value);
                case WorkoutStepPurpose.Break:
                    return new Break(duration.Value);
                default:
                    return null;
            }
        }
    }
}