namespace Uveta.Extensions.Jobs.Abstractions.Queues
{
    public class PublisherConfiguration
    {
        public QueueConfiguration Queue { get; }

        public PublisherConfiguration(QueueConfiguration queue)
        {
            Queue = queue;
        }
    }
}
