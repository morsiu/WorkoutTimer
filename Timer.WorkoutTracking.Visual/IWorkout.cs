using System;

#nullable enable

namespace Timer.WorkoutTracking.Visual
{
    internal interface IWorkout
    {
        void Activate(Action? complete);

        void Deactivate();
    }
}