using System;
using System.Threading;
using System.Threading.Tasks;
using Uveta.Extensions.Jobs.Abstractions.Models;

namespace Uveta.Extensions.Jobs.Abstractions.Queues
{
    public interface IQueuePublisher : IDisposable
    {
        Task EnqueueAsync(Job job, CancellationToken cancel);
    }
}
