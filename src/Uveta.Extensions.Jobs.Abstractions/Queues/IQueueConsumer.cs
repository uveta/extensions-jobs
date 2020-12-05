using System;
using System.Threading;
using System.Threading.Tasks;
using Uveta.Extensions.Jobs.Abstractions.Models;

namespace Uveta.Extensions.Jobs.Abstractions.Queues
{
    public interface IQueueConsumer : IDisposable
    {
        Task StartAsync(
            int? maximumConcurrentCalls = null,
            CancellationToken cancel = default);

        event Func<Job, Delivery, Task> JobQueued;
        event Func<ErrorArgs, Delivery, Task> Error;
    }
}
