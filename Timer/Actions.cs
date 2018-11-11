using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Timer
{
    internal sealed class Actions : Collection<Action>
    {
        public Events SoundEvents(int setCount)
        {
            return new Events(
                setCount > 0
                    ? SoundEvents(this.ToList())
                    : Enumerable.Empty<IEvent>());

            IEnumerable<IEvent> SoundEvents(IReadOnlyCollection<Action> actions)
            {
                using (var sounds = new Sounds())
                {
                    var events = new SoundEvents(sounds);
                    yield return events.WarmUp(TimeSpan.FromSeconds(15));
                    foreach (var set in Sets())
                    {
                        foreach (var action in actions)
                        {
                            yield return action.Event(events);
                        }
                        if (set < setCount)
                        {
                            yield return events.SetDone();
                        }
                    }
                    yield return events.AllDone();
                    yield return DelayToAllowLastSoundToPlayOut();
                }
            }

            IEvent DelayToAllowLastSoundToPlayOut() => new Delay(TimeSpan.FromSeconds(1));
            IEnumerable<int> Sets() => Enumerable.Range(1, setCount);
        }
    }
}