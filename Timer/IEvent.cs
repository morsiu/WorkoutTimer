using System.Threading;
using System.Threading.Tasks;

namespace Timer
{
    internal interface IEvent
    {
        Task Run(CancellationToken cancellation);
    }
}