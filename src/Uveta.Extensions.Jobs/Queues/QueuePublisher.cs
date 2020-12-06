using Uveta.Extensions.Jobs.Abstractions.Queues;
using System;
using System.Threading;
using System.Threading.Tasks;
using Uveta.Extensions.Jobs.Abstractions.Models;

namespace Uveta.Extensions.Jobs.Queues
{
    internal class QueuePublisher : IQueuePublisher
    {
        private readonly string _queueName;
        private readonly Func<Job, string, CancellationToken, Task> _publishAction;

        public QueuePublisher(string queueName, Func<Job, string, CancellationToken, Task> publishAction)
        {
            _queueName = queueName;
            _publishAction = publishAction;
        }

        public async Task EnqueueAsync(Job item, CancellationToken cancel)
        {
            await _publishAction(item, _queueName, cancel);
        }

        public void Dispose()
        {
        }
    }
}