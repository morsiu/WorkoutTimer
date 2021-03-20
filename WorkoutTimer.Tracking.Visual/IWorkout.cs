using System;

namespace WorkoutTimer.Tracking.Visual
{
    internal interface IWorkout
    {
        void Activate(Action? complete);

        void Deactivate();
    }
}