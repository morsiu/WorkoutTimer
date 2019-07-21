using System;
using System.ComponentModel;
using System.Linq;
using Timer.WorkoutPlans;

namespace Timer.WorkoutPlanning
{
    public sealed class WorkoutExpression : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private enum FieldType
        {
            Break,
            Exercise,
            Countdown,
            Round
        }

        public Func<WorkoutPlan, WorkoutPlan> WorkoutPlan
        {
            get
            {
                return result =>
                {
                    var parts = Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    using (var part = parts.AsEnumerable().GetEnumerator())
                    {
                        while (part.MoveNext())
                        {
                            if (Value(part.Current) is int value
                                && part.MoveNext())
                            {
                                if (Type(part.Current) is FieldType type)
                                {
                                    if (Duration.TryFromSeconds(value) is Duration duration)
                                    {
                                        switch (type)
                                        {
                                            case FieldType.Break:
                                                result = result.AddBreak(duration);
                                                continue;
                                            case FieldType.Exercise:
                                                result = result.AddExercise(duration);
                                                continue;
                                            case FieldType.Countdown:
                                                result = result.WithCountdown(duration);
                                                continue;
                                        }
                                    }
                                    if (Count.TryFromNumber(value) is Count count)
                                    {
                                        switch (type)
                                        {
                                            case FieldType.Round:
                                                result = result.WithRound(count);
                                                continue;
                                        }
                                    }
                                    return result;
                                }
                                return result;
                            }
                            return result;

                            int? Value(string input)
                            {
                                return int.TryParse(input, out var number)
                                    ? number
                                    : default;
                            }

                            FieldType? Type(string input)
                            {
                                switch (input.ToLower())
                                {
                                    case "e": return FieldType.Exercise;
                                    case "b": return FieldType.Break;
                                    case "w": return FieldType.Countdown;
                                    case "r": return FieldType.Round;
                                    default: return null;
                                }
                            }
                        }
                    }
                    return result;
                };
            }
        }

        public string ActualValue
        {
            get
            {
                var definition = WorkoutPlan(new WorkoutPlan())
                    .Definition(
                        x => x is Duration duration
                            ? $"{duration.TotalSeconds} W"
                            : string.Empty,
                        x => $"{x.TotalSeconds} E",
                        x => $"{x.TotalSeconds} B");
                return string.Format(
                    "{0} {1} R ({2})",
                    definition.Warmup,
                    definition.Round.Number,
                    definition.Round.Workouts.Any()
                        ? string.Join(" ", definition.Round.Workouts)
                        : "empty");
            }
        }

        private string _value = string.Empty;
        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WorkoutPlan)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActualValue)));
            }
        }
    }
}
