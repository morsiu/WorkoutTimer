using System;

#nullable enable

namespace WorkoutTimer.Tracking.Visual
{
    internal interface IWorkout
    {
        void Activate(Action? complete);

        void Deactivate();
    }
}