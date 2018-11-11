using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Timer
{
    internal sealed class Events
    {
        private readonly IEnumerable<IEvent> _events;

        public Events(IEnumerable<IEvent> events)
        {
            _events = events;
        }

        public async Task Run(CancellationToken cancellation)
        {
            foreach (var @event in _events)
            {
                await @event.Run(cancellation);
            }
        }
    }
}