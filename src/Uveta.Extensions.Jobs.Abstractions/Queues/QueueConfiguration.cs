namespace Uveta.Extensions.Jobs.Abstractions.Queues
{
    public class QueueConfiguration
    {
        public QueueConfiguration(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
