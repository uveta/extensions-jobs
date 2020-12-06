namespace Uveta.Extensions.Jobs.Abstractions.Queues
{
    public interface IQueue
    {
        IQueuePublisher GetPublisher(PublisherConfiguration configuration);
        IQueueConsumer GetConsumer(QueueConfiguration configuration);
    }
}
