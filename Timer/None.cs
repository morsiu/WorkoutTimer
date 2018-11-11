using System.Threading;
using System.Threading.Tasks;

namespace Timer
{
    internal sealed class None : IEvent
    {
        public Task Run(CancellationToken cancellation)
        {
            return Task.CompletedTask;
        }
    }
}