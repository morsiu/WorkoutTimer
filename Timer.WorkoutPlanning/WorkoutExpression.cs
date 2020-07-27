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
                    using var part = parts.AsEnumerable().GetEnumerator();
                    while (part.MoveNext())
                    {
                        if (Value(part.Current) is { } value
                            && part.MoveNext())
                        {
                            if (Type(part.Current) is { } type)
                            {
                                if (Duration.TryFromSeconds(value) is { } duration)
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
                                if (Count.TryFromNumber(value) is { } count)
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

                        static int? Value(string input)
                        {
                            return int.TryParse(input, out var number)
                                ? number
                                : default;
                        }

                        static FieldType? Type(string input)
                        {
                            return input.ToLower() switch
                            {
                                "e" => FieldType.Exercise,
                                "b" => FieldType.Break,
                                "w" => FieldType.Countdown,
                                "r" => FieldType.Round,
                                _ => null
                            };
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
                        x => x is { } duration
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
