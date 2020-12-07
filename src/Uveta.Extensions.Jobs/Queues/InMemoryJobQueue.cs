using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Uveta.Extensions.Jobs.Abstractions.Models;
using Uveta.Extensions.Jobs.Abstractions.Queues;

namespace Uveta.Extensions.Jobs.Queues
{
    internal class InMemoryJobQueue : IJobQueue
    {
        private readonly InMemoryJobQueueConfiguration _configuration;

        public InMemoryJobQueue(IOptions<InMemoryJobQueueConfiguration> options)
        {
            _configuration = options.Value;
        }

        private readonly ConcurrentDictionary<string, QueueConsumer> _consumers =
            new ConcurrentDictionary<string, QueueConsumer>();

        public IQueueConsumer GetConsumer(QueueConfiguration configuration)
        {
            string queueName = configuration.Name;
            return _consumers.GetOrAdd(
                queueName,
                _ => new QueueConsumer(() => RemoveConsumer(queueName)));
        }

        public IQueuePublisher GetPublisher(PublisherConfiguration configuration)
        {
            string queueName = configuration.Queue.Name;
            return new QueuePublisher(queueName, PublishMessageAsync);
        }

        private async Task PublishMessageAsync(Job item, string queueName, CancellationToken cancel)
        {
            if (!_consumers.TryGetValue(queueName, out var consumer)) return;
            await consumer.OnNewJobAsync(item, _configuration.WaitForHandlers);
        }

        private void RemoveConsumer(string queueName)
        {
            _consumers.TryRemove(queueName, out _);
        }
    }
}