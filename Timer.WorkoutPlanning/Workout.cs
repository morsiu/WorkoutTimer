using System.ComponentModel;
using Timer.WorkoutPlans;

namespace Timer.WorkoutPlanning
{
    public sealed class Workout : INotifyPropertyChanged
    {
        private int _lengthInSeconds;
        private WorkoutType _type;

        public event PropertyChangedEventHandler PropertyChanged;

        public int LengthInSeconds
        {
            get => _lengthInSeconds;
            set
            {
                _lengthInSeconds = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LengthInSeconds)));
            }
        }

        public WorkoutType Type
        {
            get => _type;
            set
            {
                _type = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Type)));
            }
        }

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