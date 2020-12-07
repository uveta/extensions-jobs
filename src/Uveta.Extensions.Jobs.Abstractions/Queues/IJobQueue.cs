namespace Uveta.Extensions.Jobs.Abstractions.Queues
{
    public interface IJobQueue
    {
        IQueuePublisher GetPublisher(PublisherConfiguration configuration);
        IQueueConsumer GetConsumer(QueueConfiguration configuration);
    }
}
