using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Uveta.Extensions.Jobs.Abstractions.Models;
using Uveta.Extensions.Jobs.Abstractions.Queues;

namespace Uveta.Extensions.Jobs.Queues
{
    internal class InMemoryJobQueue : IQueue
    {
        // TODO: add publisher option not to wait for consumers
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
            await consumer.OnNewItemAsync(item);
        }

        private void RemoveConsumer(string queueName)
        {
            _consumers.TryRemove(queueName, out _);
        }
    }
}