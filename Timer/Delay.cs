using System;
using System.Threading;
using System.Threading.Tasks;

namespace Timer
{
    internal sealed class Delay : IEvent
    {
        private readonly TimeSpan _duration;

        public Delay(TimeSpan duration)
        {
            _duration = duration;
        }
        
        public Task Run(CancellationToken cancellation)
        {
            return Task.Delay(_duration, cancellation);
        }
    }
}