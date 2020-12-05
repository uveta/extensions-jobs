namespace Uveta.Extensions.Jobs.Abstractions.Queues
{
    public class ConsumerConfiguration
    {
        public int MaximumDeliveryCount { get; set; } = 3;

        public ConsumerConfiguration UseMaximumDeliveryCount(int count)
        {
            MaximumDeliveryCount = count;
            return this;
        }

        public static bool Validate(ConsumerConfiguration configuration)
        {
            if (configuration.MaximumDeliveryCount <= 0) return false;
            return true;
        }
    }
}
