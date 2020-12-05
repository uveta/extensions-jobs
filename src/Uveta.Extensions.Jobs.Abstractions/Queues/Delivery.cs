namespace Uveta.Extensions.Jobs.Abstractions.Queues
{
    public class Delivery
    {
        public Delivery(bool supportsRepeat)
        {
            SupportsRepeat = supportsRepeat;
        }

        public bool Acknowledge { get; set; } = false;
        public bool SupportsRepeat { get; }
    }
}
