namespace Uveta.Extensions.Jobs.Abstractions.Queues
{
    public interface IQueue<T>
    {
        IQueuePublisher GetPublisher(PublisherConfiguration configuration);
        IQueueConsumer GetConsumer(QueueConfiguration configuration);
    }
}
