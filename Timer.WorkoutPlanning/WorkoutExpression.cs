using System;
using System.ComponentModel;
using System.Linq;
using Timer.WorkoutPlans;

namespace Timer.WorkoutPlanning
{
    public sealed class WorkoutExpression : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public WorkoutRound WorkoutRound
        {
            get
            {
                var parts = Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var result = new WorkoutRound();
                using (var part = parts.AsEnumerable().GetEnumerator())
                {
                    while (part.MoveNext())
                    {
                        if (Duration(part.Current) is Duration duration
                            && part.MoveNext())
                        {
                            if (Type(part.Current) is WorkoutType type)
                            {
                                switch (type)
                                {
                                    case WorkoutType.Break:
                                        result = result.AddBreakWorkout(duration);
                                        break;
                                    case WorkoutType.Exercise:
                                        result = result.AddExerciseWorkout(duration);
                                        break;
                                    default:
                                        return result;
                                }
                            }
                            else
                            {
                                return result;
                            }
                        }
                        else
                        {
                            return result;
                        }

                        Duration? Duration(string input)
                        {
                            return int.TryParse(input, out var seconds)
                                ? WorkoutPlans.Duration.TryFromSeconds(seconds)
                                : default;
                        }

                        WorkoutType? Type(string input)
                        {
                            switch (input)
                            {
                                case "E": return WorkoutType.Exercise;
                                case "B": return WorkoutType.Break;
                                default: return null;
                            }
                        }
                    }
                }
                return result;
            }
        }

        public string ActualValue
        {
            get
            {
                return string.Join(
                    " ",
                    WorkoutRound.Select(
                        exercise: x => $"{x.TotalSeconds} E",
                        @break: x => $"{x.TotalSeconds} B"));
            }
        }

        private string _value = string.Empty;
        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WorkoutRound)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActualValue)));
            }
        }
    }
}
